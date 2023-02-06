using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PurchaseRequestLine
    {
        public long ID { get; set; }
        public long PurchaseRequestGroupLineID { get; set; }
        public string PurchaseRequestID { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string Remarks { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string ContractNo { get; set; }
        public string Season { get; set; }

        /// <summary>
        /// Request quantity and unit
        /// </summary>
        public string UnitID { get; set; }
        public string PriceUnitID { get; set; }
        public string VendorID { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? PurchasedQuantity { get; set; }
        public decimal? RemainQuantity { get; set; }
        public decimal? Price { get; set; }

        public virtual PurchaseRequestGroupLine PurchaseRequestGroupLine { get; set; }
        public virtual PurchaseRequest PurchaseRequest { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual Unit PriceUnit { get; set; }
        public virtual Vendor Vendor { get; set; }

    }
}
