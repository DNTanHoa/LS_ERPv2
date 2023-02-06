using DevExpress.ExpressApp.DC;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SeasonReportDetail
    {
        /// <summary>
        /// Item infor
        /// </summary>
        public string ItemCode { get; set; } = string.Empty;
        public string ItemID { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemColorCode { get; set; } = string.Empty;
        public string ItemColorName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Specify { get; set; } = string.Empty;

        /// <summary>
        /// Thông tin garment
        /// </summary>
        public string CustomerStyle { get; set; } = string.Empty;
        public string LSStyle { get; set; } = string.Empty;
        public string GarmentColorCode { get; set; } = string.Empty;
        public string GarmentColorName { get; set; } = string.Empty;
        public string GarmentSize { get; set; } = string.Empty;
        public string Season { get; set; }

        public string Remark { get; set; }

        public decimal RequiredQuantity { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal ReceiptQuantity { get; set; }
        public decimal NotReceiptQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainQuantity { get; set; }
    }
}
