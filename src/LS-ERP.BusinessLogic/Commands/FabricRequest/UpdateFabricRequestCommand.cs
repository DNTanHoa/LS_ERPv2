using Common.Model;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateFabricRequestCommand : CommonAuditCommand,
         IRequest<UpdateFabricRequestResult>
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string CustomerStyleNumber { get; set; }
        public string OrderNumber { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? PercentWastage { get; set; }

        public string Reason { get; set; }
        public string Remark { get; set; }
        public string StatusID { get; set; }
        public List<FabricRequestDetail> Details { get; set; }
            = new List<FabricRequestDetail>();

        public List<FabricRequestLog> FabricRequestHistories { get; set; }
            = new List<FabricRequestLog>();
    }

}
