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
    public class MaterialSyncConfiguration : IEntityTypeConfiguration<MaterialSync>
    {
        public void Configure(EntityTypeBuilder<MaterialSync> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.RequiredQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.ReservedQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.RemainQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.IssuedQuantity)
                .HasPrecision(19, 9);
        }
    }
}
