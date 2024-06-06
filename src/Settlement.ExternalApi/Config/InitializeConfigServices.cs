using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using ONEbook.UM;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Transport;
using Settlement.External.Services;
using Settlement.ExternalApi.Apis;
using SharpRaven;
using StackExchange.Redis;

namespace Settlement.ExternalApi.Config;

public static class InitializeConfigServices
{
    public static IServiceCollection AddInitialServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFanexSql(configuration);
        services.AddConfigRedisCache(configuration);
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddScoped<ProviderResultService, ProviderResultServiceImp>();
        services.AddScoped<SoccerWayResultService, SoccerWayResultServiceImp>();
        services.AddScoped<ValidationRequestService, ValidationRequestServiceImp>();
        services.AddScoped<MatchSettlementService, MatchSettlementServiceImp>();

        services.AddScoped<MatchServices>();

        return services;
    }

    private static void AddConfigRedisCache(this IServiceCollection service, IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
        var applicationName = configuration.GetSection("AuthSettings:ChannelPrefix").Value;
        service.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect($"{redisConfig}"));
        service.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = redisConfig,
            //ApplicationName = applicationName,
        }));
        service.AddSingleton<ICacheService>(new CacheService());
    }

    private static void StartUmSetting(IConfiguration configuration)
    {
        var umConfig = new UMClientConfiguration().Initialize(configuration);

        UMClientManager
            .UseConfig(umConfig)
            .UseCacheService(new UMCacheService(new CacheService()))
            .UseRestClient(new UMRestClient(umConfig));
    }

    private static void StartLogSetting(IConfiguration configuration)
    {
        var logEngine = new List<ILogEngine>();

        var sentryEngineOptions = new SentryEngineOptions
        {
            Dsn = new Dsn(configuration.GetSection("SentryToken").Value)
        };
        logEngine.Add(new SentryLogging(sentryEngineOptions));

        LogManager.SetDefaultLogCategory(configuration.GetSection("SiteCategory").Value).Use(logEngine);

        ExceptionHandler
            .UseConfig(new LoggingConfiguration())
            .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);
    }
}