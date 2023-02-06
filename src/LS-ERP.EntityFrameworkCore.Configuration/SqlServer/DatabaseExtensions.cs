using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Configuration.SqlServer
{
    public static class DatabaseExtensions
    {
        public static void RegisterSqlServerDbContexts<TDbContext>(this IServiceCollection services,
            ConnectionStringConfiguration connectionStrings,
            DatabaseMigrationConfiguration databaseMigrations)
            where TDbContext : DbContext
        {
            var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseSqlServer(connectionStrings.DbConnection, sql =>
                {
                    sql.MigrationsAssembly(databaseMigrations.AppDbMigrationsAssembly ?? migrationsAssembly);
                    sql.EnableRetryOnFailure(5);
                    sql.CommandTimeout(1800);
                });
                options.LogTo(Console.WriteLine);
            });
        }
    }
}
