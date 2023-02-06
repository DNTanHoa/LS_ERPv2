using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class GroupPurchaseRequestLineToPurchaseOrderLineRespone
        : CommonRespone
    {
        public GroupPurchaseRequestLineToPurchaseOrderLineResponeData Data { get; set; }

        public new GroupPurchaseRequestLineToPurchaseOrderLineRespone SetResult(bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }

    public class GroupPurchaseRequestLineToPurchaseOrderLineResponeData
    {
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public List<PurchaseOrderGroupLine> PurchaseOrderGroupLines { get; set; }
    }
}
