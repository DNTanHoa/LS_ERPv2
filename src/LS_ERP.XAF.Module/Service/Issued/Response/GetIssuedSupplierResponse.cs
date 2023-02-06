using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GetIssuedSupplierResponse : CommonRespone
    {
        public new GetIssuedSupplierResponse SetResult(bool result, string message)
        {
            this.Result = new CommonResult(result, message);
            return this;
        }
        public IssuedSupplierDto Data { get; set; }
    }
}
