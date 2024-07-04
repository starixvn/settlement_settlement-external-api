using Fanex.Data.Repository;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Settlement.External.Constants;
using Settlement.External.Criteria;
using Settlement.External.Models;
using Settlement.External.Services;
using StackExchange.Redis;

namespace Settlement.External.UnitTests.Services;

public class MatchSettlementServiceTests
{
    private readonly IDynamicRepository repository;
    private readonly IHttpContextAccessor accessor;
    private readonly IConnectionMultiplexer connectionMultiplexer;
    private readonly MatchSettlementService service;

    public MatchSettlementServiceTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        accessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = new System.Net.IPAddress(1)
            }
        };
        accessor.HttpContext = context;
        connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        service = new MatchSettlementServiceImp(repository, accessor, connectionMultiplexer);
    }

    [Fact]
    public async Task UpdateMatchSettlementStatus_ShouldExecuteRepositoryWithCorrectCriteria()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            MatchId = 1,
            Type = SettlementStatusType.Refund
        };

        // Act
        await service.UpdateMatchSettlementStatus(model);

        // Assert
        await repository.Received(1).ExecuteAsync(Arg.Any<MatchSettlementStatusUpdCriteria>());
    }

    [Fact]
    public async Task UpdateMatchSettlementStatus_ShouldSetConfirmRefundKeyCache_WhenTypeIsRefundAndResultIsNotFail()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            MatchId = 1,
            Type = SettlementStatusType.Refund,
            IsEnabled = true
        };

        var db = Substitute.For<IDatabase>();
        connectionMultiplexer.GetDatabase(13).Returns(db);

        // Act
        await service.UpdateMatchSettlementStatus(model);

        // Assert
        db.Received(1).StringSet("SettlementConfirmRefund:1:01,00", true, TimeSpan.FromMinutes(10));
    }

    [Fact]
    public async Task UpdateMatchSettlementStatus_ShouldNotSetConfirmRefundKeyCache_WhenTypeIsNotRefund()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            MatchId = 1,
            Type = SettlementStatusType.HoldKey,
            IsEnabled = true
        };

        // Act
        await service.UpdateMatchSettlementStatus(model);

        // Assert
        connectionMultiplexer.DidNotReceive().GetDatabase(Arg.Any<int>());
    }

    [Fact]
    public async Task SetConfirmRefundKeyCache_ShouldDeleteCacheKey_WhenTypeIsRefundAndIsEnabledIsFalse()
    {
        // Arrange
        var model = new SettlementStatusUpdRequest
        {
            MatchId = 1,
            Type = SettlementStatusType.Refund,
            IsEnabled = false
        };

        var db = Substitute.For<IDatabase>();
        connectionMultiplexer.GetDatabase(13).Returns(db);

        // Act
        await service.UpdateMatchSettlementStatus(model);

        // Assert
        db.Received(1).KeyDelete("SettlementConfirmRefund:1:01,00");
    }
}