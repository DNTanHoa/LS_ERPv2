using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Configuration
{
    public class UnitSqlServerConfiguration : UnitConfiguration
    {
        public override void Configure(EntityTypeBuilder<Unit> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.LastUpdatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
