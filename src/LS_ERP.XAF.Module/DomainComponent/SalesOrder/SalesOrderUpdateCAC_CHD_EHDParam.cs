using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderUpdateCAC_CHD_EHDParam
    {
        public string File { get; set; }
        public Customer Customer { get; set; }
    }
}
