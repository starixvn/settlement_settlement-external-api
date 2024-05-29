using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Settlement.External.Services;
using Settlement.ExternalApi.Apis;

namespace Settlement.ExternalApi.Config;

public static class InitializeConfigServices
{
    public static IServiceCollection AddInitialServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFanexSql(configuration);
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddScoped<ProviderResultService, ProviderResultServiceImp>();
        services.AddScoped<SoccerWayResultService, SoccerWayResultServiceImp>();
        services.AddScoped<ValidationRequestService, ValidationRequestServiceImp>();
        services.AddScoped<MatchServices>();

        return services;
    }
}