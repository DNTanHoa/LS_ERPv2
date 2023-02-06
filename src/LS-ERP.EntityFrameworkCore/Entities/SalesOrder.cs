using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesOrder : Audit
    {
        public SalesOrder()
        {
            this.Year = DateTime.Now.Year;
            this.ID = "SO" + DateTime.Now.ToString("yy") + "_";
            this.ItemStyles = new List<ItemStyle>();
        }

        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string BrandCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string DivisionID { get; set; }
        public string PaymentTermCode { get; set; }
        public string PaymentTermDescription { get; set; }
        public string PriceTermCode { get; set; }
        public string PriceTermDescription { get; set; }
        public string CurrencyID { get; set; }
        public int? Year { get; set; }
        public string SalesOrderStatusCode { get; set; }
        public string SalesOrderTypeCode { get; set; }
        public string SaveFilePath { get; set; }
        public string FileName { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Division Division { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual PriceTerm PriceTerm { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual SalesOrderStatus SalesOrderStatus { get; set; }
        public virtual SalesOrderType SalesOrderType { get; set; }
        public virtual Brand Brand { get; set; }

        public virtual IList<ItemStyle> ItemStyles { get; set; }
    }
}
