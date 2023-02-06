using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ForecastEntry : Audit
    {
        public long ID { get; set; }
        public string ForecastGroupID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string WeekID { get; set; }
        public DateTime? EntryDate { get; set; }
        public bool IsDeActive { get; set; }
        public virtual ForecastGroup ForecastGroup { get; set; }
        public virtual Week Week { get; set; }

        public virtual IList<ForecastOverall> ForecastOveralls { get; set; }
    }
}
