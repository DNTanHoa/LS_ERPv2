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
    public class JobPriceConfiguration : IEntityTypeConfiguration<JobPrice>
    {
        public virtual void Configure(EntityTypeBuilder<JobPrice> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.CompanyID).IsRequired();
            builder.Property(x => x.Price).HasPrecision(19, 9);
        }
    }
}
