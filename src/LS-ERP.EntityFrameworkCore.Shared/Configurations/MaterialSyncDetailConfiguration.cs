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
    public class MaterialSyncDetailConfiguration : IEntityTypeConfiguration<MaterialSyncDetail>
    {
        public void Configure(EntityTypeBuilder<MaterialSyncDetail> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.Quantity)
                .HasPrecision(19, 9);
        }
    }
}
