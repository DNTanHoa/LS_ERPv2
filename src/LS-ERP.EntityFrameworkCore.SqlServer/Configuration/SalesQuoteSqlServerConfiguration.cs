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
    public class SalesQuoteSqlServerConfiguration
        : SalesQuoteConfiguration
    {
        public override void Configure(EntityTypeBuilder<SalesQuote> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.ExchangeValue)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Labour)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.CMTPrice)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Discount)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.TestingFee)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Profit)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
