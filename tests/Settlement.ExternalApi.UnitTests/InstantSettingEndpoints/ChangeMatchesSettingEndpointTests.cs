using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Settlement.External.Models;
using Settlement.External.Requests;
using Settlement.External.Services;
using Settlement.ExternalApi.InstantSettingEndpoints;

namespace Settlement.ExternalApi.UnitTests.InstantSettingEndpoints;

public class ChangeMatchesSettingEndpointTests
{
    [Fact]
    public async Task HandleAsync_CallsServiceAndReturnsOkResult()
    {
        // Arrange
        var service = Substitute.For<IInstantSettingService>();
        var matchItems = new List<MatchItem> { new(1, "remark") };
        var request = new ChangeInstantSettingRequest(matchItems);
        var expectedResult = new Result();
        service.ChangeInstantSetting(request).Returns(Task.FromResult(expectedResult));
        var endpoint = new ChangeMatchesSettingEndpoint(service);

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        await service.Received(1).ChangeInstantSetting(request);
        var okResult = Assert.IsType<Ok<Result>>(result);
        Assert.Equal(expectedResult.IsFail, okResult.Value!.IsFail);
    }
}
