using Microsoft.Extensions.Options;
using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface MatchChangeResultService
{
    Task<List<List<MatchResult>>> GetDistinctMatchesWithChanges(ChangeResultMatchRequest request);
}

public class MatchChangeResultServiceImp(IMatchService matchService,
    IOptions<AppSetting> appSettingsKey) : MatchChangeResultService
{
    public async Task<List<List<MatchResult>>> GetDistinctMatchesWithChanges(ChangeResultMatchRequest request)
    {
        request.IsTest = appSettingsKey.Value.EnvironmentName != "PRO";
        var resultChangeMatches = await matchService.GetMatchesResultChange(request);

        return resultChangeMatches
            .GroupBy(m => m.MatchId)
            .Select(g => g.ToList())
            .ToList();
    }
}