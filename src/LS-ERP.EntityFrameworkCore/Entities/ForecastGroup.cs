using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ForecastGroup : Audit
    {
        public ForecastGroup()
        {
            this.ID = Nanoid.Nanoid.Generate("0123456789ABCDEFGHIJKLNMOPQRST", size: 10);
        }

        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Description { get; set; }
        public DateTime? ForecastFrom { get; set; }
        public DateTime? ForecastTo { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual IList<ForecastEntry> ForecastEntries { get; set; }
    }
}
