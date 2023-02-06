using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DailyTargetDetailModel
    {
        DailyTargetDetailModel()
        {
            this.IsAllocated = true;
            this.Operation = "CUTTING";
        }
        public int ID { get; set; }    
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public string Size { get; set; }
        public string Remark { get; set; }        
        public decimal? TotalOrderQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Efficiency { get; set; }      
        public DateTime ProduceDate { get; set; }        
        public string Operation { get; set; }
        public string LSStyle { get; set; }
        public string Item { get; set; }
        public bool IsAllocated { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string UserName { get; set; }
    }
}

