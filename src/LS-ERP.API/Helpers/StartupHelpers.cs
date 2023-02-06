using LS_ERP.EntityFrameworkCore.Configuration;
using LS_ERP.EntityFrameworkCore.Configuration.MySql;
using LS_ERP.EntityFrameworkCore.Configuration.PostgreSQL;
using LS_ERP.EntityFrameworkCore.Configuration.SqlServer;
using LS_ERP.EntityFrameworkCore.Configurations;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using SqlMigrationAssembly = LS_ERP.EntityFrameworkCore.SqlServer.Helpers.MigrationAssembly;

namespace LS_ERP.API.Helpers
{
    public static class StartupHelpers
    {
        /// <summary>
        /// Register DbContexts for IdentityServer ConfigurationStore and PersistedGrants, Identity and Logging
        /// Configure the connection strings in AppSettings.json
        /// </summary>
        /// <typeparam name="TConfigurationDbContext"></typeparam>
        /// <typeparam name="TPersistedGrantDbContext"></typeparam>
        /// <typeparam name="TLogDbContext"></typeparam>
        /// <typeparam name="TIdentityDbContext"></typeparam>
        /// <typeparam name="TAuditLoggingDbContext"></typeparam>
        /// <typeparam name="TDataProtectionDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddDbContexts<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext
        {
            var databaseProvider = configuration.GetSection(nameof(DatabaseProviderConfiguration)).Get<DatabaseProviderConfiguration>();
            var databaseMigrations = configuration.GetSection(nameof(DatabaseMigrationConfiguration)).Get<DatabaseMigrationConfiguration>() ?? 
                new DatabaseMigrationConfiguration();

            var connectionStrings = configuration.GetSection("ConnectionString").Get<ConnectionStringConfiguration>();

            switch (databaseProvider.ProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    databaseMigrations.SetMigrationsAssemblies(typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name);
                    services.RegisterSqlServerDbContexts<TDbContext>(connectionStrings, databaseMigrations);
                    services.RegisterSqlServerDbContexts<SqlServerAppDbContext>(connectionStrings, databaseMigrations);
                    break;
                case DatabaseProviderType.PostgreSQL:
                    services.RegisterPostgreSQLDbContexts<TDbContext>(connectionStrings, databaseMigrations);
                    break;
                case DatabaseProviderType.MySql:
                    services.RegisterMySqlDbContexts<TDbContext>(connectionStrings, databaseMigrations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseProvider.ProviderType), $@"The value needs to be one of {string.Join(", ", Enum.GetNames(typeof(DatabaseProviderType)))}.");
            }
        }
    }
}
