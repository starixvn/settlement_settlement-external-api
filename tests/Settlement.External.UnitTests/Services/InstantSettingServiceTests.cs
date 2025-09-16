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
            new() { MatchId = 1, ParentID = 0, LeagueGroupId = 10 },
            new() { MatchId = 2, ParentID = 0, LeagueGroupId = 10 },
            new() { MatchId = 42, ParentID = 0, LeagueGroupId = 42 }, // in sabaLeague
            new() { MatchId = 3, ParentID = 0, LeagueGroupId = 74 }, // in sabaLeague
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

    [Fact]
    public async Task ChangeInstantSetting_ReturnsError_WhenValidationFails()
    {
        // Arrange
        var matchService = Substitute.For<IMatchService>();
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        var request = new ChangeInstantSettingRequest(new List<MatchItem>()); // Invalid: empty list

        // Act
        var result = await service.ChangeInstantSetting(request);

        // Assert
        Assert.NotEqual(0, result.ErrorCode);
        Assert.NotEqual("Success", result.ErrorMsg);
    }

    [Fact]
    public async Task ChangeInstantSetting_DoesNotPush_WhenNoValidMatches()
    {
        // Arrange
        var matchService = Substitute.For<IMatchService>();
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        var request = new ChangeInstantSettingRequest(new List<MatchItem>
        {
            new MatchItem(1, "remark")
        });

        matchService.GetMatchDetailsAsync(Arg.Any<string>())
            .Returns(new List<MatchDetail>()); // No matching details

        // Act
        var result = await service.ChangeInstantSetting(request);

        // Assert
        Assert.Equal(0, result.ErrorCode);
        Assert.Equal("Success", result.ErrorMsg);
        Assert.False(channel.Reader.TryRead(out _));
    }

    [Fact]
    public async Task ChangeInstantSetting_PushesToChannel_WhenValidMatch()
    {
        // Arrange
        var matchService = Substitute.For<IMatchService>();
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        var request = new ChangeInstantSettingRequest(new List<MatchItem>
        {
            new MatchItem(1, "remark")
        });

        matchService.GetMatchDetailsAsync(Arg.Any<string>())
            .Returns(new List<MatchDetail>
            {
                new MatchDetail
                {
                    MatchId = 1,
                    ParentID = 0,
                    EventStatus = "pending",
                    LeagueGroupId = 1
                }
            });

        // Act
        var result = await service.ChangeInstantSetting(request);

        // Assert
        Assert.Equal(0, result.ErrorCode);
        Assert.Equal("Success", result.ErrorMsg);
        Assert.True(channel.Reader.TryRead(out var matchList));
        Assert.Single(matchList);
        Assert.Equal(1, matchList[0].MatchId);
    }

    [Fact]
    public async Task ChangeInstantSetting_DisablesInstant_WhenRemarkContainsNoInstant()
    {
        // Arrange
        var matchService = Substitute.For<IMatchService>();
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        var request = new ChangeInstantSettingRequest(new List<MatchItem>
        {
            new MatchItem(2, "NoInstant")
        });

        matchService.GetMatchDetailsAsync(Arg.Any<string>())
            .Returns(new List<MatchDetail>
            {
                new MatchDetail
                {
                    MatchId = 2,
                    ParentID = 0,
                    EventStatus = "pending",
                    LeagueGroupId = 1
                }
            });

        // Act
        await service.ChangeInstantSetting(request);

        // Assert
        Assert.True(channel.Reader.TryRead(out var matchList));
        Assert.Single(matchList);
        Assert.True(matchList[0].Enable);
    }

    [Fact]
    public async Task ChangeInstantSetting_IgnoresCompletedOrSabaLeagueMatches()
    {
        // Arrange
        var matchService = Substitute.For<IMatchService>();
        var channel = Channel.CreateUnbounded<List<MatchListItem>>();
        var service = new InstantSettingService(matchService, channel);

        var request = new ChangeInstantSettingRequest(new List<MatchItem>
        {
            new MatchItem(3, "remark"),
            new MatchItem(4, "remark")
        });

        matchService.GetMatchDetailsAsync(Arg.Any<string>())
            .Returns(new List<MatchDetail>
            {
                new MatchDetail
                {
                    MatchId = 3,
                    ParentID = 0,
                    EventStatus = "completed",
                    LeagueGroupId = 1
                },
                new MatchDetail
                {
                    MatchId = 4,
                    ParentID = 0,
                    EventStatus = "pending",
                    LeagueGroupId = 42 // Saba league
                }
            });

        // Act
        await service.ChangeInstantSetting(request);

        // Assert
        Assert.False(channel.Reader.TryRead(out _));
    }
}