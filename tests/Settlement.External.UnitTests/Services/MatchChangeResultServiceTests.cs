using Microsoft.Extensions.Options;
using NSubstitute;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;

namespace Settlement.External.UnitTests.Services;

public class MatchChangeResultServiceTests
{
    private readonly IMatchService matchService;
    private readonly MatchChangeResultServiceImp service;

    public MatchChangeResultServiceTests()
    {
        matchService = Substitute.For<IMatchService>();
        var appSettings = Substitute.For<IOptions<AppSetting>>();
        appSettings.Value.Returns(new AppSetting { EnvironmentName = "TEST" });
        service = new MatchChangeResultServiceImp(matchService, appSettings);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeMatchesExist_ReturnsGroupedMatchResultList()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>
            {
                new List<MatchResult> { new MatchResult { MatchId = 1, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 2, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 3, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 4, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } }
            },
            ErrorMsg = "Success"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal(4, result.Results.Count); // 4 groups
        Assert.Single(result.Results[0]); // Group 1 has 1 item
        Assert.Single(result.Results[1]); // Group 2 has 1 item
        Assert.Single(result.Results[2]); // Group 3 has 1 item
        Assert.Single(result.Results[3]); // Group 4 has 1 item
        Assert.Equal(1, result.Results[0][0].MatchId);
        Assert.Equal(2, result.Results[1][0].MatchId);
        Assert.Equal(3, result.Results[2][0].MatchId);
        Assert.Equal(4, result.Results[3][0].MatchId);
        Assert.Equal("Success", result.ErrorMsg);
        await matchService.Received(1).GetMatchesResultChange(request);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>(),
            ErrorMsg = "No data found"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Empty(result.Results);
        Assert.Equal("No data found", result.ErrorMsg);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeHasMatches_ReturnsGroupedMatchResultList()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>
            {
                new List<MatchResult> { new MatchResult { MatchId = 4, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 5, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 6, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } }
            },
            ErrorMsg = "Success"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal(3, result.Results.Count); // 3 groups
        Assert.Single(result.Results[0]); // Group 1 has 1 item
        Assert.Single(result.Results[1]); // Group 2 has 1 item
        Assert.Single(result.Results[2]); // Group 3 has 1 item
        Assert.Equal(4, result.Results[0][0].MatchId);
        Assert.Equal(5, result.Results[1][0].MatchId);
        Assert.Equal(6, result.Results[2][0].MatchId);
        Assert.Equal("Success", result.ErrorMsg);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeListIsEmpty_ReturnsEmptyList()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>(),
            ErrorMsg = "No data found"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Empty(result.Results);
        Assert.Equal("No data found", result.ErrorMsg);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeListHasDuplicates_ReturnsGroupedMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>
            {
                new List<MatchResult> 
                { 
                    new MatchResult { MatchId = 1, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") },
                    new MatchResult { MatchId = 1, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") }
                },
                new List<MatchResult> 
                { 
                    new MatchResult { MatchId = 2, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") },
                    new MatchResult { MatchId = 2, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") }
                },
                new List<MatchResult> 
                { 
                    new MatchResult { MatchId = 3, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") },
                    new MatchResult { MatchId = 3, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") }
                },
                new List<MatchResult> 
                { 
                    new MatchResult { MatchId = 4, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") }
                }
            },
            ErrorMsg = "Success"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal(4, result.Results.Count); // 4 groups
        Assert.Equal(2, result.Results[0].Count); // Group 1 has 2 items
        Assert.Equal(2, result.Results[1].Count); // Group 2 has 2 items
        Assert.Equal(2, result.Results[2].Count); // Group 3 has 2 items
        Assert.Single(result.Results[3]); // Group 4 has 1 item
        Assert.Equal(1, result.Results[0][0].MatchId);
        Assert.Equal(1, result.Results[0][1].MatchId);
        Assert.Equal(2, result.Results[1][0].MatchId);
        Assert.Equal(2, result.Results[1][1].MatchId);
        Assert.Equal(3, result.Results[2][0].MatchId);
        Assert.Equal(3, result.Results[2][1].MatchId);
        Assert.Equal(4, result.Results[3][0].MatchId);
        Assert.Equal("Success", result.ErrorMsg);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeListHasSingleMatch_ReturnsSingleGroup()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>
            {
                new List<MatchResult> 
                { 
                    new MatchResult { MatchId = 1, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") }
                }
            },
            ErrorMsg = "Success"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Single(result.Results); // 1 group
        Assert.Single(result.Results[0]); // Group has 1 item
        Assert.Equal(1, result.Results[0][0].MatchId);
        Assert.Equal(DateTimeOffset.Parse("2025-09-19 02:50:52.390"), result.Results[0][0].ChangeTime);
        Assert.Equal("Success", result.ErrorMsg);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WithLargeMatchId_ReturnsCorrectGroup()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>
            {
                new List<MatchResult> 
                { 
                    new MatchResult { MatchId = 1000000, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") }
                }
            },
            ErrorMsg = "Success"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Single(result.Results); // 1 group
        Assert.Single(result.Results[0]); // Group has 1 item
        Assert.Equal(1000000, result.Results[0][0].MatchId);
        Assert.Equal(DateTimeOffset.Parse("2025-09-19 02:50:52.390"), result.Results[0][0].ChangeTime);
        Assert.Equal("Success", result.ErrorMsg);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WithMultipleLargeNumbers_ReturnsCorrectGroups()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new MatchResultChangeResponse
        {
            Results = new List<List<MatchResult>>
            {
                new List<MatchResult> { new MatchResult { MatchId = 999999, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 1000000, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } },
                new List<MatchResult> { new MatchResult { MatchId = 1000001, ChangeTime = DateTimeOffset.Parse("2025-09-19 02:50:52.390") } }
            },
            ErrorMsg = "Success"
        };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal(3, result.Results.Count); // 3 groups
        Assert.Single(result.Results[0]); // Group 1 has 1 item
        Assert.Single(result.Results[1]); // Group 2 has 1 item
        Assert.Single(result.Results[2]); // Group 3 has 1 item
        Assert.Equal(999999, result.Results[0][0].MatchId);
        Assert.Equal(1000000, result.Results[1][0].MatchId);
        Assert.Equal(1000001, result.Results[2][0].MatchId);
        Assert.Equal("Success", result.ErrorMsg);
    }
}