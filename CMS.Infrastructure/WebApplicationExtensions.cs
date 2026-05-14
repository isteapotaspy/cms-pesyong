using CMS.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Infrastructure;

public static class ServiceProviderExtensions
{
    public static async Task SeedDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CmsDbContext>();

        await CmsDbContextSeeder.SeedAsync(db);
    }
}