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
    public class FinishGoodTransactionConfiguration : IEntityTypeConfiguration<FinishGoodTransaction>
    {
        public virtual void Configure(EntityTypeBuilder<FinishGoodTransaction> builder)
        {
            builder.HasKey(x => x.ID);
        }
    }
}
