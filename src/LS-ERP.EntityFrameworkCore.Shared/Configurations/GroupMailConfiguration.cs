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
    public class GroupMailConfiguration : IEntityTypeConfiguration<GroupMail>
    {
        public virtual void Configure(EntityTypeBuilder<GroupMail> builder)
        {
            builder.HasKey(x => x.ID);
        }
    }
}
