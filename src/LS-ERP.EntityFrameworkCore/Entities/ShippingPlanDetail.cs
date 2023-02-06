using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ShippingPlanDetail
    {
        public int ID { get; set; }
        public int ShippingPlanID { get; set; }
        public DateTime? ShipDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentSize { get; set; }
        public string LSStyle { get; set; }
        public string CustomerStyle { get; set; }
        public int PCS { get; set; }
        public int CTN { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal Volume { get; set; }
        public decimal Dept { get; set; }
        public string Destination { get; set; }
        public string InvoiceNumber { get; set; }
        public int PackingListID { get; set; }
        public string Description { get; set; }
        public decimal PriceCM { get; set; }
        public decimal TotalPriceCM { get; set; }
        public decimal PriceFOB { get; set; }
        public decimal TotalPriceFOB { get; set; }
        public string ProductionDescription { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? EstimatedSupplierHandOver { get;set; }
        // for GA
        public string SheetName { get; set; }
        public string OrderType { get; set; }
        public string Color { get;set; }
        public decimal NetWeight { get; set; }  
        //

        /// <summary>
        /// Check PONumber & LSStyle existed
        /// </summary>
        public bool? IsError { get; set; }    
        public string FilePath { get; set; }

        /// <summary>
        /// Kích hoạt
        /// </summary>
        public bool IsDeActive { get; set; }

        /// <summary>
        /// Xác nhận
        /// </summary>
        public bool IsConfirm { get; set; }

        [JsonIgnore]
        public virtual ShippingPlan ShippingPlan { get; set; }
    }
}
