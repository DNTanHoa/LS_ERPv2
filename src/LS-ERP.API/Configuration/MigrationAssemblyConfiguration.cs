using System;
using SqlMigrationAssembly = LS_ERP.EntityFrameworkCore.SqlServer.Helpers.MigrationAssembly;
using MySqlMigrationAssembly = LS_ERP.EntityFrameworkCore.MySql.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = LS_ERP.EntityFrameworkCore.PostgreSQL.Helpers.MigrationAssembly;
using System.Reflection;
using LS_ERP.EntityFrameworkCore.Configurations;

namespace LS_ERP.API.Configuration
{
    public class MigrationAssemblyConfiguration
    {
        public static string GetMigrationAssemblyByProvider(DatabaseProviderConfiguration databaseProvider)
        {
            return databaseProvider.ProviderType switch
            {
                DatabaseProviderType.SqlServer => typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
                DatabaseProviderType.PostgreSQL => typeof(PostgreSQLMigrationAssembly).GetTypeInfo()
                    .Assembly.GetName()
                    .Name,
                DatabaseProviderType.MySql => typeof(MySqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
