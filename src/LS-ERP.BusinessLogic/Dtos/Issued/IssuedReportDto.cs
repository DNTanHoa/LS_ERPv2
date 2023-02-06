using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class IssuedReportDto
    {
        public string Number { get; set; }
        public string LSStyle { get; set; }
        public string CustomerStyle { get; set; }
        public string Season { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string CustomerID { get; set; }
        public string VendorID { get; set; }
        public string StorageCode { get; set; }
        public decimal ConsumeQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }

        public DateTime IssuedDate { get; set; }
        public string IssuedBy { get; set; }
        public string Description { get; set; }
        public string StorageBinCode { get; set; }
        public string LotNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string DyeLotNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string UnitID { get; set; }
        public decimal Roll { get; set; }



    }
}
