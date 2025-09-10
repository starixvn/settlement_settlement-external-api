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
    private static readonly List<int> sabaLeague = [42, 73, 108];

    public async Task<Result> ChangeInstantSetting(ChangeInstantSettingRequest request)
    {
        var validationResult = ChangeInstantSettingRequestValidator.ValidateRequest(request);

        if (validationResult.IsFail)
        {
            return new Result { ErrorCode = validationResult.ErrorCode, ErrorMsg = validationResult.ErrorMsg };
        }

        var matchIds = string.Join(",", request.MatchItems.Select(item => item.MatchId));
        var matchDetails = await matchService.GetMatchDetailsAsync(matchIds);

        var validMatches = request.MatchItems
            .Where(item => IsValidMatch(item, matchDetails))
            .ToList();

        var matchList = validMatches.Select(match => new MatchListItem
        {
            MatchId = match.MatchId,
            Enable = !match.Remark.Contains(RemarkValue.NoInstant, StringComparison.OrdinalIgnoreCase)
        });

        await channel.Writer.WriteAsync([.. matchList]);

        return new Result();
    }

    private static bool IsValidMatch(MatchItem item, List<MatchDetail> matchDetails)
    {
        const string eventStatus = "running";

        return matchDetails.Exists(detail => IsValidMatchDetail(item, detail, eventStatus));
    }

    private static bool IsValidMatchDetail(MatchItem item, MatchDetail detail, string eventStatus)
    {
        if (detail.MatchId != item.MatchId || detail.ParentID != 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(detail.EventStatus))
        {
            return false;
        }

        if (!detail.EventStatus.Equals(eventStatus, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !sabaLeague.Contains(detail.LeagueId);
    }
}