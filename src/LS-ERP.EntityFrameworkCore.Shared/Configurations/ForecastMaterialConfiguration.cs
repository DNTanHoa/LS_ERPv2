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
    public class ForecastMaterialConfiguration
        : IEntityTypeConfiguration<ForecastMaterial>
    {
        public virtual void Configure(EntityTypeBuilder<ForecastMaterial> builder)
        {
            builder.HasKey(x => x.ID);

            builder.Property(x => x.Price)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.RequiredQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FabricWeight)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.FabricWidth)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.QuantityPerUnit)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.WastageQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.CutWidth)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.ReservedQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.WastagePercent)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.RemainQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.WareHouseQuantity)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
