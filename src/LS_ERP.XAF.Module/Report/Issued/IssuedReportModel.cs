using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class IssuedReportModel
    {
        public string IssuedNumber { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Issuer { get; set; }
        public string Receiver { get; set; }
        public string Description { get; set; }
        public List<IssuedReportDetailModel> Details { get; set; }
    }
}
