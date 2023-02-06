using DevExpress.ExpressApp.DC;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderCompareParam
    {
        public List<SalesOrderCompareDto> Compare { get; set; } = new List<SalesOrderCompareDto>();
        [Browsable(false)]
        public string SalesOrders { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
