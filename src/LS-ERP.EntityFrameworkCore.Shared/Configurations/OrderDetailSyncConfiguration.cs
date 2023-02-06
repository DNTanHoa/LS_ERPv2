using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared
{
    public class OrderDetailSyncConfiguration
        : IEntityTypeConfiguration<OrderDetailSync>
    {
        public void Configure(EntityTypeBuilder<OrderDetailSync> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.Quantity).HasPrecision(19, 9);
        }
    }
}
