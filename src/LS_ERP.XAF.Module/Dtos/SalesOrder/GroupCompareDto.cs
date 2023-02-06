using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos.SalesOrder
{
    [DomainComponent]
    public class GroupCompareDto
    {
        public List<SalesOrderCompareDto> CompareItemStyles { get; set; }
        public string SalesOrders { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
