using Fanex.Data.Repository;
using NSubstitute;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Requests;
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
    public async Task Insert_ShouldReturnSystemErrorResult_WhenExceptionThrown()
    {
        // Arrange
        var model = new QueryRequest
        {
            MatchId = 1,
            ResultInfo = [new Score { HomeValue = "1", AwayValue = "0", SourceId = 1, TypeId = GoalType.HalfTime }]
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
    public async Task Insert_Success_ReturnErrorMessageSuccess()
    {
        // Arrange
        var model = new QueryRequest
        {
            MatchId = 1,
            ResultInfo = [new Score { HomeValue = "1", AwayValue = "0", SourceId = 1, TypeId = GoalType.HalfTime }]
        };

        // Act
        var result = await service.Insert(model);

        // Assert
        const string expected = "Success";
        Assert.Equal(expected, result.ErrorMsg);
    }
}