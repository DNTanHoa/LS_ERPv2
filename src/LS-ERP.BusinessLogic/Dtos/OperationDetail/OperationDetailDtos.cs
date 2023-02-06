using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class OperationDetailDtos
    {
        public string ID { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string Set { get; set; }
        public int FabricContrastID { get; set; }
        public string FabricContrastName { get; set; }
        public string OperationID { get; set; }
        public string OperationName { get; set; }
        public bool IsPercentPrint { get; set; }

    }
}
