using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class MapUPCResponse : CommonRespone
    {
        public List<PurchaseOrderLine> Data { get; set; }

        public new MapUPCResponse SetResult(bool isSucess, string message)
        {
            this.Result = new Common.CommonResult(isSucess, message);
            return this;
        }
    }
}
