using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ExportPurchaseOrderLinePU_Dto
    {
        public DateTime? TrxDate { get; set; }
        public DateTime? ETADate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? EstimateShipDate { get; set; }
        public DateTime? VendorConfirmDate { get; set; }
        public string PONo { get; set; }
        public string Warehouse { get; set; }
        public string Manufacturer { get; set; }
        public string MatrCode { get; set; }
        public string MatrClass { get; set; }
        public string GarmentColor { get; set; }
        public string ContractNo { get; set; }
        public string CustomerStyle { get; set; }
        public string DeliveryNote { get; set; }
        public int ItemNo { get; set; }
        public decimal? AllocQty { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? TotalReceiveQuantity { get; set; }
        public List<PurchaseOrderLineReceiveDate_Dto> ArrivedDates { get; set; } = new List<PurchaseOrderLineReceiveDate_Dto>();
    }

    public class PurchaseOrderLineReceiveDate_Dto
    {
        public DateTime? ArrivedDate { get; set; }
    }
}
