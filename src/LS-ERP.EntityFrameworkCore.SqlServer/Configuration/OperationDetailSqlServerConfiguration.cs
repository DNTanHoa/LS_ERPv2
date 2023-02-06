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
    public class OperationDetailSqlServerConfiguration
        : OperationDetailConfiguration
    {
        public override void Configure(EntityTypeBuilder<OperationDetail> builder)
        {
            base.Configure(builder); 
        }
    }
}
