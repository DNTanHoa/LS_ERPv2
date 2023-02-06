using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class MapUPCRequest
    {
        public string CustomerID { get; set; }
        public List<PurchaseOrderLineDto> PurchaseOrderLines { get; set; }
    }
}
