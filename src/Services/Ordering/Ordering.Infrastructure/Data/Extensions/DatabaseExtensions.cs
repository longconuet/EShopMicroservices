using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Ordering.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(ApplicationDbContext).Name);

            var retry = Policy.Handle<SqlException>()
                        .WaitAndRetryAsync(
                            retryCount: 5,
                            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (exception, retryCount, context) =>
                            {
                                logger.LogError($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                            });

            await retry.ExecuteAsync(async () =>
            {
                context.Database.MigrateAsync().GetAwaiter().GetResult();
                await SeedAsync(context);
            });

            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(ApplicationDbContext).Name);
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(ApplicationDbContext).Name);
        }
    }

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedCustomerAsync(context);
        await SeedProductAsync(context);
        await SeedOrdersWithItemsAsync(context);
    }

    public static async Task SeedCustomerAsync(ApplicationDbContext context)
    {
        if (!await context.Customers.AnyAsync())
        {
            context.Customers.AddRange(InitialData.Customers);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedProductAsync(ApplicationDbContext context)
    {
        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(InitialData.Products);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedOrdersWithItemsAsync(ApplicationDbContext context)
    {
        if (!await context.Orders.AnyAsync())
        {
            context.Orders.AddRange(InitialData.Orders);
            await context.SaveChangesAsync();
        }
    }
}
