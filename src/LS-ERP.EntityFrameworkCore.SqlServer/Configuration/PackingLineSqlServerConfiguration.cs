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
    public class PackingLineSqlServerConfiguration : PackingLineConfiguration
    {
        public override void Configure(EntityTypeBuilder<PackingLine> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.QuantitySize).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.QuantityPerPackage).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PackagesPerBox).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.QuantityPerCarton).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.TotalQuantity).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.NetWeight).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.GrossWeight).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Quantity).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Length).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Width).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Height).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.InnerLength).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.InnerWidth).HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.InnerHeight).HasColumnType("DECIMAL(19,9)");
        }
    }
}
