using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class JobOutput : Audit
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string JobHeadNumber { get; set; }
        public string JobOperationID { get; set; }
        public string JobOperationName { get; set; }
        public DateTime? OutputAt { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TotalTargetQuantity { get; set; }
        public decimal? TargetQuantity { get; set; }
        public decimal? PassedQuantity { get; set; }
        public decimal? Efficiency { get; set; }
        public string Problem { get; set; }
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public string DepartmentID { get; set; }
        public int WorkingTimeID { get; set; }
        public string WorkingTimeName { get; set; }
        public string StyleNO { get; set; }
        public string ItemStyleDescription { get; set; }
        public int DailyTargetID { get; set; }
        public string  Operation { get; set; }
        public int NumberOfWorker { get; set; }
        [JsonIgnore]
        public virtual JobHead JobHead { get; set; }
        [JsonIgnore]
        public virtual JobOperation JobOperation { get; set; }

        public virtual WorkingTime? WorkingTime { get; set; }
        public virtual WorkCenter WorkCenter { get;set;}   
    }
}
