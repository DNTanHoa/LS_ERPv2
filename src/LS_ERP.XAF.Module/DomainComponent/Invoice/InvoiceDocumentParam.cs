using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class InvoiceDocumentParam
    {
        public InvoiceDocumentType InvoiceDocumentType { get; set; }
        [XafDisplayName("File")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }
    }
}
