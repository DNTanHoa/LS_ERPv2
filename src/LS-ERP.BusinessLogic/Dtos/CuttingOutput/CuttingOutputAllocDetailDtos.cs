using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class CuttingOutputAllocDetailDtos
    {
        public DateTime PSDD { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string MergeLSStyle { get; set; }
        public string PriorityLSStyle { get; set; }       
        public string Size { get; set; }
        public string Set { get; set; }
        public string Lot { get; set; }
        public string FabricContrastName { get; set; }
        public string FabricContrastDescription { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal AllocQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal CuttingQuantity { get; set; }
        public decimal Balance { get; set; }        
        public CuttingOutput CuttingOutput { get; set;}
    }
}
