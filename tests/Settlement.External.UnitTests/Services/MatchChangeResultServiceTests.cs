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
    public async Task GetDistinctMatchesWithChanges_WhenBothListsHaveMatches_ReturnsCommaSeparatedString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int> { 1, 2, 3 };
        var resultChangeMatches = new List<int> { 2, 3, 4 };

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1,2,3,4", result);
        await matchService.Received(1).GetMatchesEventStatusChange(request);
        await matchService.Received(1).GetMatchesResultChange(request);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenOnlyEventStatusHasMatches_ReturnsEventStatusMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int> { 1, 2, 3 };
        var resultChangeMatches = new List<int>();

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1,2,3", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenOnlyResultChangeHasMatches_ReturnsResultChangeMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int>();
        var resultChangeMatches = new List<int> { 4, 5, 6 };

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("4,5,6", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenBothListsAreEmpty_ReturnsEmptyString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int>();
        var resultChangeMatches = new List<int>();

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenListsHaveDuplicates_ReturnsDistinctMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int> { 1, 1, 2, 2, 3 };
        var resultChangeMatches = new List<int> { 2, 2, 3, 3, 4 };

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1,2,3,4", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WhenListsAreIdentical_ReturnsSingleList()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int> { 1, 2, 3 };
        var resultChangeMatches = new List<int> { 1, 2, 3 };

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("1,2,3", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WithSingleMatchInEachList_ReturnsBothMatches()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int> { 100 };
        var resultChangeMatches = new List<int> { 200 };

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("100,200", result);
    }

    [Fact]
    public async Task GetDistinctMatchesWithChanges_WithLargeNumbers_ReturnsCorrectString()
    {
        // Arrange
        var request = new ChangeResultMatchRequest(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var eventStatusMatches = new List<int> { 999999, 1000000 };
        var resultChangeMatches = new List<int> { 1000000, 1000001 };

        matchService.GetMatchesEventStatusChange(request).Returns(eventStatusMatches);
        matchService.GetMatchesResultChange(request).Returns(resultChangeMatches);

        // Act
        var result = await service.GetDistinctMatchesWithChanges(request);

        // Assert
        Assert.Equal("999999,1000000,1000001", result);
    }
}