using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Inventory
    {
        public int ID { get; set; }
        public string StorageCode { get; set; }
        public string CustomerID { get; set; }
        
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemMasterID { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Garment Information
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyles { get; set; }
        public string Season { get; set; }

        public decimal? OnHandQuantity { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? NotReceivedQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }

        /// <summary>
        /// Reference Purchase Order Number
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
        public string StorageBinEntry { get; set; }

        /// <summary>
        /// Group information
        /// </summary>
        public string GroupItemColor { get; set; }
        public string GroupSize { get; set; }
    }
}
