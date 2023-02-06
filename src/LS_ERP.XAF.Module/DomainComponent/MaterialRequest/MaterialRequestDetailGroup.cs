using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class MaterialRequestDetailEdit
    {
        public List<MaterialRequestDetailGroup> Groups { get; set; }
        = new List<MaterialRequestDetailGroup>();
    }
    [DomainComponent]
    public class MaterialRequestDetailGroup
    {
        /// <summary>
        /// Thông tin item
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// Garment infor
        /// </summary>
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }

        /// <summary>
        /// Quantity infor
        /// </summary>
        public decimal RequiredQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityPerUnit { get; set; }

        /// <summary>
        /// For only fabric
        /// </summary>
        public decimal Roll { get; set; }
        public string UnitID { get; set; }
        public string Remarks { get; set; }

        public List<MaterialRequestDetail> Details { get; set; }
        = new List<MaterialRequestDetail>();
    }
}
