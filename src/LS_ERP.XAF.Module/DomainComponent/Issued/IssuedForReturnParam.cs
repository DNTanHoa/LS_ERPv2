using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class IssuedForReturnParam
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
    }
}
