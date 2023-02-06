using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class GroupPurchaseRequestLineToPurchaseOrderLineRequest
    {
        public string PurchaseOrderID { get; set; }
        public List<PurchaseRequestLineDto> PurchaseRequestLines { get; set; }
        public List<PurchaseOrderLineDto> PurchaseOrderLines { get; set; }
        public List<PurchaseOrderGroupLineDto> PurchaseOrderGroupLines { get; set; }
    }
}
