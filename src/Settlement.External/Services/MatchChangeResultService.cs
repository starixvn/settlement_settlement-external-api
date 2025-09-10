using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface MatchChangeResultService
{
    Task<string> GetDistinctMatchesWithChanges(ChangeResultMatchRequest request);
}

public class MatchChangeResultServiceImp(IMatchService matchService) : MatchChangeResultService
{
    public async Task<string> GetDistinctMatchesWithChanges(ChangeResultMatchRequest request)
    {
        var eventStatusMatches = await matchService.GetMatchesEventStatusChange(request);
        var resultChangeMatches = await matchService.GetMatchesResultChange(request);

        var distinctMatches = eventStatusMatches
            .Union(resultChangeMatches)
            .Distinct()
            .ToList();

        return string.Join(",", distinctMatches);
    }
}