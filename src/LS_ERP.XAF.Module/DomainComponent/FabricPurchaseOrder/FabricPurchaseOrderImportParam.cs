using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class FabricPurchaseOrderImportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }

        [ImmediatePostData(true)]
        public PriceTerm ProductionMethod { get; set; }

        [XafDisplayName("File")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }

    }
}
