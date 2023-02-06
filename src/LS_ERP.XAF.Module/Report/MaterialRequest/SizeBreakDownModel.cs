using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class SizeBreakDownModel
    {
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Description { get; set; }
        public string Season { get; set; }
        public string GarmentSize { get; set; }
        public int Quantity { get; set; }
        public int RequestQuantity { get; set; }
        public int PercentQuantity { get; set; }
        public int SampleQuantity { get; set; }
        public int OrderQuantity { get; set; }
    }
}
