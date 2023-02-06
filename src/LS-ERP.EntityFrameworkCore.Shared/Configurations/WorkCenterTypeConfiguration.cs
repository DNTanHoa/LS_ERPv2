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
    public class WorkCenterTypeConfiguration : IEntityTypeConfiguration<WorkCenterType>
    {
        public void Configure(EntityTypeBuilder<WorkCenterType> builder)
        {
            builder.Property(x => x.ID);
        }
    }
}
