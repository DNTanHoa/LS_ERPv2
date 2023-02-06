using LS_ERP.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Helpers
{
    public class DbMigrationHelpers
    {
        /// <summary>
        /// Generate migration before calling this method
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="host"></param>
        /// <param name="applyDbMigrationWithDataSeedFromProgramArguments"></param>
        /// <param name="seedConfiguration"></param>
        /// <param name="databaseMigrationsConfiguration"></param>
        /// <returns></returns>
        public static async Task ApplyDbMigrationsWithDataSeedAsync<TDbContext>(
            IHost host, bool applyDbMigrationWithDataSeedFromProgramArguments, SeedConfiguration seedConfiguration,
            DatabaseMigrationConfiguration databaseMigrationsConfiguration)
            where TDbContext : DbContext

        {
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                if ((databaseMigrationsConfiguration != null && databaseMigrationsConfiguration.ApplyDatabaseMigrations)
                    || (applyDbMigrationWithDataSeedFromProgramArguments))
                {
                    await EnsureDatabasesMigratedAsync<TDbContext>(services);
                }
            }
        }

        public static async Task EnsureDatabasesMigratedAsync<TDbContext>(IServiceProvider services)
            where TDbContext : DbContext
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TDbContext>())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }
    }
}
