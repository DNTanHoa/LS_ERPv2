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
    public class BulkJobOperationCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<IEnumerable<JobOperation>>>
    {
        public List<JobOperationModel> Data { get; set; }
    }

    public class JobOperationModel
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public DateTime? EstimateStartDate { get; set; }
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
