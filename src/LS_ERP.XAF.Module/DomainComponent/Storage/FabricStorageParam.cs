using DevExpress.ExpressApp.DC;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class FabricStorageParam
    {
        public List<StorageDetailsReport> Storage { get; set; }
            = new List<StorageDetailsReport>();
    }
}
