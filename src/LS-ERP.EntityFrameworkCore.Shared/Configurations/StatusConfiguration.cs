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
    public class StatusConfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasData(new Status()
            {
                ID = "A",
                Name = "Approved",
            });

            builder.HasData(new Status()
            {
                ID = "C",
                Name = "Cutted",
            });

            builder.HasData(new Status()
            {
                ID = "N",
                Name = "New",
            });

            builder.HasData(new Status()
            {
                ID = "S",
                Name = "Send",
            });

            builder.HasData(new Status()
            {
                ID = "R",
                Name = "Reject",
            });


        }
    }
}
