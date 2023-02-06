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
    public class ItemStyleSyncMasterConfiguration 
        : IEntityTypeConfiguration<ItemStyleSyncMaster>
    {
        public void Configure(EntityTypeBuilder<ItemStyleSyncMaster> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.TotalQuantity).HasPrecision(19,9);
            builder.Property(x => x.OldTotalQuantity).HasPrecision(19,9);
        }
    }
}
