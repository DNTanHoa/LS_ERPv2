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
    public class WeekConfiguration
        : IEntityTypeConfiguration<Week>
    {
        public void Configure(EntityTypeBuilder<Week> builder)
        {
            builder.HasKey(x => x.ID);
        }
    }
}
