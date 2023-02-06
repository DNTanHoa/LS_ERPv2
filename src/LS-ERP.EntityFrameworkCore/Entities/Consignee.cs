using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Consignee
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long? BankAccountID { get; set; }
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public long? CountryID { get; set; }
        public string CustomerID { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Country Country { get; set; }
        public virtual BankAccount BankAccount { get; set; }

    }
}
