using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.MatchDrawScoreEndpoints;

namespace Settlement.ExternalApi.UnitTests.MatchDrawScoreEndpoints;

public class CreateGoalRecordEndpointTests
{
    private readonly SoccerWayResultService soccerWayService;

    private readonly ValidationRequestService validationService;

    private readonly CreateGoalRecordEndpoint endpoint;

    public CreateGoalRecordEndpointTests()
    {
        soccerWayService = Substitute.For<SoccerWayResultService>();
        validationService = Substitute.For<ValidationRequestService>();
        endpoint = new CreateGoalRecordEndpoint(soccerWayService, validationService);
    }

    [Fact]
    public void AddRoute_ConfiguresRouteCorrectly()
    {
        // Arrange
        var app = Substitute.For<IEndpointRouteBuilder>();

        // Act
        endpoint.AddRoute(app);

        // Assert
        app.Received().MapPost("api/match/goalRecord",
            async (GoalRecordRequest request) => await endpoint.HandleAsync(request));
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_ReturnsOkResult()
    {
        var request = new GoalRecordRequest();
        var validationResult = new Result();

        validationService.ValidGoalRecordRequest(Arg.Any<GoalRecordRequest>()).Returns(validationResult);
        soccerWayService.Insert(request).Returns(new Result());

        var result = await endpoint.HandleAsync(request);
        var okResult = (Ok<Result>)result;

        Assert.IsType<Result>(okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Success", okResult.Value.ErrorMsg);
    }

    [Fact]
    public async Task HandleAsync_InvalidRequest_ReturnsBadRequest()
    {
        var request = new GoalRecordRequest();
        var validationResult = new Result
        {
            ErrorCode = 1,
            ErrorMsg = "Error"
        };

        validationService.ValidGoalRecordRequest(Arg.Any<GoalRecordRequest>()).Returns(validationResult);

        var result = await endpoint.HandleAsync(request);
        var badRequest = (BadRequest<Result>)result;

        Assert.IsType<Result>(badRequest?.Value);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Equal("Error", badRequest.Value.ErrorMsg);
    }
}