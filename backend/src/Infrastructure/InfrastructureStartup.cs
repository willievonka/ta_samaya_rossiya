using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;
using Serilog;


namespace Infrastructure;

public static class InfrastructureStartup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MapDBConnection") 
                               ?? throw new NullReferenceException("Connection string 'MapDBConnection' not found.");

        services.AddDbContext<MapDbContext>(options =>
        {
            options.UseNpgsql(connectionString, postgresOptions => postgresOptions.UseNetTopologySuite());
            options.UseSnakeCaseNamingConvention();
        });
        
        return services;
    }

    public static async Task CheckAndMigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(10));
        var retryPolicy = Policy.Handle<PostgresException>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt * 2),
                onRetry: (exception, delay, attempt) =>
                {
                    Log.Error($"Migration retry {attempt} due to {exception.Message}. Waiting {delay} before next retry.");
                });
        var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);
        
        var dbContext = scope.ServiceProvider.GetRequiredService<MapDbContext>();
        
        await using var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        try
        {
            Log.Information("Checking and migrating Map database.");

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT pg_try_advisory_lock(913337);";
            var lockAcquired = (bool)(await cmd.ExecuteScalarAsync())!;

            if (!lockAcquired)
            {
                Log.Warning("Another instance is already performing migration. Skipping.");
                return;
            }

            await combinedPolicy.ExecuteAsync(async () => await dbContext.Database.MigrateAsync());

            var regionSeeder = scope.ServiceProvider.GetRequiredService<IRegionSeederService>();
            await regionSeeder.SeedIfEmptyAsync();
            
            var adminSeeder = scope.ServiceProvider.GetRequiredService<IAdminSeederService>();
            await adminSeeder.DeleteAllAndSeedAsync();
            
            var mapSeeder = scope.ServiceProvider.GetRequiredService<IMapSeederService>();
            await mapSeeder.SeedAnalyticsMapIfEmptyAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while performing migration of Map database");
            throw;
        }
    }
}