using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class PurchaseForSalesOrderDto
    {
        public string SalesOrderID { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string SalesOrderMS { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public string CustomerStyle { get; set; } = string.Empty;
        public string LSStyle { get; set; } = string.Empty;
        public string PurchaseOrderID { get; set; } = string.Empty;
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        public DateTime? PurchaseOrderDate { get; set; }
        public string VenderID { get; set; }
        public string PurchaseOrderMS { get; set; } = string.Empty;
    }
}
