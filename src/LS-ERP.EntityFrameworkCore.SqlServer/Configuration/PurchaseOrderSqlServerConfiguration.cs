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
    public class PurchaseOrderSqlServerConfiguration
        : PurchaseOrderConfiguration
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.CurrencyExchangeValue)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Discount)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
