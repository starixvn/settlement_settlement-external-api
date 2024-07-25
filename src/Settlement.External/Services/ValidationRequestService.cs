using Microsoft.Extensions.Options;
using Settlement.External.Common;

using Settlement.External.Constants;
using Settlement.External.Models;

namespace Settlement.External.Services;

public interface ValidationRequestService
{
    Result ValidResult(QueryRequest model);

    Result MultiValidation(ListQueryRequest model);

    bool ValidToken(string token);

    Result ValidSettlementStatusRequest(SettlementStatusUpdRequest model);

    Result ValidGoalRecordRequest(GoalRecordRequest model);
};

public class ValidationRequestServiceImp(IOptions<AppSetting> appSettingOptions) : ValidationRequestService
{
    public Result ValidResult(QueryRequest model)
    {
        if (InvalidParameter(model))
        {
            return Result.ErrorParameter;
        }

        if (!ValidToken(model.SecurityToken))
        {
            return Result.ErrorSecurityToken;
        }

        return ValidScore(model.ResultInfo);
    }

    public Result MultiValidation(ListQueryRequest model)
    {
        foreach (var item in model.Matches)
        {
            var queryMatch = new QueryRequest
            {
                MatchId = item.MatchId,
                SecurityToken = model.SecurityToken,
                ResultInfo = item.ResultInfo
            };

            var result = ValidResult(queryMatch);

            if (result.IsFail)
            {
                return result;
            }
        }

        return new Result();
    }

    private Result ValidScore(IList<Score> resultInfo)
    {
        foreach (var item in resultInfo)
        {
            if (!Helper.IsNumber(item.HomeValue))
            {
                return Result.ErrorHomeValue;
            }

            if (!Helper.IsNumber(item.AwayValue))
            {
                return Result.ErrorAwayValue;
            }
        }

        return new Result { ErrorMsg = ErrorMessage.Success };
    }

    private bool InvalidParameter(QueryRequest model) => model.MatchId < 1
        || string.IsNullOrWhiteSpace(model.SecurityToken)
        || model.ResultInfo == null;

    public bool ValidToken(string token) => !string.IsNullOrEmpty(token)
        && appSettingOptions.Value.SecurityToken.Equals(token, StringComparison.CurrentCultureIgnoreCase);

    public Result ValidSettlementStatusRequest(SettlementStatusUpdRequest model)
    {
        if (!ValidToken(model.SecurityToken))
        {
            return Result.ErrorSecurityToken;
        }

        short[] acceptTypes = [SettlementStatusType.HoldKey, SettlementStatusType.Refund, SettlementStatusType.MthHold];

        if (!acceptTypes.Contains(model.Type))
        {
            return Result.ErrorParameter;
        }

        return new Result();
    }

    public Result ValidGoalRecordRequest(GoalRecordRequest model)
    {
        var queryRequest = new QueryRequest
        {
            MatchId = model.MatchId,
            SecurityToken = model.SecurityToken,
            ResultInfo = model.ResultInfo.Select(score => new Score
            {
                HomeValue = score.HomeValue,
                AwayValue = score.AwayValue,
                SourceId = score.SourceId,
            }).ToList()
        };

        return ValidResult(queryRequest);
    }
}