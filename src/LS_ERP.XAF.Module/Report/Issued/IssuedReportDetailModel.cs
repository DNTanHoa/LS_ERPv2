using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class IssuedReportDetailModel
    {
        public string No { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string UnitID { get; set; }
        public string DyeLot { get; set; }
        public string GarmentSize { get; set; }
        public decimal Quantity { get; set; }
        public decimal RequestQuantity { get; set; }
        public decimal SemiFinishedProductQuantity { get; set; }
        public decimal Roll { get; set; }
        public string Note { get; set; }
        public IssuedReportModel IssuedReportModel { get; set; } = new IssuedReportModel();
    }
}
