using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class XAFDailyTargetDetailSummaryDtos
    {
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string CustomerID { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }
        public string GarmentSize { get; set; }
        public int OrderQuantity { get; set; }
        public string Operation { get; set; }
        public int OutputQuantity { get; set; }
    }
}
