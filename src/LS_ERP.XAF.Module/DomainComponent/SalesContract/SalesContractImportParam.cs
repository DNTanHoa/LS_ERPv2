using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesContractImportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }

        [XafDisplayName("File")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }

    }
}
