using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using MassTransit;
using Settlement.External.Consumers;
using Settlement.External.Models;
using Settlement.External.Services;
using StackExchange.Redis;
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
        services.AddRabbitMq(configuration);

        services.AddTransient<ILogger, Logger>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddScoped<ProviderResultService, ProviderResultServiceImp>();
        services.AddScoped<SoccerWayResultService, SoccerWayResultServiceImp>();
        services.AddScoped<ValidationRequestService, ValidationRequestServiceImp>();
        services.AddScoped<MatchSettlementService, MatchSettlementServiceImp>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IInstantSettingService, InstantSettingService>();

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


    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSetting = configuration.GetSection(RabbitMqSetting.SectionName).Get<RabbitMqSetting>();
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ChangeInstantSettingConsumer>();
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSetting.Host, rabbitMqSetting.VirtualHost, h =>
                {
                    h.Username(rabbitMqSetting.UserName);
                    h.Password(rabbitMqSetting.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}