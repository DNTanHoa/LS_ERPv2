using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    [XafDisplayName("Import UPC")]
    public class UPCImportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }
        public string FilePath { get; set; }
    }
}
