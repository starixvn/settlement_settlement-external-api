using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;
using System.Threading.Channels;

namespace Settlement.External.Services;

public interface IInstantSettingService
{
    Task<Result> ChangeInstantSetting(ChangeInstantSettingRequest request);
}

public class InstantSettingService(
    IMatchService matchService,
    Channel<List<MatchListItem>> channel) : IInstantSettingService
{
    private readonly static List<int> sabaLeague = [42, 73, 108];

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

        await channel.Writer.WriteAsync([.. matchList]);

        return new Result();
    }
}
