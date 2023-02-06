using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class CurrencyExchange : Audit
    {
        public long ID { get; set; }
        public string CurrencyID { get; set; }
        public string DestinationCurrencyID { get; set; }
        public decimal Value { get; set; }
        public DateTime EffectDate { get; set; }
    }
}
