using AspireApp.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService;

public class DataSeederService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();

        await RunMigrationAsync(context, stoppingToken);
        SeedData(context);
    }

    private static async Task RunMigrationAsync(WeatherDbContext context, CancellationToken cancellationToken)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await context.Database.MigrateAsync(cancellationToken);
        });
    }

    private static void SeedData(WeatherDbContext context)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        if (!context.WeatherForecasts.Any())
        {
            foreach (var index in Enumerable.Range(1, 5))
            {
                context.WeatherForecasts
                .Add(new WeatherForecast
                {
                    Date = DateOnly
                        .FromDateTime(
                            DateTime.Now.AddDays(index)),
                    TemperatureC =
                        Random.Shared.Next(-20, 55),
                    Summary =
                        summaries[Random.Shared.Next(
                            summaries.Length)]
                });

                context.SaveChanges();
            }
        }
    }
}
