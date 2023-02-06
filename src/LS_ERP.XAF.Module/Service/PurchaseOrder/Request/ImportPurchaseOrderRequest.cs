using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportPurchaseOrderRequest
    {
        public string CustomerID { get; set; }
        public string Number { get; set; }
        public string UserName { get; set; }
        public string Season { get; set; }

        public string FilePath { get; set; }
        public TypeImportPurchaseOrder Type { get; set; }
    }
}
