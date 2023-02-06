using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class StorageDetailImportParam
    {
        public string UserName { get; set; }
        public Storage Storage { get; set; }
        public Customer Customer { get; set; }
        public string FilePath { get; set; }
    }
}
