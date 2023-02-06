using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Helpers
{
    public static class DbContextHelpers
    {
        public static string GetEntityTable<TDbContext>(IServiceProvider serviceProvider, string entityTypeName = null)
            where TDbContext : DbContext
        {
            var db = serviceProvider.GetService<TDbContext>();
            if (db != null)
            {
                var entityType = entityTypeName != null ? db.Model.FindEntityType(entityTypeName) : db.Model.GetEntityTypes().FirstOrDefault();
                if (entityType != null)
                    return entityType.GetTableName();
            }
            return null;
        }
    }
}
