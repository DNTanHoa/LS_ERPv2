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
    public class LoadingPlanSqlServerConfiguration : LoadingPlanConfiguration
    {
        public override void Configure(EntityTypeBuilder<LoadingPlan> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.PCB)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.GrossWeight)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.NetWeight)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Quantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Volumn)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
