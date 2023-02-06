using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Shu
    {
        public string BarCode { get; set; }
        public string OrderNumber { get; set; }
        public string ItemCode { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentSize { get; set; }
        public int PCB { get; set; }
        public int UE { get; set; }
        public string CartNo { get; set; }
    }
}
