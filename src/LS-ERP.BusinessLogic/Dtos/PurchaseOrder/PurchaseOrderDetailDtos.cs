﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class PurchaseOrderDetailDtos
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
        /// Shipping information
        /// </summary>
        public string ShipTo { get; set; }
        public string ShippingMethodCode { get; set; }
        public string ShippingTermCode { get; set; }

        public IList<PurchaseOrderGroupLineDtos> GroupLines { get; set; }
    }
}