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
    public class ShippingPlanDetailConfiguration : IEntityTypeConfiguration<ShippingPlanDetail>
    {
        public void Configure(EntityTypeBuilder<ShippingPlanDetail> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.PriceCM)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PriceFOB)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.TotalPriceCM)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.TotalPriceFOB)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.GrossWeight)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.NetWeight)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Volume)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
