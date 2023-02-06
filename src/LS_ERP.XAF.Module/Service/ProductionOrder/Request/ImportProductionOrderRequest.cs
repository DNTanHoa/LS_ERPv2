using LS_ERP.XAF.Module.Dtos.SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportProductionOrderRequest
    {
        public string CustomerID { get; set; }
        public DateTime? StartDate { get; set; }
        public string FilePath { get; set; }
        public string UserName { get; set; }
    }
}
