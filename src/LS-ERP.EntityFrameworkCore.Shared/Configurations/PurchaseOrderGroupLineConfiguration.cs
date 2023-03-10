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
    public class PurchaseOrderGroupLineConfiguration
        : IEntityTypeConfiguration<PurchaseOrderGroupLine>
    {
        public virtual void Configure(EntityTypeBuilder<PurchaseOrderGroupLine> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.WareHouseQuantity)
                .HasPrecision(19, 9);
            builder.Property(x => x.Price)
               .HasPrecision(19, 9);
            builder.Property(x => x.Quantity)
               .HasPrecision(19, 9);
        }
    }
}
