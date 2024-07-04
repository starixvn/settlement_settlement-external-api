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
}