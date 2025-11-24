using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
                onRetry: (exception, delay, attempt, context) =>
                {
                    Log.Error($"Migration retry {attempt} due to {exception.Message}. Waiting {delay} before next retry.");
                });
        var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);
        try
        {
            Log.Information("Checking and migrating Map database.");
            var dbContext = scope.ServiceProvider.GetRequiredService<MapDbContext>();
            await combinedPolicy.ExecuteAsync(async () => await dbContext.Database.MigrateAsync());
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while performing migration of Map database");
            throw;
        }
    }
}