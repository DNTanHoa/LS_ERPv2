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
    public class PartMaterialConfiguration : IEntityTypeConfiguration<PartMaterial>
    {
        public virtual void Configure(EntityTypeBuilder<PartMaterial> builder)
        {
            builder.HasKey(x => x.ID);
        }
    }
}
