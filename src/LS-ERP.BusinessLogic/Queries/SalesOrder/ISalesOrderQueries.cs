using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Dtos.SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries.SalesOrder
{
    public interface ISalesOrderQueries
    {
        IEnumerable<SalesOrderSummaryDtos> GetSalesOrderSummaries();
        List<PurchaseForSalesOrderDto> GetPurchaseForOrder(string CustomerID, string style, string salesOrderID);
        SalesOrderDetailDtos GetSalesOrderByID(string OrderID);
    }
}
