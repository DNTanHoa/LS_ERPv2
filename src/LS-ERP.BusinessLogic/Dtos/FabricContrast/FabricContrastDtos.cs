using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class FabricContrastDtos
    {
        public int ID { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionForShirt { get; set; }
        public string DescriptionForPant { get; set; }
        public string ContrastColor { get; set; }
    }
}
