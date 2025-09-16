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
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeMatchesExist_ReturnsCommaSeparatedString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int> { 1, 2, 3, 4 };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1,2,3,4", result);
        await matchService.Received(1).GetMatchesResultChange(request);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenNoMatches_ReturnsEmptyString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int>();

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeHasMatches_ReturnsResultChangeMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int> { 4, 5, 6 };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("4,5,6", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeListIsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int>();

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeListHasDuplicates_ReturnsDistinctMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int> { 1, 1, 2, 2, 3, 3, 4 };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1,2,3,4", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenResultChangeListHasSingleMatch_ReturnsSingleMatch()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int> { 1 };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WithLargeMatchId_ReturnsCorrectResult()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int> { 1000000 };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1000000", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WithMultipleLargeNumbers_ReturnsCorrectString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var resultChangeMatches = new List<int> { 999999, 1000000, 1000001 };

        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("999999,1000000,1000001", result);
    }
}