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
    public class GroupMailSqlServerConfiguration : GroupMailConfiguration
    {
        public override void Configure(EntityTypeBuilder<GroupMail> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ID)
                .UseIdentityColumn();

            builder.HasData(new GroupMail()
            {
                ID = 1,
                DepartmentName = "CUTTING",
                CompanyCode = "LS",
                CustomerID = "DE",
                Mail = "cutting@leadingstar.com.vn"
            });

            builder.HasData(new GroupMail()
            {
                ID = 2,
                DepartmentName = "CUTTING",
                CompanyCode = "LK",
                CustomerID = "DE",
                Mail = "cutting@luckystargarment.com.vn"
            });

            builder.HasData(new GroupMail()
            {
                ID = 3,
                DepartmentName = "WAREHOUSE",
                CompanyCode = "LS",
                CustomerID = "DE",
                Mail = "fb@leadingstar.com.vn"
            });

            builder.HasData(new GroupMail()
            {
                ID = 4,
                DepartmentName = "WAREHOUSE",
                CompanyCode = "LK",
                CustomerID = "DE",
                Mail = "fb@leadingstar.com.vn"
            });
        }
    }
}
