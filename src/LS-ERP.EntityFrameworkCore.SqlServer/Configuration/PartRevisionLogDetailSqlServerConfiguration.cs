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
    public class PartRevisionLogDetailSqlServerConfiguration
        : PartRevisionLogDetailConfiguration
    {
        public override void Configure(EntityTypeBuilder<PartRevisionLogDetail> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.CutWidth)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FabricWeight)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FabricWidth)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FreePercent)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.LessPercent)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.OverPercent)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Price)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.QuantityPerUnit)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.WastagePercent)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
