using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class PurchaseOrderReportGroupDetail
    {
        public PurchaseOrderReport PurchaseOrderReport { get; set; }
        public decimal? TotalGroupQuantity { get; set; }
        public List<PurchaseOrderReportDetail> Details { get; set; }
    }
}
