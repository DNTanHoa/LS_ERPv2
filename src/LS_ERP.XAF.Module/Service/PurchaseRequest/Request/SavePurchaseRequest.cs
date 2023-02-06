using LS_ERP.XAF.Module.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class SavePurchaseRequest
    {
        public string ID { get; set; }
        public string Number { get; set; }
        public string CustomerID { get; set; }
        public string DivisionID { get; set; }
        public string CurrencyID { get; set; }
        public string CurrencyExchangeTypeID { get; set; }
        public decimal? ExchangeValue { get; set; }
        public string Reason { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Time information
        /// </summary>
        public DateTime? RequestDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ConfirmDate { get; set; }


        public List<PurchaseRequestGroupLineDto> PurchaseRequestGroupLines { get; set; }
    }
}
