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
    public class BulkOffsetDailyTargetDetailCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>>
    {
        public List<OffsetDailyTargetDetailModel> Data { get; set; }
    }
    public class OffsetDailyTargetDetailModel
    {
        public int ID { get; set; }
        public string StyleNO { get; set; }
        public string Item { get; set; }
        public DateTime ProduceDate { get; set; }
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public decimal? RejectRate { get; set; }
        public decimal? Offset { get; set; }
    }
}
