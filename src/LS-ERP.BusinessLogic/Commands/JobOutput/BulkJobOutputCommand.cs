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
    public class BulkJobOutputCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<IEnumerable<JobOutput>>>
    {
        public List<JobOutputModel> Data { get; set; }
    }

    public class JobOutputModel
    {
        public long ID { get; set; }
        public string JobHeadNumber { get; set; }
        public string JobOperationID { get; set; }
        public string JobOperationName { get; set; }
        public DateTime? OutputAt { get; set; }
        public decimal? Quantity { get; set; }
        public string WorkCenterID { get; set; }
    }
}
