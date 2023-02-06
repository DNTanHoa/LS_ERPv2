using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class DeletePurchaseOrderLineRequest
    {
        public string UserName { get; set; }
        public List<long> PurchaseOrderLineIDs { get; set; }
    }
}
