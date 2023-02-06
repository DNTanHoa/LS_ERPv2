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
    public class SalesQuoteStatusConfiguration : IEntityTypeConfiguration<SalesQuoteStatus>
    {
        public void Configure(EntityTypeBuilder<SalesQuoteStatus> builder)
        {
            builder.HasKey(x => x.Code);

            builder.HasData(new SalesQuoteStatus()
            {
                Code = "N",
                Name = "Preparing",
                CanEdit = true
            });

            builder.HasData(new SalesQuoteStatus()
            {
                Code = "S",
                Name = "Submitted",
                CanEdit = false
            });

            builder.HasData(new SalesQuoteStatus()
            {
                Code = "A",
                Name = "Approved",
                CanEdit = false
            });

            builder.HasData(new SalesQuoteStatus()
            {
                Code = "R",
                Name = "Rejected",
                CanEdit = true,
            });
        }
    }
}
