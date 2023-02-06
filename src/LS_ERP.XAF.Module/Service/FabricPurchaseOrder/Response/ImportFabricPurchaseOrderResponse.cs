using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.FabricPurchaseOrder.Response
{
    public class ImportFabricPurchaseOrderResponse : CommonRespone
    {
        public List<LS_ERP.EntityFrameworkCore.Entities.FabricPurchaseOrder> Data { get; set; }

        public new ImportFabricPurchaseOrderResponse SetResult(bool isSucess, string message)
        {
            this.Result = new Common.CommonResult(isSucess, message);
            return this;
        }
    }
}
