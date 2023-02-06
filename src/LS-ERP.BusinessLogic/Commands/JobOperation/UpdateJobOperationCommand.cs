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
    public class UpdateJobOperationCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<JobOperation>>
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public DateTime? EstimatedStartDate { get; set; }
        public DateTime? EstimateEndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? RequiredQuantity { get; set; }
        public decimal? CompletedQuantity { get; set; }
        public decimal? RemainQuantity { get; set; }
        public string EstimatedWorkCenterID { get; set; }
        public string WorkCenterID { get; set; }
        public string JobHeadNumber { get; set; }
        public int Index { get; set; }
    }
}
