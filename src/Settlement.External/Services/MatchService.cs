using Settlement.External.Models;

namespace Settlement.External.Services;

public class MatchService(IDynamicRepository repository)
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
}
