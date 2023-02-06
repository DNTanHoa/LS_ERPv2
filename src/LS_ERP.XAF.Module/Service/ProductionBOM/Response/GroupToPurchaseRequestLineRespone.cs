using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GroupToPurchaseRequestLineRespone : CommonRespone
    {
        public GroupToPurchaseRequestLineResponeData Data { get; set; }

        public new GroupToPurchaseRequestLineRespone SetResult(bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }

    public class GroupToPurchaseRequestLineResponeData
    {
        public List<PurchaseRequestLine> PurchaseRequestLines { get; set; }
        public List<PurchaseRequestGroupLine> PurchaseRequestGroupLines { get; set; }
    }
}
