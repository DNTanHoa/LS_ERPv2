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
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public virtual void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.TotalAmount).HasPrecision(29,9);
        }
    }
}
