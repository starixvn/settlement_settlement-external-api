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
    private static readonly List<int> sabaLeague = [42, 74, 108, 113, 252];

    public async Task<Result> ChangeInstantSetting(ChangeInstantSettingRequest request)
    {
        var validationResult = ChangeInstantSettingRequestValidator.ValidateRequest(request);

        if (validationResult.IsFail)
        {
            return new Result { ErrorCode = validationResult.ErrorCode, ErrorMsg = validationResult.ErrorMsg };
        }

        var matchIds = string.Join(",", request.MatchItems.Select(item => item.MatchId).Distinct());
        var matchDetails = await matchService.GetMatchDetailsAsync(matchIds);

        var validMatches = request.MatchItems
            .DistinctBy(x => x.MatchId)
            .Where(item => IsValidMatch(item, matchDetails))
            .ToList();

        var matchList = validMatches.Select(match => new MatchListItem
        {
            MatchId = match.MatchId,
            Enable = !match.Remark.Contains(RemarkValue.NoInstant, StringComparison.OrdinalIgnoreCase)
        }).ToList();

        if (matchList.Count != 0)
        {
            await channel.Writer.WriteAsync(matchList);
        }

        return new Result();
    }

    private static bool IsValidMatch(MatchItem item, List<MatchDetail> matchDetails)
    {
        return matchDetails.Exists(detail =>
            detail.MatchId == item.MatchId &&
            detail.ParentID == 0 &&
            !string.Equals(detail.Eventstatus ?? string.Empty, "completed", StringComparison.OrdinalIgnoreCase) &&
            !sabaLeague.Contains(detail.LeagueGroupId));
    }
}