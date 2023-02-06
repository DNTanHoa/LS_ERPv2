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
    public class FabricPurchaseOrderSqlServerConfiguration : FabricPurchaseOrderConfiguration
    {
        public override void Configure(EntityTypeBuilder<FabricPurchaseOrder> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID)
               .UseIdentityColumn(1, 1);
            builder.Property(x => x.OrderedQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.ShippedQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.ReceivedQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.DeliveredQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.OnHandQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.IssuedQuantity)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
