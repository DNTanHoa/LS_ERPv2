using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class CuttingOutputDailyReportDtos
    {
        public CuttingOutputDailyReportDtos()
        {
            this.Status = "NO";
        }
        public string MergeBlockLSStyle { get; set; }
        public string MergeLSStyle { get; set; }
        public string LSStyle { get; set; }
        public string Set { get; set; }
        public string CustomerName { get; set; }
        public decimal TargetQuantity { get; set; }
        public string WorkCenterName { get; set; }
        public string GarmentColor { get; set; }
        public decimal Quantity { get; set; }
        public string Status { get; set; }
        public DateTime ProduceDate { get; set; }
        public string ProduceDateString { get; set; }
        public string Remark { get; set; }
        public bool IsAllSize { get; set; }
        public bool IsPrint { get; set; }

        public void setStatus()
        {
            if (this.Quantity >= this.TargetQuantity)
            {
                this.Status = "OK";
            }   
            else
            {
                this.Status = "NO";
            }    
        }
        public void setStatus(string status)
        {
             this.Status = status;
        }
        public void setProduceDateString()
        {
            ProduceDateString = ProduceDate.ToString("dd/MM/yyyy");
        }

    }
}
