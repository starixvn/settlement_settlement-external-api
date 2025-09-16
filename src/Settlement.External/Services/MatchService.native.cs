using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface IMatchService
{
    Task<List<MatchDetail>> GetMatchDetailsAsync(string matchIds);

    Task<List<int>> GetMatchesResultChange(ChangeResultMatchRequest request);
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

    public async Task<List<int>> GetMatchesResultChange(ChangeResultMatchRequest request)
    {
        var criteria = new Criteria.GetResultChangeCriteria
        {
            FromDate = DateTime.Parse(request.FromDate),
            ToDate = DateTime.Parse(request.ToDate),
            IsTest = request.IsTest
        };
        var result = await repository.FetchAsync<int>(criteria);
        return [.. result];
    }
}