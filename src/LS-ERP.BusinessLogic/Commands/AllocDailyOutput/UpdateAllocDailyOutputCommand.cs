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
    public class UpdateAllocDailyOutputCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<AllocDailyOutput>>
    {
        public int ID { get; set; }
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public string Set { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal Percent { get; set; }
        public decimal PercentQuantity { get; set; }
        public decimal Sample { get; set; }
        public bool IsFull { get; set; }
        public bool IsCanceled { get; set; }
        public string Operation { get; set; }

        //for cutting
        public int TargetID { get; set; }
        public int FabricContrastID { get; set; }
        public string FabricContrastName { get; set; }
        public int CuttingOutputID { get; set; }
        public DateTime PSDD { get; set; }
        public string EstimatedPort { get; set; }
        public string OrderTypeName { get; set; }

        public string Season { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string LastUpdatedBy { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public decimal PercentPrint { get; set; }

    }
}
