using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Configuration
{
    public class ConnectionStringConfiguration
    {
		public string DbConnection { get; set; }

		public void SetConnections(string commonConnectionString)
		{
			DbConnection = commonConnectionString;
		}
	}
}
