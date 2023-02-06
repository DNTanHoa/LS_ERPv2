using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class JobHeadFilterRequest
    {
        public string Style { get; set; }
        public string CustomerID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
