using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GetPurchaseOrderForSalesOrderResponse : CommonRespone
    {
        public List<SalesOrderPurchaseReportDetail> Data { get; set; }
    }
}
