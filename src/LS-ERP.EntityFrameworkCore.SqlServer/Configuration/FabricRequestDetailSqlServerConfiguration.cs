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
    public class FabricRequestDetailSqlServerConfiguration : FabricRequestDetailConfiguration
    {
        public override void Configure(EntityTypeBuilder<FabricRequestDetail> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.OrderQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.RequestQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.ConsumtionQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.StreakRequestQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.BalanceQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.CustomerConsumption)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.CuttingConsumption)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PercentPrint)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PercentPrintQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PercentWastage)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PercentWastageQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.IssuedQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.SemiFinishedProductQuantity)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
