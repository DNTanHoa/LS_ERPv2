using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class GetScanResultSummaryRequest
    {
        public string Company { get; set; } = string.Empty;
        public DateTime SummaryDate { get; set; }
    }
}
