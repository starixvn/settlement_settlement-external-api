
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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var matchList = await channel.Reader.ReadAsync(stoppingToken);

        var matchListJson = JsonSerializer.Serialize(
            matchList,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


        var criteria = new ChangeInstantSettingCriteria
        {
            MatchList = matchListJson
        };

        await repository.ExecuteAsync(criteria, cancellationToken: stoppingToken);

        foreach (var item in matchList)
        {
            WriteLogUpdateMatchSetting(item.MatchId, item.Enable);
        }
    }

    private void WriteLogUpdateMatchSetting(int matchId, bool enable)
    {

        var msg = $"NAP: Update auto setting on match({matchId}) has Instant({enable})";
        var logCriteria = new LogCriteria(matchId, msg, ActionId.AutoPayoutSettingMatch, 111);

        repository.Execute(logCriteria);
    }
}
