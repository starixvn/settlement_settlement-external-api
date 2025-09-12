using Fanex.Data.Repository;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using System.Text.Json;
using System.Threading.Channels;

namespace Settlement.ExternalApi.InstantSettingEndpoints;

public class ChangeMatchesSettingService(
    Channel<List<MatchListItem>> channel,
    IDynamicRepository repository) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var matchList = (await channel.Reader.ReadAsync(stoppingToken))
                .ToList();

            await ProcessMatches(matchList, stoppingToken);
        }
    }

    private async Task ProcessMatches(List<MatchListItem> matchList, CancellationToken stoppingToken)
    {
        var matchListJson = JsonSerializer.Serialize(matchList, JsonOptions);

        var criteria = new ChangeInstantSettingCriteria
        {
            MatchList = matchListJson
        };

        var matchListChange = await repository.FetchAsync<int>(criteria, cancellationToken: stoppingToken);

        var tasks = matchList
            .Where(x => matchListChange.Contains(x.MatchId))
            .Select(matchItem => WriteLogUpdateMatchSetting(matchItem.MatchId, matchItem.Enable))
            .ToList();

        await Task.WhenAll(tasks);
    }

    private async Task WriteLogUpdateMatchSetting(int matchId, bool enable)
    {
        var message = $"NAP: Update auto setting on match({matchId}) has Instant({enable})";
        var logCriteria = new LogCriteria(matchId, message, ActionId.AutoPayoutSettingMatch);

        await repository.ExecuteAsync(logCriteria);
    }
}