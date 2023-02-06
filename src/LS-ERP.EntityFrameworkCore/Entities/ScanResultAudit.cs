using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ScanResultAudit : Audit
    {
        public long ID { get; set; }
        public long ScanResultDetailID { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentSize { get; set; }
        public string TagID { get; set; }
        public int? Expected { get; set; }
        public int? Found { get; set; }
        public int? Missing { get; set; }
        public int? Unexpected { get; set; }
        public string Description { get; set; }
        public string ItemCode { get; set; }
        public string AuditUserName { get; set; }
        public DateTime? AuditDate { get; set; }
    }
}
