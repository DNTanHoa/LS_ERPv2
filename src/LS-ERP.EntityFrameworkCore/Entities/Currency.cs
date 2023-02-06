using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Currency : Audit
    {
        public Currency()
        {
            ID = string.Empty;
        }
        public string ID { get; set; }
        public string Description { get; set; }
        public int? Rounding { get; set; }
    }
}
