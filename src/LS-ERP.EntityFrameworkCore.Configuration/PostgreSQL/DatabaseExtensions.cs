using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LS_ERP.EntityFrameworkCore.Configuration.PostgreSQL
{
    public static class DatabaseExtensions
    {
        public static void RegisterPostgreSQLDbContexts<TDbContext>(this IServiceCollection services,
            ConnectionStringConfiguration connectionStrings,
            DatabaseMigrationConfiguration databaseMigrations)
            where TDbContext : DbContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for identity
            services.AddDbContext<TDbContext>(options =>
                options.UseMySql(connectionStrings.DbConnection, ServerVersion.AutoDetect(connectionStrings.DbConnection),
                sql => sql.MigrationsAssembly(databaseMigrations.AppDbMigrationsAssembly ?? migrationsAssembly)));
        }
    }
}
