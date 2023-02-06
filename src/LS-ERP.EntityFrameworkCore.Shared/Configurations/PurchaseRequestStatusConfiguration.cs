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
    public class PurchaseRequestStatusConfiguration
        : IEntityTypeConfiguration<PurchaseRequestStatus>
    {
        public void Configure(EntityTypeBuilder<PurchaseRequestStatus> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasData(new PurchaseRequestStatus() 
            {
                ID = "N",
                Name = "New request",
                CanEdit = true
            });

            builder.HasData(new PurchaseRequestStatus()
            {
                ID = "S",
                Name = "Submitted",
                CanEdit = false
            });

            builder.HasData(new PurchaseRequestStatus()
            {
                ID = "A",
                Name = "Approved",
                CanEdit = false
            });

            builder.HasData(new PurchaseRequestStatus()
            {
                ID = "R",
                Name = "Rejected",
                CanEdit = true,
            });
            
            builder.HasData(new PurchaseRequestStatus()
            {
                ID = "F",
                Name = "Finished",
                CanEdit = false
            });
            
            builder.HasData(new PurchaseRequestStatus()
            {
                ID = "C",
                Name = "Closed",
                CanEdit = false
            });
        }
    }
}
