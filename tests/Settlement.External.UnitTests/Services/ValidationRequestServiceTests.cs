using Microsoft.Extensions.Options;
using Settlement.External.Constants;
using Settlement.External.Models;
using Settlement.External.Services;

namespace Settlement.External.UnitTests.Services;

public class ValidationRequestServiceTests
{
    private readonly ValidationRequestService services;

    public ValidationRequestServiceTests()
    {
        var appSetting = new AppSetting { SecurityToken = "ABC" };
        var options = Options.Create(appSetting);

        services = new ValidationRequestServiceImp(options);
    }

    [Fact]
    public void ValidResult_CheckParameter_ReturnErrorParameterResult()
    {
        // Arrange
        var queryRequest = new QueryRequest
        {
            MatchId = 1,
            SecurityToken = "",
            ResultInfo = null
        };

        // Act
        var result = services.ValidResult(queryRequest);

        //

        Assert.Equal(ErrorCode.InvalidParameter, result.ErrorCode);
    }

    [Fact]
    public void ValidResult_CheckSecurityToken_ReturnErrorSecurityTokenResult()
    {
        // Arrange
        var queryRequest = new QueryRequest
        {
            MatchId = 1,
            SecurityToken = "XYZ",
            ResultInfo = []
        };

        // Act
        var result = services.ValidResult(queryRequest);

        //

        Assert.Equal(ErrorCode.IncorrectSecurityToken, result.ErrorCode);
    }

    [Fact]
    public void ValidResult_CheckValidScore_ReturnErrorHomeValueResult()
    {
        // Arrange
        var queryRequest = new QueryRequest
        {
            MatchId = 1,
            SecurityToken = "ABC",
            ResultInfo =
            [
                new Score()
                {
                    HomeValue = "",
                    AwayValue = "2",
                }
            ]
        };

        // Act
        var result = services.ValidResult(queryRequest);

        //

        Assert.Equal(ErrorCode.InvalidHomeValue, result.ErrorCode);
    }

    [Fact]
    public void ValidResult_CheckValidScore_ReturnErrorAwayValueResult()
    {
        // Arrange
        var queryRequest = new QueryRequest
        {
            MatchId = 1,
            SecurityToken = "ABC",
            ResultInfo =
            [
                new()
                {
                    HomeValue = "1",
                    AwayValue = "",
                }
            ]
        };

        // Act
        var result = services.ValidResult(queryRequest);

        //

        Assert.Equal(ErrorCode.InvalidAwayValue, result.ErrorCode);
    }

    [Fact]
    public void ValidResult_CheckParamters_ReturnSuccessResult()
    {
        // Arrange
        var queryRequest = new QueryRequest
        {
            MatchId = 1,
            SecurityToken = "ABC",
            ResultInfo =
            [
                new()
                {
                    HomeValue = "1",
                    AwayValue = "0",
                }
            ]
        };

        // Act
        var result = services.ValidResult(queryRequest);

        //

        Assert.Equal(0, result.ErrorCode);
    }

    [Fact]
    public void MultiValidation_InvalidMatch_ShouldReturnFirstError()
    {
        // Arrange
        var model = new ListQueryRequest
        {
            SecurityToken = "ABC",
            Matches =
            [
                new()
                {
                    MatchId = 1,
                    ResultInfo =
                    [
                        new() { HomeValue = "1", AwayValue = "1", SourceId = 1 }
                    ]
                },
                new()
                {
                    MatchId = 0, // Invalid MatchId

                    ResultInfo =
                    [
                        new() { HomeValue = "1", AwayValue = "1", SourceId = 1 }
                    ]
                }
            ]
        };

        // Act
        var result = services.MultiValidation(model);

        // Assert
        Assert.Equal(Result.ErrorParameter.ErrorCode, result.ErrorCode);
    }

    [Fact]
    public void MultiValidation_AllValidMatches_ShouldReturnSuccess()
    {
        // Arrange
        var model = new ListQueryRequest
        {
            SecurityToken = "ABC",
            Matches =
            [
                new()
                {
                    MatchId = 1,
                    ResultInfo =
                    [
                        new() { HomeValue = "1", AwayValue = "1", SourceId = 1 }
                    ]
                },
                new()
                {
                    MatchId = 2,
                    ResultInfo =
                    [
                        new() { HomeValue = "2", AwayValue = "2", SourceId = 2 }
                    ]
                }
            ]
        };

        // Act
        var result = services.MultiValidation(model);

        // Assert
        Assert.False(result.IsFail);
    }

    [Fact]
    public void ValidSettlementStatusRequest_InvalidToken_ShouldReturnErrorSecurityToken()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            SecurityToken = "",
            Type = SettlementStatusType.HoldKey
        };

        // Act
        var result = services.ValidSettlementStatusRequest(model);

        // Assert
        Assert.Equal(Result.ErrorSecurityToken.ErrorCode, result.ErrorCode);
    }

    [Fact]
    public void ValidSettlementStatusRequest_InvalidType_ShouldReturnErrorParameter()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            SecurityToken = "ABC",
            Type = 999 // Invalid type
        };

        // Act
        var result = services.ValidSettlementStatusRequest(model);

        // Assert
        Assert.Equal(Result.ErrorParameter.ErrorCode, result.ErrorCode);
    }

    [Fact]
    public void ValidSettlementStatusRequest_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            SecurityToken = "ABC",
            Type = SettlementStatusType.HoldKey
        };

        // Act
        var result = services.ValidSettlementStatusRequest(model);

        // Assert
        Assert.False(result.IsFail);
    }

    [Fact]
    public void ValidGoalRecordRequest_InvalidRequest_ShouldReturnErrorParameter()
    {
        // Arrange
        var model = new GoalRecordRequest
        {
            MatchId = 0,
            SecurityToken = "",
            ResultInfo = new List<SoccerWayScore>
            {
                new SoccerWayScore { HomeValue = "1", AwayValue = "1", SourceId = 1 }
            }
        };

        // Act
        var result = services.ValidGoalRecordRequest(model);

        // Assert
        Assert.Equal(Result.ErrorParameter.ErrorCode, result.ErrorCode);
    }

    [Fact]
    public void ValidGoalRecordRequest_ValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var model = new GoalRecordRequest
        {
            MatchId = 1,
            SecurityToken = "ABC",
            ResultInfo = new List<SoccerWayScore>
            {
                new SoccerWayScore { HomeValue = "1", AwayValue = "1", SourceId = 1 }
            }
        };

        // Act
        var result = services.ValidGoalRecordRequest(model);

        // Assert
        Assert.False(result.IsFail);
    }
}