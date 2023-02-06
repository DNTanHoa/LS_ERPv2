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
    public class DailyTargetDetailConfiguration : IEntityTypeConfiguration<DailyTargetDetail>
    {
        public virtual void Configure(EntityTypeBuilder<DailyTargetDetail> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.RejectRate)
                .HasPrecision(19, 9);
            builder.Property(x => x.Offset)
                .HasPrecision(19, 9);
        }
    }
}
