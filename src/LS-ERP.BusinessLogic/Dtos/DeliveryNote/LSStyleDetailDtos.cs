using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class LSStyleDetailDtos
    {
        public string LSStyle { get; set; }
        public string Set { get; set; }
        public string FabricContrastName { get; set; }
        public string Size { get; set; }
        public decimal Quantity { get; set; }
    }
}
