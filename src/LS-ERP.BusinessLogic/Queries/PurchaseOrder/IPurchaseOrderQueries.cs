using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IPurchaseOrderQueries
    {
        IEnumerable<PurchaseOrderSummaryDtos> GetPurchaseOrders();
        IEnumerable<PurchaseOrderReportDtos> GetPurchaseOrderReport(string CustomerID, string VendorID, DateTime FromDate, DateTime ToDate);
        List<PurchaseOrderLineReceivedDtos> GetPurchaseOrderReceived(string customerID, string storageCode, DateTime? fromDate, DateTime? toDate);
        IEnumerable<PurchaseOrderInforDtos> GetPurchaseOrderInfors(string number);
        PurchaseOrderDetailDtos GetPurchaseOrder(string ID);

    }
}
