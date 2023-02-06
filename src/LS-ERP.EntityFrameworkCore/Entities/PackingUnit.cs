using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PackingUnit : Audit
    {
        public PackingUnit()
        {
            ID = string.Empty;
        }
        public string ID { get; set; }
        public string LengthUnit { get; set; }
        public string WeightUnit { get; set; }
        public string Description { get; set; }
    }
}
