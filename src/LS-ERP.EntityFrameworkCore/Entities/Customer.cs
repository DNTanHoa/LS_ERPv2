using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Customer : Audit
    {
        public Customer()
        {
            ID = string.Empty;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PaymentTermCode { get; set; }
        public string PriceTermCode { get; set; }
        public string DivisionID { get; set; }
        public string CurrencyID { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string ContactName { get; set; }
        public long? BankAccountID { get; set; }
        public long? CountryID { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual PriceTerm PriceTerm { get; set; }
        public virtual Division Division { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual Country Country { get; set; }

        public virtual IList<Brand> Brands { get; set; }
        public virtual IList<Size> Sizes { get; set; }
    }
}
