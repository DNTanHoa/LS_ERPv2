using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class BankAccount
    {
        public long? ID { get; set; }
        public string ShortName { get; set; }
        public string Address { get; set; }
        public string SwiftCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
    }
}
