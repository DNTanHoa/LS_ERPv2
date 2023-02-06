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
    public class CreateJobOutputCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<JobOutput>>
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string JobHeadNumber { get; set; }
        public string JobOperationID { get; set; }
        public string JobOperationName { get; set; }
        public DateTime? OutputAt { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TargetQuantity { get; set; }
        public decimal? PassedQuantity { get; set; }
        public decimal? Efficiency { get; set; }
        public string Problem { get; set; }
        public string WorkCenterID { get; set; }
        public int WorkingTimeID { get; set; }
        public string WorkingTimeName { get; set; }
        public string StyleNO { get; set; }
        public string ItemStyleDescription { get; set; }
    }
}
