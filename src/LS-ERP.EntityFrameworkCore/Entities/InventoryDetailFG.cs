using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class InventoryDetailFG : Audit
    {
        public long ID { get; set; }
        public long InventoryFGID { get; set; }
        public int InventoryPeriodID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string LSStyle { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? Quantity { get; set; } /// Scan Result(+) / Shipping Plan(-) 
        public long ScanResultDetailID {get; set; }
        public long ShippingPlanDetailID { get; set; }
        public long PackingListID { get; set; }

    }
}
