using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Configuration;
using LS_ERP.EntityFrameworkCore.Shared.Helpers;
using System.IO;
using LS_ERP.EntityFrameworkCore.Configurations;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Autofac.Extensions.DependencyInjection;

namespace LS_ERP.API
{
    public class Program
    {
        private const string SeedArgs = "/seed";

        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration(args);

            var host = CreateHostBuilder(args).Build();
#if DEBUG
            await ApplyDbMigrationsWithDataSeedAsync(args, configuration, host);
#endif
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Limits.MaxRequestBodySize = int.MaxValue;
                    });
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task ApplyDbMigrationsWithDataSeedAsync(string[] args, IConfiguration configuration, IHost host)
        {
            var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);
            if (applyDbMigrationWithDataSeedFromProgramArguments) args = args.Except(new[] { SeedArgs }).ToArray();

            var seedConfiguration = configuration.GetSection(nameof(SeedConfiguration)).Get<SeedConfiguration>();
            var databaseMigrationsConfiguration = configuration.GetSection(nameof(DatabaseMigrationConfiguration))
                .Get<DatabaseMigrationConfiguration>();

            var databaseProvider = configuration.GetSection(nameof(DatabaseProviderConfiguration))
                .Get<DatabaseProviderConfiguration>();

            switch (databaseProvider.ProviderType)
            {
                case DatabaseProviderType.SqlServer:
                    await DbMigrationHelpers
                    .ApplyDbMigrationsWithDataSeedAsync<SqlServerAppDbContext>(host,
                        applyDbMigrationWithDataSeedFromProgramArguments, seedConfiguration, databaseMigrationsConfiguration);
                    break;
                case DatabaseProviderType.MySql:
                    await DbMigrationHelpers
                    .ApplyDbMigrationsWithDataSeedAsync<AppDbContext>(host,
                        applyDbMigrationWithDataSeedFromProgramArguments, seedConfiguration, databaseMigrationsConfiguration);
                    break;
                case DatabaseProviderType.PostgreSQL:
                    await DbMigrationHelpers
                    .ApplyDbMigrationsWithDataSeedAsync<AppDbContext>(host,
                        applyDbMigrationWithDataSeedFromProgramArguments, seedConfiguration, databaseMigrationsConfiguration);
                    break;
                default:
                    throw new Exception("Invalid provider to migrate your database");
            }
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = environment == Environments.Development;

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

            if (isDevelopment)
            {
                configurationBuilder.AddUserSecrets<Startup>();
            }

            var configuration = configurationBuilder.Build();

            configurationBuilder.AddCommandLine(args);
            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder.Build();
        }
    }
}
