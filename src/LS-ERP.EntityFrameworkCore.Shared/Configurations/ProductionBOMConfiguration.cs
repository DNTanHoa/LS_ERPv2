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
    public class ProductionBOMConfiguration : IEntityTypeConfiguration<ProductionBOM>
    {
        public virtual void Configure(EntityTypeBuilder<ProductionBOM> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.ConsumptionQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.FabricWeight)
               .HasPrecision(19, 9);
            builder.Property(x => x.FabricWidth)
               .HasPrecision(19, 9);
            builder.Property(x => x.FreePercent)
               .HasPrecision(19, 9);
            builder.Property(x => x.FreeQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.IssuedQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.LessPercent)
               .HasPrecision(19, 9);
            builder.Property(x => x.LessQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.Price)
               .HasPrecision(19, 9);
            builder.Property(x => x.QuantityPerUnit)
               .HasPrecision(19, 9);
            builder.Property(x => x.RemainQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.RequiredQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.ReservedQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.WareHouseQuantity)
               .HasPrecision(19, 9);
            builder.Property(x => x.WastagePercent)
               .HasPrecision(19, 9);
            builder.Property(x => x.WastageQuantity)
               .HasPrecision(19, 9);
        }
    }
}
