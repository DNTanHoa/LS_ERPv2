using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class InvoiceDetail : Audit
    {
        public long ID { get; set; }
        public string CategoryNo { get; set; }
        public string CustomerStyle { get; set; }
        public string CustomerPurchaseOrderNumber { get; set; }
        public string Description { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string FabricContent { get; set; }
        public decimal? Quantity { get; set; }
        public string UnitID { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? PriceCM { get; set; }
        public decimal? PriceFOB { get; set; }
        public decimal? Amount { get; set; }
        public long? InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Unit Unit { get; set; }

        public string KeyInvoice()
        {
            return this.CustomerStyle + this.CustomerPurchaseOrderNumber + this.GarmentColorCode;
        }

    }
}
