using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class PurchaseRequestLineDto
    {
        public long ID { get; set; }
        public long PurchaseRequestGroupLineID { get; set; }
        public string PurchaseRequestID { get; set; }
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string DsmItemID { get; set; }
        public string ItemID { get; set; }
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

        /// <summary>
        /// Request quantity and unit
        /// </summary>
        public string UnitID { get; set; }
        public string PriceUnitID { get; set; }
        public string VendorID { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
