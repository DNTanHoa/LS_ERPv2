using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class FBRequestReportDetailModel
    {
        public string No { get; set; }
        public string FabricColor { get; set; }
        public string CustomerStyleNumber { get; set; }
        public string OrderNumber { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? RequestQuantity { get; set; }
        public decimal? SemiFinishedProductQuantity { get; set; }
        public decimal? ConsumtionQuantity { get; set; }
        public decimal? StreakRequestQuantity { get; set; }
        public decimal? BalanceQuantity { get; set; }
        public decimal? CustomerConsumption { get; set; }
        public decimal? CuttingConsumption { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemMasterID { get; set; }
        public string BreadthWidth { get; set; }
        public long FabricRequestID { get; set; }
        public decimal? PercentPrint { get; set; }
        public decimal? PercentPrintQuantity { get; set; }
        public decimal? PercentWastage { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? PercentWastageQuantity { get; set; }

        public FBRequestReportModel FBRequestReportModel { get; set; } = new FBRequestReportModel();
    }
}
