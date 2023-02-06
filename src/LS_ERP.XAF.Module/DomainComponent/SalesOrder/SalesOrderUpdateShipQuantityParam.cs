using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderUpdateShipQuantityParam
    {
        public string File { get; set; }
        public Customer Customer { get; set; }
    }
}
