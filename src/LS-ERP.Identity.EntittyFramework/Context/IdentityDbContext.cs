using LS_ERP.Identity.EntittyFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Identity.EntittyFramework.Context
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext() { }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options) {  }

        public virtual DbSet<User> User { get; set; }
    }
}
