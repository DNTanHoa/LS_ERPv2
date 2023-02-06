using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ProductionOrderLine : Audit
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public string ProductionOrderID { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string ItemCode { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string ProductDescription { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// 4518690883
        /// </summary>
        public string CustomerOrderNumber { get; set; }
        /// <summary>
        /// Order type, for DE 
        /// WO: Work order
        /// DO: Delivery order
        /// WoDo: Work order and delivery order
        /// </summary>
        public string CustomerOderType { get; set; }
        public DateTime BeginProductionDate { get; set; }
        public DateTime EndProductionDate { get; set; }
        public DateTime EstimateSupplierHandOver { get; set; }

        public decimal OrderedQuantity { get; set; }
        public decimal ShippedQuantity { get; set; }
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Shipping
        /// </summary>
        public string ShipmentType { get; set; }
        public string ShipVia { get; set; }

        public int PerCartonBox { get; set; }
        public string Packing { get; set; }
    }
}
