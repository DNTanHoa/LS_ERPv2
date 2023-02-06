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
    public class FabricPurchaseOrderConfiguration : IEntityTypeConfiguration<FabricPurchaseOrder>
    {
        public virtual void Configure(EntityTypeBuilder<FabricPurchaseOrder> builder)
        {
            builder.HasKey(x => x.ID);
            builder.HasIndex(x => x.Number).IsUnique();

        }
    }
}
