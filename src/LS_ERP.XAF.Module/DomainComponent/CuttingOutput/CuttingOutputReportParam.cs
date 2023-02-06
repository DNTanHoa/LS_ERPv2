using DevExpress.ExpressApp.DC;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class CuttingOutputReportParam
    {
        public string CustomerID { get; set; } = string.Empty;  
        public string CustomerName { get; set; } = string.Empty;
        public string Set { get; set; } = string.Empty;
        public DateTime PSDD { get; set; }
        public string LSStyle { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public decimal? OrderQuantity { get; set; }
        public decimal? Cutting { get; set; }
        public decimal? CuttingType { get; set; }
        public decimal? Compling { get; set; }
        public decimal? SupperMarket { get; set; }
        public decimal? SM_OnHand { get; set; }
        public decimal? Sewing { get; set; }

    }
}
