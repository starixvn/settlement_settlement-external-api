using Fanex.Data.Repository;
using Microsoft.AspNetCore.Http;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using StackExchange.Redis;

namespace Settlement.External.Services;

public interface MatchSettlementService
{
    Task<Result> UpdateMatchSettlementStatus(SettlementStatusUpdRequest model);
}

public class MatchSettlementServiceImp : MatchSettlementService
{
    private readonly IDynamicRepository repository;
    private readonly IHttpContextAccessor accessor;
    private readonly IConnectionMultiplexer connectionMultiplexer;
    private const string KeyConfirmRefund = "SettlementConfirmRefund";

    public MatchSettlementServiceImp(IDynamicRepository repository, IHttpContextAccessor accessor, IConnectionMultiplexer connectionMultiplexer)
    {
        this.repository = repository;
        this.accessor = accessor;
        this.connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Result> UpdateMatchSettlementStatus(SettlementStatusUpdRequest model)
    {
        var criteria = new MatchSettlementStatusUpdCriteria
        {
            MatchId = model.MatchId,
            Type = model.Type,
            Ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString()
        };

        await repository.ExecuteAsync(criteria);

        var result = new Result
        {
            ErrorCode = criteria.ErrorCode,
            ErrorMsg = criteria.ErrMsg,
        };

        SetConfirmRefundKeyCache(model, result);

        return result;
    }

    private void SetConfirmRefundKeyCache(SettlementStatusUpdRequest model, Result result)
    {
        if (model.Type != SettlementStatusType.Refund || result.IsFail) { return; }

        var db = connectionMultiplexer.GetDatabase(13);

        var key = $"{KeyConfirmRefund}:{model.MatchId}:01,00";

        if (model.IsEnabled)
        {
            db.StringSet(key, true, TimeSpan.FromMinutes(10));
        }
        else
        {
            db.KeyDelete(key);
        }
    }
}