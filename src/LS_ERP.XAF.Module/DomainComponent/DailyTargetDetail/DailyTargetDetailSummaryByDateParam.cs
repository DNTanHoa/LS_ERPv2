using DevExpress.ExpressApp.DC;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyTargetDetailSummaryByDateParam
    {
        public int ID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int DailyTargetID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string StyleNO { get; set; } = string.Empty;
        public string WorkCenterID { get; set; } = string.Empty;
        public string WorkCenterName { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Remark { get; set; } = string.Empty;
        public decimal TotalTargetQuantity { get; set; }
        public decimal? TotalOrderQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Efficiency { get; set; }
        public decimal? SMV { get; set; }
        public int NumberOfWorker { get; set; }
        public DateTime ProduceDate { get; set; }
        public DateTime InlineDate { get; set; }
        public string Operation { get; set; } = string.Empty;
        public string LSStyle { get; set; } = string.Empty;
    }
}
