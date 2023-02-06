using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class ItemStyleInforDtos
    {
        public string LSStyle { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ProductionSketDeliveryDate { get; set; }
        public DateTime? AccessoriesDate { get; set; }
        public DateTime? FabricDate { get; set; }
        public string Remark { get; set; }
    }
}
