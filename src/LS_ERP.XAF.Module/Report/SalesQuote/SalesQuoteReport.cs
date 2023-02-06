using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class SalesQuoteReport
    {
        public DateTime? CostingDate { get; set; }
        public string Season { get; set; }
        public string CustomerStyle { get; set; }
        public string Gender { get; set; }
        public string SizeRun { get; set; }
        public string Item { get; set; }
        public string FactoryCode { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string PrepareBy { get; set; }
        public string ApprovedBy { get; set; }
        public string TargetFOBPrice { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        public string CurrencyExchangeTypeID { get; set; }
        public string PriceTermCode { get; set; }
        public string SalesQuoteStatusCode { get; set; }
        public decimal? ExchangeValue { get; set; }
        public decimal? Labour { get; set; }
        public decimal? Profit { get; set; }
        public decimal? TestingFee { get; set; }
        public decimal? CMTPrice { get; set; }
        public decimal? Discount { get; set; }
    }
}
