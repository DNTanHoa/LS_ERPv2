using DevExpress.ExpressApp.DC;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ExportOnhandTransactionByLocationParam
    {
               
        public List<StorageBinCodeModel> StorageBinCodes { get; set; } = new List<StorageBinCodeModel>();
    }
}
