using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class FabricRequestLog : Audit
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string CustomerStyleNumber { get; set; }
        public string OrderNumber { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? PercentWastage { get; set; }
        public string Remark { get; set; }
        public string StatusID { get; set; }
        public string Reason { get; set; }
        public long FabricRequestID { get; set; }
        public virtual FabricRequest FabricRequest { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Company Company { get; set; }
        public virtual Status Status { get; set; }
        public virtual List<FabricRequestDetailLog> Details { get; set; }
            = new List<FabricRequestDetailLog>();
    }
}
