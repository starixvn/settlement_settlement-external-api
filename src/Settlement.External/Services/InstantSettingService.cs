using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;
using System.Text.Json;

namespace Settlement.External.Services;

public interface IInstantSettingService
{
    Task<Result> ChangeInstantSetting(ChangeInstantSettingRequest request);
}

public class InstantSettingService : IInstantSettingService
{
    private readonly static List<int> sabaLeague = [42, 73, 108];
    private readonly static List<int> pingoalLeague = [252, 113];
    private readonly IDynamicRepository _repository;
    private readonly IMatchService matchService;

    public InstantSettingService(IDynamicRepository repository, IMatchService matchService)
    {
        _repository = repository;
        this.matchService = matchService;

    }

    public async Task<Result> ChangeInstantSetting(ChangeInstantSettingRequest request)
    {
        var matchIds = string.Join(",", request.MatchItems.Select(x => x.MatchId));
        var matchesDetail = await matchService.GetMatchDetailsAsync(matchIds);

        var validMatches = request.MatchItems
            .Where(x => matchesDetail.Exists(m => m.MatchId == x.MatchId
                && m.ParentID == 0 && !sabaLeague.Contains(m.LeagueId)))
            .ToList();

        var matchList = validMatches.Select(x => new MatchListItem
        {
            MatchId = x.MatchId,
            Enable = !x.Remark.Contains(RemarkValue.NoInstant)
        });

        var matchListJson = JsonSerializer.Serialize(
            matchList,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


        var criteria = new ChangeInstantSettingCriteria
        {
            MatchList = matchListJson
        };

        await _repository.ExecuteAsync(criteria);

        foreach (var item in matchList)
        {
            WriteLogUpdateMatchSetting(item.MatchId, item.Enable);
        }


        return new Result();
    }

    private void WriteLogUpdateMatchSetting(int matchId, bool enable)
    {

        var msg = $"NAP: Update auto setting on match({matchId}) has Instant({enable})";
        var logCriteria = new LogCriteria(matchId, msg, ActionId.AutoPayoutSettingMatch, 100);

        _repository.Execute(logCriteria);
    }
}
