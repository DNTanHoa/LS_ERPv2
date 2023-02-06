using DevExpress.ExpressApp.DC;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PackingListPopupModel
    {
        public int ID { get; set; }
        public string LSStyles { get; set; }
        public string CustomerStyle { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ProductionDescription { get; set; }
        public string OrderType { get; set; }   
        public string Model { get; set; }
        public string Color { get; set; }
        public string SheetName { get; set; }
       // public string ColorCode { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Unit { get; set; }
        public string DeliveryPlace { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }
        public DateTime? ProductionSkedDeliveryDate { get; set; }
        public DateTime? ContractDate { get; set; }
        public DateTime? EstimatedSupplierHandOver { get; set; }
        public DateTime PPCBookDate { get; set; }
        public int Dept { get; set; }
        //for IFG
        public string Gender { get; set; }
    }
}
