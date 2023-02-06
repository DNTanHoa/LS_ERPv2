using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemStyle : Audit
    {
        public ItemStyle()
        {
            Number = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLNMOPQ", 12);
        }
        /// <summary>
        /// Sale order infor
        /// </summary>
        public string SalesOrderID { get; set; }
        public string Number { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string OldLSStyle { get; set; }
        public string ItemStyleStatusCode { get; set; }
        public string UnitID { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// Style infor
        /// </summary>
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string Description { get; set; }
        public string ProductionDescription { get; set; }
        public string Brand { get; set; }
        public string FabricContent { get; set; }
        public string PIC { get; set; }
        public string UE { get; set; }
        public string PCB { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }
        public string Division { get; set; }
        public string ETAPort { get; set; }
        public string Season { get; set; }
        public string Gender { get; set; }
        public decimal? MSRP { get; set; }
        public string Image { get; set; }
        public string PriceTermCode { get; set; }
        public string CustomerCode { get; set; } // import from file PU
        public string CustomerCodeNo { get; set; } // import from file PU = customer PO of sales contract
        public string UCustomerCode { get; set; } // import from file PU
        public string UCustomerCodeNo { get; set; } // import from file PU
        /// <summary>
        /// Contract Infor
        /// </summary>
        public DateTime? ContractDate { get; set; }
        public string ContractNo { get; set; }
        public DateTime? EstimatedSupplierHandOver { get; set; }
        public string Year { get; set; }

        /// <summary>
        /// Packing Inforf
        /// </summary>
        public string Packing { get; set; }
        public string HangFlat { get; set; }

        /// <summary>
        /// Delivery Infor
        /// </summary>
        public string DeliveryPlace { get; set; }
        public string Destination { get; set; }
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Ship Infor
        /// </summary>
        public string ShipMode { get; set; }
        public string ShipTo { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? OldShipDate { get; set; }
        public string ShippingMark { get; set; }
        public string ShipColor { get; set; }
        public string ShippingStyle { get; set; }

        /// <summary>
        /// Customer purchase order infor
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public string PurchaseOrderTypeCode { get; set; }
        public int PurchaseOrderNumberIndex { get; set; }

        /// <summary>
        /// For external reference
        /// </summary>
        public string ExternalPurchaseOrderTypeName { get; set; }
        public string ExternalPurchaseOrderTypeCode { get; set; }

        public string PurchaseOrderStatusCode { get; set; }

        /// <summary>
        /// Quantity info
        /// </summary>
        public decimal? OldTotalQuantity { get; set; }
        public decimal? TotalQuantity { get; set; }

        /// <summary>
        /// Action confirm
        /// </summary>
        public bool? IsNew { get; set; }
        public bool? IsBomPulled { get; set; }
        public bool? IsCalculateRequiredQuantity { get; set; }
        public bool? IsQuantityBalanced { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? IsEnoughQuantity { get; set; }
        public bool? IsIssued { get; set; }
        public bool? MultiShip { get; set; }
        public bool? IsNotCompare { get; set; } // dùng cho compare (HA): true thì không compare, false thì compare

        /// <summary>
        /// Production infor
        /// </summary>
        public DateTime? ProductionSkedDeliveryDate { get; set; }   // PSDD

        [JsonIgnore]
        public virtual SalesOrder SalesOrder { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrderStatus PurchaseOrderStatus { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrderType PurchaseOrderType { get; set; }
        [JsonIgnore]
        public virtual ItemStyleStatus ItemStyleStatus { get; set; }
        public virtual PriceTerm PriceTerm { get; set; }

        [JsonIgnore]
        public virtual IList<ItemStyleBarCode> Barcodes { get; set; } = new List<ItemStyleBarCode>();
        [JsonIgnore]
        public virtual IList<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        [JsonIgnore]
        public virtual IList<ProductionBOM> ProductionBOMs { get; set; }
        [JsonIgnore]
        public virtual IList<PackingOverQuantity> PackingOverQuantities { get; set; }
        [JsonIgnore]
        public virtual IList<PackingList> PackingLists { get; set; }
        [JsonIgnore]
        public virtual Unit Unit { get; set; }
        [JsonIgnore]
        public virtual List<ItemStyleSyncMaster> ItemStyleSyncMasters { get; set; }
            = new List<ItemStyleSyncMaster>();
    }
}
