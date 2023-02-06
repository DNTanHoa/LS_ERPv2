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
    public class ReceiptTypeConfiguration : IEntityTypeConfiguration<ReceiptType>
    {
        public void Configure(EntityTypeBuilder<ReceiptType> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasData(new ReceiptType()
            {
                Id = "RC",
                Name = "Receipt for purchase",
                OtherName = "Nhập kho mua hàng"
            });

            builder.HasData(new ReceiptType()
            {
                Id = "RP",
                Name = "Receipt from customer",
                OtherName = "Nhập kho nhận hàng"
            });

            builder.HasData(new ReceiptType()
            {
                Id = "RFG",
                Name = "Receipt finish good",
                OtherName = "Nhập kho thành phẩm"
            });

            builder.HasData(new ReceiptType()
            {
                Id = "RR",
                Name = "Receipt for return",
                OtherName = "Nhập hoàn kho"
            });
        }
    }
}
