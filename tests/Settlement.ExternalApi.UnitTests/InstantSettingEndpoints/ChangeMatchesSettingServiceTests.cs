using Fanex.Data.Repository;
using NSubstitute;
using Settlement.External.Criteria;
using Settlement.ExternalApi.InstantSettingEndpoints;
using System.Threading.Channels;

namespace Settlement.ExternalApi.UnitTests.InstantSettingEndpoints;

public class ChangeMatchesSettingServiceTests
{
    [Fact]
    public async Task ExecuteAsync_ProcessesMatchListAndCallsRepositoryAndLogs()
    {
        // Arrange
        var matchList = new List<MatchListItem>
        {
            new MatchListItem { MatchId = 1, Enable = true },
            new MatchListItem { MatchId = 2, Enable = false }
        };
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        await channel.Writer.WriteAsync(matchList);
        var repository = Substitute.For<IDynamicRepository>();
        var service = new ChangeMatchesSettingService(channel, repository);
        var cancellationToken = CancellationToken.None;

        // Act
        await service.StartAsync(cancellationToken);
        await service.StopAsync(cancellationToken);

        // Assert
        await repository.Received(1).ExecuteAsync(Arg.Any<ChangeInstantSettingCriteria>(), cancellationToken: Arg.Any<CancellationToken>());

        repository.Received().Execute(Arg.Is<LogCriteria>(c => c.MatchId == 1 && c.Remark.Contains("Instant(True)")));
        repository.Received().Execute(Arg.Is<LogCriteria>(c => c.MatchId == 2 && c.Remark.Contains("Instant(False)")));
    }
}
