using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class LSStyleCompareDtos
    {
        public string LSStyle { get; set; } = string.Empty;
        public decimal TargetQuantity { get; set; }
        public decimal OrderQuantity { get; set; }
        public string Result { get; set; } = string.Empty;
    }
}
