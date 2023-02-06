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
    public class MaterialRequestConfiguration
        : IEntityTypeConfiguration<MaterialRequest>
    {
        public void Configure(EntityTypeBuilder<MaterialRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .UseIdentityColumn();
        }
    }
}
