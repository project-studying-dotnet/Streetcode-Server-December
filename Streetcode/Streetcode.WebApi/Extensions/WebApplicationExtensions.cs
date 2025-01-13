using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            try
            {
                var streetcodeContext = scope.ServiceProvider.GetRequiredService<StreetcodeDbContext>();
                await streetcodeContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured during startup migration");
            }
        }
    }
}
