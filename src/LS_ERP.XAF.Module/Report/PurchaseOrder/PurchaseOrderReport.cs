using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class PurchaseOrderReport
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyFaxNumber { get; set; }

        public string PurchaseNumber { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public string PaymentTerm { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ShippingTerm { get; set; }
        public string ShippingMethod { get; set; }
        public string CurrencyExchangeType { get; set; }
        public decimal? CurrencyExhangeValue { get; set; }
        public string Currency { get; set; }
        public int? CurrencyRounding { get; set; }
        public decimal? TotalQuantity { get; set; }
        public string Remark { get; set; }

        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }

        public string VenderName { get; set; }
        public string VenderAddress { get; set; }
        public string Description { get; set; }

        public string ShipTo { get; set; }
        public decimal? SubTotal { get; set; }
        public string SubTotalText { get; set; }
        public decimal? Vat { get; set; }
        public string VatText { get; set; }
        public decimal? TotalVat { get; set; }
        public string TotalVatText { get; set; }
        public decimal? Total { get; set; }
        public string TotalText { get; set; }
        public string MerchandiserSignature { get; set; }
        public string HeadOfDepartmentSignature { get; set; }
        public string ChiefAccountantSignature { get; set; }
        public string GeneralManagerSignature { get; set; }
        public List<PurchaseOrderReportDetail> Details { get; set; }
    }
}
