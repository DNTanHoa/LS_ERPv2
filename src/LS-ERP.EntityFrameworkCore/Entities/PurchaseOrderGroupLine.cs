using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PurchaseOrderGroupLine : Audit
    {
        public PurchaseOrderGroupLine()
        {
            this.PurchaseOrderLines = new List<PurchaseOrderLine>();
        }

        public long ID { get; set; }
        public string PurchaseOrderID { get; set; }

        /// <summary>
        /// For manage item
        /// </summary>
        public string ItemMasterID { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Mfg { get; set; }
        public string UPC { get; set; }
        public string Division { get; set; }
        public string Label { get; set; }
        public string Specify { get; set; }
        public string Remarks { get; set; }
        public string ContractNo { get; set; }
        public DateTime? ShipDate { get; set; }
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }
        public string OrderNo { get; set; } // for puma


        /// <summary>
        /// Price and unit
        /// </summary>
        public decimal? Price { get; set; }
        public string UnitID { get; set; }
        public string WareHouseUnitID { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? WareHouseQuantity { get; set; }
        public decimal? ReceiptQuantity { get; set; }
        public decimal? WastageQuantity { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerPurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Unit Unit { get; set; }

        public virtual List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    }
}
