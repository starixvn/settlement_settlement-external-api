using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;

namespace Settlement.External.Services;

public class InstantSettingService
{
    private readonly static List<int> sabaLeague = [42, 73, 108];
    private readonly static List<int> pingoalLeague = [252, 113];
    private readonly IDynamicRepository _repository;
    private readonly MatchService matchService;
    public InstantSettingService(IDynamicRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result> ChangeInstantSetting(ChangeInstantSettingRequest request)
    {
        var matchIds = string.Join(",", request.MatchItems.Select(x => x.MatchId));
        var matchesDetail = await matchService.GetMatchDetailsAsync(matchIds);

        var validMatches = request.MatchItems
            .Where(x => matchesDetail.Exists(m => m.MatchId == x.MatchId
                && m.ParentID == 0 && !sabaLeague.Contains(m.LeagueId)))
            .ToList();


        var criteria = new ChangeInstantSettingCriteria
        {

        };
        var result = await _repository.ExecuteAsync(criteria);


        return new Result();
    }

    private void WriteLogUpdateMatchSetting(int matchId, bool enable)
    {

        var msg = $"NAP: Update auto setting on match({matchId}) has Instant({enable})";
        var logCriteria = new LogCriteria(matchId, msg, ActionId.AutoPayoutSettingMatch, 100);

        _repository.Execute(logCriteria);
    }
}
