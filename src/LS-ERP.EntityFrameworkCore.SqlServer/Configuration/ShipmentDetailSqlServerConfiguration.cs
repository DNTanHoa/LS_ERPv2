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
    public class ShipmentDetailSqlServerConfiguration
        : ShipmentDetailConfiguration
    {
        public override void Configure(EntityTypeBuilder<ShipmentDetail> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.ReceivedQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.AllocQuantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.AllocReceivedQuantity)
                .HasColumnType("DECIMAL(19,9)");

        }
    }
}
