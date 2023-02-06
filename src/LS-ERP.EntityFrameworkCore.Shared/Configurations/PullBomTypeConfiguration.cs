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
    public class PullBomTypeConfiguration : IEntityTypeConfiguration<PullBomType>
    {
        public virtual void Configure(EntityTypeBuilder<PullBomType> builder)
        {
            builder.HasKey(x => x.Code);

            builder.HasData(new PullBomType()
            {
                Code = "PA",
                Name = "Pull All",
            });
        }
    }
}
