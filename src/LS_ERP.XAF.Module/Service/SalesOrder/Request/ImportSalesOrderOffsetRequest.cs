using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportSalesOrderOffsetRequest
    {
        public string CustomerID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
