using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();
    }
}
