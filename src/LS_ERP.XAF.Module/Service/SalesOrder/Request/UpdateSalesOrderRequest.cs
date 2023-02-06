using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class UpdateSalesOrderRequest
    {
        public string CustomerID { get; set; }
        public string FilePath { get; set; }
        public string UserName { get; set; }
        public string ID { get; set; }
        public string BrandCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string DivisionID { get; set; }
        public string PaymentTermCode { get; set; }
        public string PriceTermCode { get; set; }
        public string CurrencyID { get; set; }
        public int? Year { get; set; }
        public string SalesOrderStatusCode { get; set; }
        public string SalesOrderOrderTypeCode { get; set; }

    }
}
