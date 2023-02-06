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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public virtual void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .HasMaxLength(50);
            builder.Property(x => x.Name)
                .HasMaxLength(500);
            builder.Property(x => x.Description)
                .HasMaxLength(500);
        }
    }
}
