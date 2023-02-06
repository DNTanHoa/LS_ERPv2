using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ScanResultDetail : Audit
    {
        public long ID { get; set; }    
        public long ScanResultID { get; set; }
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

        [JsonIgnore]
        public virtual ScanResult ScanResult { get; set; }
    }
}
