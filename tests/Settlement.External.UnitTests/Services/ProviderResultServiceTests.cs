using Fanex.Data.Repository;
using NSubstitute;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.DTOs;
using Settlement.External.Models;
using Settlement.External.Services;

namespace Settlement.External.UnitTests.Services;

public class ProviderResultServiceImpTests
{
    private readonly IDynamicRepository repository;
    private readonly ProviderResultServiceImp service;

    public ProviderResultServiceImpTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        service = new ProviderResultServiceImp(repository);
    }

    [Fact]
    public async Task GetResultByMatchId_ShouldReturnResults_WhenMatchIdExists()
    {
        // Arrange
        var matchId = 1;
        var expectedResults = new List<ScoreDto>
        {
            new() {  HomeValue = "1", AwayValue = "0" }
        };
        repository.FetchAsync<ScoreDto>(Arg.Any<GetResultByMatchIdCriteria>())
            .Returns(Task.FromResult((IEnumerable<ScoreDto>)expectedResults));

        // Act
        var result = await service.GetResultByMatchId(matchId);

        // Assert
        Assert.Equal(expectedResults.Count, result.Count);
    }

    [Fact]
    public async Task GetMatch_ShouldReturnMatchDto_WhenMatchIdExists()
    {
        // Arrange
        var matchId = 1;
        var expectedResults = new List<ScoreDto>
        {
            new() { HomeValue = "1", AwayValue = "0" }
        };
        repository.FetchAsync<ScoreDto>(Arg.Any<GetResultByMatchIdCriteria>())
            .Returns(Task.FromResult((IEnumerable<ScoreDto>)expectedResults));

        // Act
        var result = await service.GetMatch(matchId);

        // Assert
        Assert.Equal(matchId, result.MatchId);
        Assert.Equal(expectedResults.Count, result.ResultInfo.Count);
    }

    [Fact]
    public async Task Insert_ShouldReturnSystemErrorResult_WhenExceptionThrown()
    {
        // Arrange
        var model = new QueryRequest
        {
            MatchId = 1,
            ResultInfo = [new() { HomeValue = "1", AwayValue = "0", SourceId = 1, TypeId = GoalType.HalfTime }]
        };
        repository.When(x => x.ExecuteAsync(Arg.Any<AddResultCriteria>()))
            .Do(_ => throw new Exception("Database error"));

        // Act
        var result = await service.Insert(model);

        // Assert
        Assert.True(result.IsFail);
        Assert.Equal(ErrorCode.SystemError, result.ErrorCode);
        Assert.Equal("Database error", result.ErrorMsg);
    }

    [Fact]
    public async Task InsertMultiMatches_ShouldReturnFailResult_WhenAnyMatchInsertFails()
    {
        // Arrange
        var model = new ListQueryRequest
        {
            Matches = new List<Match>
            {
                new() { MatchId = 1, ResultInfo = [new() { HomeValue = "1", AwayValue = "0" }] },
                new() { MatchId = 2, ResultInfo = [new() { HomeValue = "2", AwayValue = "1" }] }
            }
        };

        // Act
        await service.InsertMultiMatches(model);

        // Assert
        await repository.Received(model.Matches.Count).ExecuteAsync(Arg.Any<AddResultCriteria>());
    }
}