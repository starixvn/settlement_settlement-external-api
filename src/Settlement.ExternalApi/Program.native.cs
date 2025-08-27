using MinimalApi.Endpoint.Extensions;
using Settlement.External.Models;
using Settlement.ExternalApi.Config;
using Settlement.ExternalApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();

builder.Services.AddHealthChecks();

builder.Services.Configure<AppSetting>(builder.Configuration);

builder.Services.AddCors(builder =>
{
    builder.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddInitialServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();
    //app.MapScalarApiReference();
}

app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseCors();

app.MapEndpoints();

app.Run();