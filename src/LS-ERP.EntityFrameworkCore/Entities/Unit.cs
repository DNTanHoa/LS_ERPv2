using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Unit : Audit
    {
        public Unit()
        {
            ID = string.Empty;
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public int? Rouding { get; set; }
        public string DecUnit { get; set; }
        public decimal? Factor { get; set; }
        public int? RoundingPurchase { get; set; }
        public bool? RoundUp { get; set; }
        public bool? RoundDown { get; set; }
        public string FullName { get; set; }
    }
}
