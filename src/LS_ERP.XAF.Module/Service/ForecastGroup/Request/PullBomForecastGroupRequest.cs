using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class PullBomForecastGroupRequest
    {
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public List<string> ForecastOverallIDs { get; set; }
    }
}
