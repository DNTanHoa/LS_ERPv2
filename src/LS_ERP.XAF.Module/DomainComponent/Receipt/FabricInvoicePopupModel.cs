using DevExpress.ExpressApp.DC;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class FabricInvoicePopupModel
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public DateTime? ArrivedDate { get; set; }
    }
}
