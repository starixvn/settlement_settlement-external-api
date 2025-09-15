using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Settlement.External.Criteria;
using Settlement.External.Services;
using Settlement.ExternalApi.InstantSettingEndpoints;
using StackExchange.Redis;
using System.Threading.Channels;
using ILogger = Fanex.Logging.ILogger;

namespace Settlement.ExternalApi.Config;

public static class InitializeConfigServices
{
    public static IServiceCollection AddInitialServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFanexSql(configuration);
        services.AddConfigRedisCache(configuration);
        services.AddFanexLogSentry(configuration);
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddTransient<ILogger, Logger>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddScoped<ProviderResultService, ProviderResultServiceImp>();
        services.AddScoped<SoccerWayResultService, SoccerWayResultServiceImp>();
        services.AddScoped<ValidationRequestService, ValidationRequestServiceImp>();
        services.AddScoped<MatchSettlementService, MatchSettlementServiceImp>();
        services.AddScoped<MatchChangeResultService, MatchChangeResultServiceImp>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IInstantSettingService, InstantSettingService>();
        services.AddSingleton(Channel.CreateUnbounded<List<MatchListItem>>());
        services.AddHostedService<ChangeMatchesSettingService>();

        return services;
    }

    private static void AddConfigRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{redisConfig}"));
        services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = redisConfig,
        }));
        services.AddSingleton<ICacheService>(new CacheService());
    }
}