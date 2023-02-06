using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ScanResult : Audit
    {
        public ScanResult()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        public long ID { get; set; }
        public string Oid { get; set; }
        public string PONumber { get; set; }
        public string Barcode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int? Expected { get; set; }
        public int? Found { get; set; }
        public int? Missing { get; set; }
        public int? Unexpected { get; set; }
        public int? OperationsCount { get; set; }
        public int? Sequence { get; set; }
        public string CompanyID { get; set; }
        public bool? IsConfirm { get; set; }
        public bool? IsDeActive { get; set; }
        [JsonIgnore]
        public virtual Company Company { get; set; }
        public virtual List<ScanResultDetail> Details { get; set; }
    }
}
