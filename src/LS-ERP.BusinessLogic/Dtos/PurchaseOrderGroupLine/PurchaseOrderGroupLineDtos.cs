using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class PurchaseOrderGroupLineDtos
    {
        public long ID { get; set; }

        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }

        public string PurchaseUnitID { get; set; }
        public decimal? PurchaseQuantity { get; set; }

        public string EntryUnitID { get; set; }
        public decimal? EntryQuantity { get; set; }

        public string BinCode { get; set; }
        public string LotNumber { get; set; }

        public string Remark { get; set; }
    }
}
