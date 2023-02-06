using LS_ERP.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Configurations
{
    public class PurchaseOrderLineConfiguration
        : IEntityTypeConfiguration<PurchaseOrderLine>
    {
        public virtual void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.WareHouseQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.WastageQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.Price)
               .HasPrecision(19, 9);
            builder.Property(x => x.Quantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.QuantityPerUnit)
               .HasPrecision(19, 9);
            builder.Property(x => x.CanReusedQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.ReservedForecastQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.ReservedQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.FreePercent)
               .HasPrecision(19, 9);
            builder.Property(x => x.FreePercentQuantity)
              .HasPrecision(19, 9);
            builder.Property(x => x.MSRP)
              .HasPrecision(19, 9);
        }
    }
}
