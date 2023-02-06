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
    public class MaterialRequestDetailConfiguration
        : IEntityTypeConfiguration<MaterialRequestDetail>
    {
        public void Configure(EntityTypeBuilder<MaterialRequestDetail> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn();

            builder.Property(x => x.Quantity)
                .HasPrecision(19,9);
            builder.Property(x => x.RequiredQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.QuantityPerUnit)
                .HasPrecision(19, 9);
            builder.Property(x => x.Roll)
                .HasPrecision(19, 9);
        }
    }
}
