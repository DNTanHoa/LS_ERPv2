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
    public class QualityStatusConfiguration : IEntityTypeConfiguration<QualityStatus>
    {
        public void Configure(EntityTypeBuilder<QualityStatus> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasData(new QualityStatus()
            {
                ID = "PA",
                Name = "Passed",
                OtherName = "Đạt"
            });

            builder.HasData(new QualityStatus()
            {
                ID = "FA",
                Name = "Failed",
                OtherName = "Không đạt"
            });

            builder.HasData(new QualityStatus()
            {
                ID = "PE",
                Name = "Pending",
                OtherName = "Chờ xử lý"
            });
        }
    }
}
