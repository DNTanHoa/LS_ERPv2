using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class RequestOldPurchaseOrder
    {
        public string PurchaseOrderNumber { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    }
}
