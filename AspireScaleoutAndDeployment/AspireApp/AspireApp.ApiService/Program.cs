using AspireApp.ApiService;
using AspireApp.ApiService.Data;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.AddSqlServerDbContext<WeatherDbContext>("sqldb");
builder.Services.AddSingleton<DataSeederService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DataSeederService>());

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/weatherforecast",
    ([FromServices] WeatherDbContext context) =>
    {
        return context.WeatherForecasts.ToArray();
    });

app.MapDefaultEndpoints();

app.Run();
