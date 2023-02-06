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
    public class InvoiceDetailSqlServerConfiguration : InvoiceDetailConfiguration
    {
        public override void Configure(EntityTypeBuilder<InvoiceDetail> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ID)
                .UseIdentityColumn(1, 1);
            builder.Property(x => x.Quantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.UnitPrice)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Amount)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PriceCM)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.PriceFOB)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
