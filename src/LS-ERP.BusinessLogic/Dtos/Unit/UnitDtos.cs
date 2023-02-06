using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class  UnitDtos
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Rouding { get; set; }
        public string DecUnit { get; set; }
        public decimal Factor { get; set; }
        public bool RoundingPurchase { get; set; }
        public bool RoundUp { get; set; }
        public bool RoundDown { get; set; }
    }
}
