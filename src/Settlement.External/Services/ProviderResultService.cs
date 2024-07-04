using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.DTOs;
using Settlement.External.Models;
using ErrorCodes = Settlement.External.Constants.ErrorCode;

namespace Settlement.External.Services;

public interface ProviderResultService
{
    Task<List<ScoreDto>> GetResultByMatchId(int matchId);

    Task<MatchDto> GetMatch(int matchId);

    Task<Result> Insert(QueryRequest model);

    Task<Result> InsertMultiMatches(ListQueryRequest model);
}

public class ProviderResultServiceImp(IDynamicRepository repository) : ProviderResultService
{
    public async Task<List<ScoreDto>> GetResultByMatchId(int matchId)
    {
        var criteria = new GetResultByMatchIdCriteria
        {
            MatchId = matchId
        };

        var result = await repository.FetchAsync<ScoreDto>(criteria);

        return result.ToList();
    }

    public async Task<MatchDto> GetMatch(int matchId)
    {
        var results = await GetResultByMatchId(matchId);
        var result = new MatchDto
        {
            MatchId = matchId,
            ResultInfo = results,
        };

        return result;
    }

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

    public async Task<Result> InsertMultiMatches(ListQueryRequest model)
    {
        var matchFail = string.Empty;
        foreach (var match in model.Matches)
        {
            var queryModel = new QueryRequest { MatchId = match.MatchId, ResultInfo = match.ResultInfo };

            var resultItem = await Insert(queryModel);

            if (resultItem.IsFail)
            {
                matchFail = string.Join(", ", match.MatchId);
            }
        }

        if (!string.IsNullOrEmpty(matchFail))
        {
            return new Result { ErrorCode = ErrorCodes.FailMatch, ErrorMsg = ErrorMessage.MatchFailed + matchFail };
        }

        return new Result();
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