using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class GroupToIssuedLineResponse : CommonRespone
    {
        public GroupToPurchaseOrderLineResponeData Data { get; set; }

        public new GroupToIssuedLineResponse SetResult(bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }

    public class GroupToPurchaseOrderLineResponeData
    {
        public List<IssuedLine> IssuedLines { get; set; }
        public List<IssuedGroupLine> IssuedGroupLines { get; set; }
    }
}
