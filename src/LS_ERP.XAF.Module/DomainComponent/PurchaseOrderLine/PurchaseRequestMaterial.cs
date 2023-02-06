using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchaseRequestMaterial
    {
        public string RequestNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Vendor Vendor { get; set; }
        public List<PurchaseRequest> Requests { get; set; }
        public List<PurchaseRequestLine> RequestMaterials { get; set; }
    }
}
