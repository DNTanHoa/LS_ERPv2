using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class AllocDailyOutputChangeDtos
    {
        public int ID { get; set; }
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal OldQuantity { get; set; }
        public decimal NewQuantity { get; set; }
        public decimal CuttingQuantity { get; set; }
        public bool IsFull { get; set; }
        public string FabricContrastName { get; set; }
        public string Set { get; set; }
        public string Operation { get; set; }
        public string Remark { get; set; }
    }
}
