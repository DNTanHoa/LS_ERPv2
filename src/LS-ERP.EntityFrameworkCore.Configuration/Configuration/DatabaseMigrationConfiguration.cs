using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Configuration
{
    public class DatabaseMigrationConfiguration
    {
		public bool ApplyDatabaseMigrations { get; set; } = false;

		public string AppDbMigrationsAssembly { get; set; }

		public void SetMigrationsAssemblies(string commonMigrationsAssembly)
		{
			AppDbMigrationsAssembly = commonMigrationsAssembly;
		}
	}
}
