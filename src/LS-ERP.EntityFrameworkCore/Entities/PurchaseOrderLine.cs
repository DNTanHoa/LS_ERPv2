using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PurchaseOrderLine : Audit
    {
        public PurchaseOrderLine()
        {
            ReservationEntries = new List<ReservationEntry>();
        }

        public long ID { get; set; }
        public string PurchaseOrderID { get; set; }
        /// <summary>
        /// Thông tin mua hàng forecast
        /// </summary>
        public string ForecastWeekID { get; set; }
        public string ContractualHandoverDateWeekID { get; set; }
        /// <summary>
        /// Thông tin mua hàng sale order
        /// </summary>
        public string SalesOrderID { get; set; }
        /// <summary>
        /// Thông tin mua hàng theo yêu cầu
        /// </summary>
        public string PurchaseRequestID { get; set; }
        public long? PurchaseOrderGroupLineID { get; set; }

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

        public string ItemNo { get; set; } // for PUMA
        public string MaterialTypeClass { get; set; } // for PUMA
        public string MaterialType { get; set; } // for PUMA
        public string CustomerSupplier { get; set; } // for PUMA
        public string Position { get; set; }
        public string Mfg { get; set; }
        public string UPC { get; set; }
        public string Division { get; set; }
        public string Label { get; set; }
        public string Specify { get; set; }
        public string Remarks { get; set; }
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerPurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? EstimateShipDate { get; set; } // for PUMA
        public DateTime? VendorConfirmDate { get; set; }  // for PUMA
        public DateTime? OrderDate { get; set; }  // for PUMA
        public string ContractNo { get; set; }
        public string OrderNo { get; set; } // for PUMA
        public string InvoiceNo { get; set; } // for PUMA
        public string DeliveryNote { get; set; } // for PUMA
        public decimal? OrderQuantityTracking { get; set; } // for PUMA

        /// <summary>
        /// Price and unit
        /// </summary>
        public decimal? Price { get; set; }
        public string UnitID { get; set; }
        public string SecondUnitID { get; set; }
        public decimal? WareHouseQuantity { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? ReservedQuantity { get; set; }
        public decimal? ReservedForecastQuantity { get; set; }
        /// <summary>
        /// Số lượng đã cấn trừ forecast
        /// </summary>
        public decimal? ConsumedForecastQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public decimal? FreePercent { get; set; }
        public decimal? FreePercentQuantity { get; set; }
        public decimal? CanReusedQuantity { get; set; }
        public decimal? WastageQuantity { get; set; }

        /// <summary>
        /// UPC information
        /// </summary>
        public decimal? MSRP { get; set; }
        public string SuppPlt { get; set; }
        public string ReplCode { get; set; }
        public string DeptSubFineline { get; set; }
        public string FixtureCode { get; set; }
        public string TagSticker { get; set; }
        public string ModelName { get; set; }

        public string ToDictionaryKey(string customerID)
        {
            string key = string.Empty;

            switch (customerID)
            {
                case "PU":
                    {
                        key = this.CustomerPurchaseOrderNumber + this.ItemID + this.ItemColorCode
                            + this.Specify;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }

            return key;
        }

        public string KeyMatchingShipment()
        {
            string key = string.Empty;

            key = this.CustomerPurchaseOrderNumber + this.ContractNo + this.ItemID + this.ItemColorName;
            return key;
        }

        /// <summary>
        /// Matched shipment
        /// </summary>
        /// 

        public bool? MatchingShipment { get; set; } // for puma

        [JsonIgnore]
        public virtual PurchaseOrderGroupLine PurchaseOrderGroupLine { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        [JsonIgnore]
        public virtual Unit Unit { get; set; }
        [JsonIgnore]
        public virtual Unit SecondUnit { get; set; }
        public virtual IList<ReservationEntry> ReservationEntries { get; set; }
            = new List<ReservationEntry>();
        public virtual IList<ReservationForecastEntry> ReservationForecastEntries { get; set; }
            = new List<ReservationForecastEntry>();
    }
}
