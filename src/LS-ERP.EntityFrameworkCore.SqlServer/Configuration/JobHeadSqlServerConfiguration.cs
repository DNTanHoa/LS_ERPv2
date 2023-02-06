using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Configuration
{
    public class JobHeadSqlServerConfiguration
        : JobHeadConfiguration
    {
        public override void Configure(EntityTypeBuilder<JobHead> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ProductionQuantity)
                .HasColumnType("DECIMAL(19,9)");
        }
    }
}
