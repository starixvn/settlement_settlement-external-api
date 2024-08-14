using Microsoft.Extensions.Options;
using Settlement.External.Common;

using Settlement.External.Constants;
using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface ValidationRequestService
{
    Result ValidResult(QueryRequest model);

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

    private static Result ValidScore(IList<Score> resultInfo)
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

    private static bool InvalidParameter(QueryRequest model) => model.MatchId < 1
                                                                || string.IsNullOrWhiteSpace(model.SecurityToken)
                                                                || model.ResultInfo == null;

    private bool ValidToken(string token) => !string.IsNullOrEmpty(token)
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