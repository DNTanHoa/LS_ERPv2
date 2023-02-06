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
    public class PurchaseRequestGroupLineSqlServerConfiguration
        : PurchaseRequestGroupLineConfiguration
    {
        public override void Configure(EntityTypeBuilder<PurchaseRequestGroupLine> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID)
                .UseIdentityColumn();
            
            builder.Property(x => x.Quantity)
                .HasColumnType("DECIMAL(19,9)");
            builder.Property(x => x.Price)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
