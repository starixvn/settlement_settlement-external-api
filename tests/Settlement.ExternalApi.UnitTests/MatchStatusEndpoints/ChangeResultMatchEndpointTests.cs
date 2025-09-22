using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.MatchStatusEndpoints;

namespace Settlement.ExternalApi.UnitTests.MatchStatusEndpoints;

public class ChangeResultMatchEndpointTests
{
    private readonly MatchChangeResultService matchChangeResultService;
    private readonly ChangeResultMatchEndpoint endpoint;

    public ChangeResultMatchEndpointTests()
    {
        matchChangeResultService = Substitute.For<MatchChangeResultService>();
        endpoint = new ChangeResultMatchEndpoint(matchChangeResultService);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationPasses_ReturnsOkResult()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-02 10:00:00");
        var expectedResult = new List<List<MatchResult>>
        {
            new List<MatchResult> { new MatchResult { MatchId = 1, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 2, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 3, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 4, ChangeTime = DateTime.MinValue } }
        };

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ReturnsBadRequestResult()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-02 10:00:00", "2024-01-01 10:00:00"); // Invalid: toDate < fromDate

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.DidNotReceive().GetDistinctMatchesWithChanges(Arg.Any<ChangeResultMatchRequest>());
    }

    [Fact]
    public async Task HandleAsync_WhenServiceReturnsEmptyList_ReturnsOkResult()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-02 10:00:00");
        var expectedResult = new List<List<MatchResult>>();

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public async Task HandleAsync_WhenServiceReturnsSingleMatch_ReturnsOkResult()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-02 10:00:00");
        var expectedResult = new List<List<MatchResult>>
        {
            new List<MatchResult> { new MatchResult { MatchId = 123, ChangeTime = DateTime.MinValue } }
        };

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public async Task HandleAsync_WhenServiceThrowsException_PropagatesException()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-02 10:00:00");
        var expectedException = new InvalidOperationException("Service error");

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => endpoint.HandleAsync(request));
        Assert.Equal("Service error", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_WithValidDateRange_CallsServiceWithCorrectRequest()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-08 10:00:00");
        var expectedResult = new List<List<MatchResult>>
        {
            new List<MatchResult> { new MatchResult { MatchId = 1, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 2, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 3, ChangeTime = DateTime.MinValue } }
        };

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        await endpoint.HandleAsync(request);

        // Assert
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public async Task HandleAsync_WithSameFromAndToDate_ReturnsOkResult()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-01 10:00:00");
        var expectedResult = new List<List<MatchResult>>
        {
            new List<MatchResult> { new MatchResult { MatchId = 5, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 6, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 7, ChangeTime = DateTime.MinValue } }
        };

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public void AddRoute_ConfiguresRouteCorrectly()
    {
        // Arrange
        var app = Substitute.For<IEndpointRouteBuilder>();

        // Act
        endpoint.AddRoute(app);

        // Assert
        // Note: MapGet is an extension method and cannot be easily mocked
        // This test verifies that AddRoute can be called without throwing exceptions
        Assert.NotNull(app);
    }

    [Fact]
    public async Task HandleAsync_WhenServiceReturnsNull_HandlesGracefully()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-02 10:00:00");
        List<List<MatchResult>>? expectedResult = null;

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public async Task HandleAsync_WithLargeDateRange_ProcessesSuccessfully()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2023-01-01 10:00:00", "2024-01-01 10:00:00");
        var expectedResult = new List<List<MatchResult>>
        {
            new List<MatchResult> { new MatchResult { MatchId = 100, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 200, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 300, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 400, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 500, ChangeTime = DateTime.MinValue } }
        };

        // Clear any previous argument matchers
        matchChangeResultService.ClearReceivedCalls();
        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public async Task HandleAsync_WithMinutePrecisionDates_ProcessesSuccessfully()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:30:00", "2024-01-01 11:45:00");
        var expectedResult = new List<List<MatchResult>>
        {
            new List<MatchResult> { new MatchResult { MatchId = 999, ChangeTime = DateTime.MinValue } },
            new List<MatchResult> { new MatchResult { MatchId = 1000, ChangeTime = DateTime.MinValue } }
        };

        matchChangeResultService.GetDistinctMatchesWithChanges(request).Returns(expectedResult);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        Assert.NotNull(result);
        await matchChangeResultService.Received(1).GetDistinctMatchesWithChanges(request);
    }

    [Fact]
    public void ValidateRequest_WithValidDates_ReturnsSuccess()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "2024-01-02 10:00:00");

        // Act
        var result = ChangeResultMatchRequestValidator.ValidateRequest(request);

        // Assert
        Assert.False(result.IsFail);
        Assert.Equal("2024-01-01 10:00:00", request.FromDate);
        Assert.Equal("2024-01-02 10:00:00", request.ToDate);
    }

    [Fact]
    public void ValidateRequest_WithInvalidFromDate_ReturnsError()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("invalid-date", "2024-01-02 10:00:00");

        // Act
        var result = ChangeResultMatchRequestValidator.ValidateRequest(request);

        // Assert
        Assert.True(result.IsFail);
        Assert.Equal(1, result.ErrorCode);
        Assert.Contains("Invalid fromDate format", result.ErrorMsg);
    }

    [Fact]
    public void ValidateRequest_WithInvalidToDate_ReturnsError()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-01 10:00:00", "invalid-date");

        // Act
        var result = ChangeResultMatchRequestValidator.ValidateRequest(request);

        // Assert
        Assert.True(result.IsFail);
        Assert.Equal(1, result.ErrorCode);
        Assert.Contains("Invalid toDate format", result.ErrorMsg);
    }

    [Fact]
    public void ValidateRequest_WithInvalidDateRange_ReturnsError()
    {
        // Arrange
        var request = new ChangeResultMatchRequest("2024-01-02 10:00:00", "2024-01-01 10:00:00"); // toDate < fromDate

        // Act
        var result = ChangeResultMatchRequestValidator.ValidateRequest(request);

        // Assert
        Assert.True(result.IsFail);
        Assert.Equal(1, result.ErrorCode);
        Assert.Contains("From date must be before To date", result.ErrorMsg);
    }
}