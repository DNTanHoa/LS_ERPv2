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
    public class CuttingOutputConfiguration : IEntityTypeConfiguration<CuttingOutput>
    {
        public virtual void Configure(EntityTypeBuilder<CuttingOutput> builder)
        {
            builder.HasKey(x => x.ID);            
         
        }
    }
}
