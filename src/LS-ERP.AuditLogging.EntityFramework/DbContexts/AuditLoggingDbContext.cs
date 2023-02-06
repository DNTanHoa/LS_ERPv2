using LS_ERP.AuditLogging.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.EntityFramework.DbContexts
{
    public abstract class AuditLoggingDbContext<TAuditLog> : DbContext, IAuditLoggingDbContext<TAuditLog>
        where TAuditLog : AuditLog

    {
        protected AuditLoggingDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<TAuditLog> AuditLog { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }
    }
}
