using Fanex.Data.Repository;
using NSubstitute;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Services;

namespace Settlement.External.UnitTests.Services;

public class SoccerWayResultServiceImpTests
{
    private readonly IDynamicRepository repository;
    private readonly SoccerWayResultServiceImp service;

    public SoccerWayResultServiceImpTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        service = new SoccerWayResultServiceImp(repository);
    }

    [Fact]
    public async Task Insert_ResultInfoIsEmpty_ShouldReturnResult()
    {
        // Act
        var result = await service.Insert(new GoalRecordRequest { MatchId = 1, ResultInfo = [] });

        // Assert
        Assert.False(result.IsFail);
        await repository.Received(4).ExecuteAsync(Arg.Any<AddSoccerWayResultCriteria>());
    }

    [Fact]
    public async Task ProcessForAutoInstant_ShouldReturnResult_WhenScoreIsProcessed()
    {
        // Arrange
        var listScore = new List<SoccerWayScore>
        {
            new() { GoalTime = 30, HomeValue = "1", AwayValue = "0" },
            new() { GoalTime = 60, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = await service.Insert(new GoalRecordRequest { MatchId = 1, ResultInfo = listScore });

        // Assert
        Assert.False(result.IsFail);
    }

    [Fact]
    public async Task AddScoreAndGoal_ShouldReturnResult_WhenScoresAreAdded()
    {
        // Arrange
        var halfTimeScore = new List<SoccerWayScore>
        {
            new() { GoalTime = 30, HomeValue = "1", AwayValue = "0" }
        };
        var fullTimeScore = new List<SoccerWayScore>
        {
            new() { GoalTime = 90, HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var result = await service.Insert(new GoalRecordRequest
        {
            MatchId = 1,
            ResultInfo = halfTimeScore.Concat(fullTimeScore).ToList()
        });

        // Assert
        Assert.False(result.IsFail);
    }
}