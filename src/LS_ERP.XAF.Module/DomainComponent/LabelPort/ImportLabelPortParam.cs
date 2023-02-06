using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ImportLabelPortParam
    {
        public Customer Customer { get; set; }
        public string FilePath { get; set; }
    }
}
