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
    public class IssuedGroupLineSqlServerConfiguration
        : IssuedGroupLineConfiguration
    {
        public override void Configure(EntityTypeBuilder<IssuedGroupLine> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.Property(x => x.IssuedQuantity)
                .HasColumnType("DECIMAL(19,4)");
            builder.Property(x => x.ReceivedQuantity)
                .HasColumnType("DECIMAL(19,4)");
            builder.Property(x => x.Roll)
                .HasColumnType("DECIMAL(19,4)");
            builder.Property(x => x.RequestQuantity)
                .HasColumnType("DECIMAL(19,4)");
        }
    }
}
