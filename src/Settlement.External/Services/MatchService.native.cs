using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface IMatchService
{
    Task<List<MatchDetail>> GetMatchDetailsAsync(string matchIds);

    Task<MatchResultChangeResponse> GetMatchesResultChange(ChangeResultMatchRequest request);
}

public class MatchService(IDynamicRepository repository) : IMatchService
{
    public async Task<List<MatchDetail>> GetMatchDetailsAsync(string matchIds)
    {
        var criteria = new Criteria.GetMatchDetailCriteria
        {
            MatchIds = matchIds
        };

        var result = await repository.FetchAsync<MatchDetail>(criteria);

        return [.. result];
    }

    public async Task<MatchResultChangeResponse> GetMatchesResultChange(ChangeResultMatchRequest request)
    {
        var criteria = new Criteria.GetResultChangeCriteria
        {
            ToDate = DateTime.Parse(request.ToDate),
            IsTest = request.IsTest
        };
        
        if (!string.IsNullOrEmpty(request.FromDate))
        {
            criteria.FromDate = DateTime.Parse(request.FromDate);
        }
        var result = await repository.FetchAsync<MatchResult>(criteria);
        var groupedResults = result
            .GroupBy(m => m.MatchId)
            .Select(g => g.ToList())
            .ToList();
        var errMsg = criteria.ErrMsg;
        
        return new MatchResultChangeResponse
        {
            Results = groupedResults,
            ErrorMsg = errMsg
        };
    }
}