using NSubstitute;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using System.Threading.Channels;

namespace Settlement.External.UnitTests.Services;

public class InstantSettingServiceTests
{
    [Fact]
    public async Task ChangeInstantSetting_WritesValidMatchesToChannel()
    {
        // Arrange
        var matchItems = new List<MatchItem>
        {
            new MatchItem(1, ""), // valid
            new MatchItem(2, "NO Instant Settlement"), // valid
            new MatchItem(42, ""), // sabaLeague, should be filtered out
            new MatchItem(3, ""), // valid
        };
        var request = new ChangeInstantSettingRequest(matchItems);
        var matchDetails = new List<MatchDetail>
        {
            new() { MatchId = 1, ParentID = 0, LeagueId = 10,  EventStatus = "running" },
            new() { MatchId = 2, ParentID = 0, LeagueId = 10,  EventStatus = "running" },
            new() { MatchId = 42, ParentID = 0, LeagueId = 42 }, // in sabaLeague
            new() { MatchId = 3, ParentID = 0, LeagueId = 73 }, // in sabaLeague
        };
        var matchService = Substitute.For<IMatchService>();
        matchService.GetMatchDetailsAsync(Arg.Any<string>()).Returns(matchDetails);
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        // Act
        await service.ChangeInstantSetting(request);
        var written = await channel.Reader.ReadAsync();

        // Assert
        Assert.Equal(2, written.Count); // Only MatchId 1 and 2 are valid
        Assert.Contains(written, x => x.MatchId == 1 && x.Enable);
        Assert.Contains(written, x => x.MatchId == 2 && !x.Enable);
    }

    [Fact]
    public async Task ChangeInstantSetting_EnableIsSetCorrectly()
    {
        // Arrange
        var matchItems = new List<MatchItem>
        {
            new MatchItem(2, "NO Instant Settlement"), // valid
            new MatchItem(6, "OtherRemark"), // Enable should be true
        };
        var request = new ChangeInstantSettingRequest(matchItems);
        var matchDetails = new List<MatchDetail>
        {
            new() { MatchId = 2, ParentID = 0, LeagueId = 10, EventStatus = "running"},
            new() { MatchId = 6, ParentID = 0, LeagueId = 10 , EventStatus = "running"},
        };
        var matchService = Substitute.For<IMatchService>();
        matchService.GetMatchDetailsAsync(Arg.Any<string>()).Returns(matchDetails);
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        // Act
        await service.ChangeInstantSetting(request);
        var written = await channel.Reader.ReadAsync();

        // Assert
        Assert.Contains(written, x => x.MatchId == 2 && !x.Enable);
        Assert.Contains(written, x => x.MatchId == 6 && x.Enable);
    }
}