using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class StorageBinEntry : Audit
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string StorageCode { get; set; }
        public DateTime EntryDate { get; set; }
        public string PurchaseOrderNumber { get; set; } 
        public string FabricPurchaseOrderNumber { get; set; } 
        /// <summary>
        /// PO in Item Style 
        /// </summary>
        public string CustomerPurchaseOrderNumber { get; set; } 
        /// <summary>
        /// Garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string Factory { get; set; }
        public string BinCode { get; set; }

        /// <summary>
        /// Update information to storage
        /// </summary>
        public bool IsProcess { get; set; }
        public bool IsSucess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
