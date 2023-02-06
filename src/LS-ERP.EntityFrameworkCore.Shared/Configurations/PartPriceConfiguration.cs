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
    public class PartPriceConfiguration : IEntityTypeConfiguration<PartPrice>
    {
        public virtual void Configure(EntityTypeBuilder<PartPrice> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.StyleNO).IsRequired();
            builder.Property(x => x.Item).IsRequired();
            builder.Property(x => x.SMV).HasPrecision(19, 9);
        }
    }
}
