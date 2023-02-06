using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class PivotCuttingLotDtos
    {
        public string LSStyle { get; set; }
        public string  MergeLSStyle { get; set; }
        public string GarmentColor { get; set; }
        public string Set { get; set; }
        public string Size { get; set; }
        public string Lot { get; set; }
        public decimal TotalLSStyleQuantity { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal Quantity { get; set; }  
        public decimal? CutQuantity { get; set; }
        public string Status { get; set; }
        public bool IsFull { get; set; }
       
        public void SetStatus()
        {
            if(this.IsFull)
            {
                this.Status = "OK";
            }
            else
            {
                this.Status = "NO";
            }    
        }
    }
}
