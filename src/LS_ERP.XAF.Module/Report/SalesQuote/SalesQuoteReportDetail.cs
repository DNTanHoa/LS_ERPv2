using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class SalesQuoteReportDetail
    {
        public SalesQuoteReport ReportMaster { get; set; }
        public string ExternalCode { get; set; }
        public long? SaleQuoteID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string Position { get; set; }
        public decimal? Consumption { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public string UnitID { get; set; }
        public string PriceUnitID { get; set; }
        public string VendorID { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string MaterialTypeCode { get; set; }
        public int? Type { get; set; }
        public int MaterialSortIndex { get; set; }
        public string Country { get; set; }
    }
}
