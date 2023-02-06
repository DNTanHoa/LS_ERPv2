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
    public class PackingOutputConfiguration : IEntityTypeConfiguration<PackingOutput>
    {
        public void Configure(EntityTypeBuilder<PackingOutput> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn();
            builder.Property(x => x.Quantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.PackQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.PackPercent)
                .HasPrecision(19, 9);
        }
    }
}
