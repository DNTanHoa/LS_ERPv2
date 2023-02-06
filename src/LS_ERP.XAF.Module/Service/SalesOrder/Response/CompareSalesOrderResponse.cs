using LS_ERP.XAF.Module.Dtos.SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.SalesOrder.Response
{
    public class CompareSalesOrderResponse : CommonRespone
    {
        public string CustomerID { get; set; }
        public string BrandCode { get; set; }
        public string Style { get; set; }
        public bool? IsUpdate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string FilePath { get; set; }

        public string UserName { get; set; }
        public GroupCompareDto Data { get; set; }

        public new CompareSalesOrderResponse SetResult(bool isSucess, string message)
        {
            this.Result = new Common.CommonResult(isSucess, message);
            return this;
        }
    }
}
