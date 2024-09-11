using Microsoft.AspNetCore.Http;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;
using StackExchange.Redis;

namespace Settlement.External.Services;

public interface MatchSettlementService
{
    Task<Result> UpdateMatchSettlementStatus(SettlementStatusUpdRequest model);
}

public class MatchSettlementServiceImp(
    IDynamicRepository repository,
    IHttpContextAccessor accessor,
    IConnectionMultiplexer connectionMultiplexer)
    : MatchSettlementService
{
    private const string KeyConfirmRefund = "SettlementConfirmRefund";

    public async Task<Result> UpdateMatchSettlementStatus(SettlementStatusUpdRequest model)
    {
        var criteria = new MatchSettlementStatusUpdCriteria
        {
            MatchId = model.MatchId,
            Type = model.Type,
            AdminId = model.AdminId,
            IsEnabled = model.IsEnabled,
            Ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString()
        };

        await repository.ExecuteAsync(criteria);

        var result = new Result
        {
            ErrorCode = criteria.ErrCode,
            ErrorMsg = criteria.ErrMsg,
        };

        SetConfirmRefundKeyCache(model, result);

        return result;
    }

    private void SetConfirmRefundKeyCache(SettlementStatusUpdRequest model, Result result)
    {
        if (model.Type != SettlementStatusType.Refund || result.IsFail)
        {
            return;
        }

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