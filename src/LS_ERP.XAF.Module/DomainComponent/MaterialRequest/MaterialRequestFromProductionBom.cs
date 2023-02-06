using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class MaterialRequestFromProductionBom
    {
        public string PurchaseOrderNumber { get; set; }
        public string Style { get; set; }
        public List<ItemStyle> ItemStyles { get; set; }
            = new List<ItemStyle>();
        public List<MaterialRequestDetailPreview> MaterialRequestDetailPreviews { get; set; }
             = new List<MaterialRequestDetailPreview>();
    }

    [DomainComponent]
    public class MaterialRequestDetailPreview
    {
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string DsmItemID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string LSStyle { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string MaterialTypeCode { get; set; }
        public string OtherName { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public string PerUnitID { get; set; }
        public string PriceUnitID { get; set; }
        public decimal? ConsumeQuantity { get; set; }
        public decimal? RequiredQuantity { get; set; }

        /// <summary>
        /// GroupSize
        /// </summary>
        public bool GroupSize { get; set; }
        public bool GroupItemColor { get; set; }

        public decimal? QuantityPerUnit { get; set; }
    }
}
