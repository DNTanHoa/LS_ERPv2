using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class PurchaseOrderInforDtos
    {
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? ReceiptQuantity { get; set; }
        public decimal? RemainQuantity { get; set; }
        public string PurchaseUnitID { get; set; }
        public string EntryUnitID { get; set; }
        public int? PurchaseOrderGroupLineID { get; set; }
    }
}
