using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class ShuModel
    {
        public string BarCode { get; set; }
        public string Size { get; set; }
        public string OrderNumber { get; set; }
        public string ItemCode { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentSize { get; set; }
        public int PCB { get; set; }
        public int UE { get; set; }
        public int TotalCarton { get; set; }
    }
}
