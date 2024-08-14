using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface ProviderResultService
{
    Task<Result> Insert(QueryRequest model);
}

public class ProviderResultServiceImp(IDynamicRepository repository) : ProviderResultService
{
    public async Task<Result> Insert(QueryRequest model)
    {
        var result = new Result();
        foreach (var score in model.ResultInfo)
        {
            try
            {
                return await InsertResultDetails(model.MatchId, score);
            }
            catch (Exception exception)
            {
                result.ErrorCode = ErrorCode.SystemError;
                result.ErrorMsg = exception.Message;
            }
        }

        return result;
    }

    private async Task<Result> InsertResultDetails(int matchId, Score score)
    {
        var criteria = new AddResultCriteria
        {
            MatchId = matchId,
            SourceId = score.SourceId,
            TypeId = score.TypeId,
            HomeValue = score.HomeValue,
            AwayValue = score.AwayValue
        };

        await repository.ExecuteAsync(criteria);

        return new Result
        {
            ErrorCode = criteria.ErrCode,
            ErrorMsg = criteria.ErrMsg
        };
    }
}