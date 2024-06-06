using Settlement.External.Models;
using Settlement.ExternalApi.Apis;
using Settlement.ExternalApi.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSetting>(builder.Configuration);

builder.Services.AddInitialServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapMatchApi();

app.MapGet("/ok", () => "OK");

app.Run();