using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Configuration.MySql
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionStrings"></param>
        /// <param name="databaseMigrations"></param>
        public static void RegisterMySqlDbContexts<TDbContext>(this IServiceCollection services,
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
