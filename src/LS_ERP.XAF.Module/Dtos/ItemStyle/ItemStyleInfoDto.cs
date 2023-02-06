using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ItemStyleInfoDto
    {
        public string LSStyle { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ProductionSketDeliveryDate { get; set; }
        public DateTime? AccessoriesDate { get; set; }
        public DateTime? FabricDate { get; set; }
        public string Remark { get; set; }
    }
}
