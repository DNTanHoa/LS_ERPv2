using Common.Model;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateCuttingOutputCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<CuttingOutput>>
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string MergeLSStyle { get; set; }
        public string PriorityLSStyle { get; set; }
        public bool IsAllSize { get; set; }
        public string Size { get; set; }
        public string Set { get; set; }
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public int TableNO { get; set; }
        public string FabricContrast { get; set; }
        public string FabricContrastDescription { get; set; }
        public int FabricContrastID { get; set; }
        public decimal Block { get; set; }
        public decimal TotalPackage { get; set; }
        public string Lot { get; set; }
        public decimal Layer { get; set; }
        public decimal Ratio { get; set; }
        public decimal Quantity { get; set; }
        public DateTime ProduceDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Remark { get; set; }
        public bool IsAllocated { get; set; }
        public bool IsPrint { get; set; }

    }
}
