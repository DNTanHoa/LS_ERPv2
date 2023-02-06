using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Configuration
{
    public class PurchaseOrderTypeSqlServerConfiguration
        : PurchaseOrderTypeConfiguration
    {
        public override void Configure(EntityTypeBuilder<PurchaseOrderType> builder)
        {
            base.Configure(builder);
        }
    }
}
