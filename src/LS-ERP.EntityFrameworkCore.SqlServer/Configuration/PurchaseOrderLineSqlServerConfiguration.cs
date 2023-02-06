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
    public class PurchaseOrderLineSqlServerConfiguration
        : PurchaseOrderLineConfiguration
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.CanReusedQuantity)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FreePercent)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FreePercentQuantity)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.MSRP)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Price)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Quantity)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.QuantityPerUnit)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.ReservedForecastQuantity)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.ReservedQuantity)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.WareHouseQuantity)
               .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.OrderQuantityTracking)
               .HasColumnType("DECIMAL(19,9)");
        }
    }
}
