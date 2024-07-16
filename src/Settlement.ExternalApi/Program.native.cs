using Settlement.External.Models;
using Settlement.ExternalApi.Apis;
using Settlement.ExternalApi.Config;
using Settlement.ExternalApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.Configure<AppSetting>(builder.Configuration);

builder.Services.AddInitialServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapMatchApi();

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandler>();

app.UseRouting();

app.UseCors("AllowOrigin");

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();