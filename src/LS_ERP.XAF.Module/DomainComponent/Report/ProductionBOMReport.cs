using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ProductionBOMReport
    {
        public SalesOrder SalesOrder { get; set; }
        
        public List<ItemStyle> ItemStyles { get; set; }

        public List<ProductionBOMReportDetail> Detail { get; set; }
    }

    [DomainComponent]
    public class ProductionBOMReportDetail
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string GarmentSize { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string VenndorID { get; set; }
        
        public decimal? RequiredQuantity { get; set; }
        public decimal? ClearedQuantity { get; set; }
        public decimal? BalancedQuatity { get; set; }
        public decimal? PurchasedQuantity { get; set; }
        public decimal? ReceiptQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? InStockQuantity { get; set; }
    }
}
