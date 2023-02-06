using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Port : Audit
    {
        public long ID { get; set; }
        public string Name { get; set; }

        public long? CountryID { get; set; }

        public virtual Country Country { get; set; }
    }
}
