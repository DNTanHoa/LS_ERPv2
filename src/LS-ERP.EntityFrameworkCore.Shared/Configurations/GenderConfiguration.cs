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
    public class GenderConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasData(new Gender()
            {
                ID = "M",
                Name = "Boy",
            });

            builder.HasData(new Gender()
            {
                ID = "F",
                Name = "Girl",
            });
        }
    }
}
