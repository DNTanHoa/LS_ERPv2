using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PurchaseOrder : Audit
    {
        public PurchaseOrder()
        {
            Number = "PO" + DateTime.Now.Year + "-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRST123456789", 6);
            ID = Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRST123456789", 15);
        }

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
        public string ProductionMethodCode { get; set; }

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
        public decimal? TotalAmount { get; set; }


        private Vendor vendor;
        public virtual Vendor Vendor
        {
            get => vendor;
            set
            {
                if (vendor != value)
                {
                    vendor = value;
                    this.Description = value.Description;
                }
            }
        }

        private Company company;
        public virtual Company Company
        {
            get => company;
            set
            {
                if (company != value)
                {
                    this.company = value;
                    this.ShipTo = value.DisplayAddress;
                }
            }
        }

        public virtual Customer Customer { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual CurrencyExchangeType CurrencyExchangeType { get; set; }
        public virtual Tax Tax { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual PriceTerm ProductionMethod { get; set; }
        public virtual PurchaseOrderStatus PurchaseOrderStatus { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }
        public virtual ShippingTerm ShippingTerm { get; set; }
        public virtual SupplierCNUF SupplierCNUF { get; set; }

        public virtual IncoTerm IncoTerm { get; set; }

        public virtual List<PurchaseOrderGroupLine> PurchaseOrderGroupLines { get; set; }
            = new List<PurchaseOrderGroupLine>();
        public virtual List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
            = new List<PurchaseOrderLine>();
    }
}
