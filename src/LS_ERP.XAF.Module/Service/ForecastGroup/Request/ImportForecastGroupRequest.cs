using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class ImportForecastGroupRequest
    {
        public string CustomerID { get; set; }
        public string UserName { get; set; }
        public string ForecastGroupID { get; set; }
        public string FilePath { get; set; }
    }
}
