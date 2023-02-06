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
    public class IssuedTypeConfiguration : IEntityTypeConfiguration<IssuedType>
    {
        public void Configure(EntityTypeBuilder<IssuedType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasData(new IssuedType()
            {
                Id = "IS",
                Name = "Issued for production",
                OtherName = "Xuất phụ liệu/vải cho sản xuất"
            });

            builder.HasData(new IssuedType()
            {
                Id = "IR",
                Name = "Issued for return",
                OtherName = "Xuất trả hàng"
            });
        }
    }
}
