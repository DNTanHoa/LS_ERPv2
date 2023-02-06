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
    public class SalesOrderOffsetConfiguration : IEntityTypeConfiguration<SalesOrderOffset>
    {
        public void Configure(EntityTypeBuilder<SalesOrderOffset> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OffsetQuantity)
                .HasPrecision(19,9);
        }
    }
}
