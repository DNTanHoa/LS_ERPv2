using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Vendor : Audit
    {
        public Vendor()
        {
            ID = Nanoid.Nanoid.Generate("1234567890ABCDEFGHJKLMNOPQRSTUVWXZY", 10);
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string CurrencyID { get; set; }
        public string TaxCode { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Tax Tax { get; set; }
    }
}
