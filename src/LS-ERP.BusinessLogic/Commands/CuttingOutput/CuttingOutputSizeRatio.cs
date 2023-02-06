using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class CuttingOutputSizeRatio
    {
        public string Size { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? Quantity { get; set; }
    }
}
