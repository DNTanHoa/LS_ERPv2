using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class CreatePurchaseOrderRequest
    {
        public string ID { get; set; }
        public string Number { get; set; }

        /// <summary>
        /// Timming information
        /// </summary>
        public DateTime? OrderDate { get; set; }
        public DateTime? EstimateShipDate { get; set; }
        public DateTime? VendorConfirmedDate { get; set; }
        public DateTime? ShipDate { get; set; }

        /// <summary>
        /// Header information
        /// </summary>
        public string CustomerID { get; set; }
        public string InvoiceNo { get; set; }
        public string Reason { get; set; }
        public string PaymentTermCode { get; set; }
        public string PurchaseOrderStatusCode { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string CompanyCode { get; set; }
        public string IncoTermCode { get; set; }

        /// <summary>
        /// Supplier information
        /// Note: In the supply chain at our site vendor will be called supplier
        /// But in our customer site our supplier wil be called vendor
        /// </summary>
        public string VendorID { get; set; }
        public string SupplierCNUFCode { get; set; }


        /// <summary>
        /// Amount information
        /// </summary>
        public string CurrencyExchangeTypeID { get; set; }
        public string CurrencyID { get; set; }
        public decimal? CurrencyExchangeValue { get; set; }
        public decimal? Discount { get; set; }
        public bool? IsIncludedTax { get; set; }
        public string TaxCode { get; set; }
        public bool? Percentage { get; set; }


        /// <summary>
        /// Shipping information
        /// </summary>
        public string ShipTo { get; set; }
        public string ShippingMethodCode { get; set; }
        public string ShippingTermCode { get; set; }

        public string UserName { get; set; }

        public List<PurchaseOrderGroupLineDto> PurchaseOrderGroupLines { get; set; }
    }
}
