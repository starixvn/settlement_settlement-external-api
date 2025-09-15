using Microsoft.Extensions.Options;
using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public interface MatchChangeResultService
{
    Task<string> GetDistinctMatchesWithChanges(ChangeResultMatchRequest request);
}

public class MatchChangeResultServiceImp(IMatchService matchService,
    IOptions<AppSetting> appSettingsKey) : MatchChangeResultService
{
    public async Task<string> GetDistinctMatchesWithChanges(ChangeResultMatchRequest request)
    {
        request.IsTest = appSettingsKey.Value.EnvironmentName != "PRO";
        var eventStatusMatches = await matchService.GetMatchesEventStatusChange(request);
        var resultChangeMatches = await matchService.GetMatchesResultChange(request);

        var distinctMatches = eventStatusMatches
            .Union(resultChangeMatches)
            .Distinct()
            .ToList();

        return string.Join(",", distinctMatches);
    }
}