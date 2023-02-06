using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class StorageImport : Audit
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string StorageCode { get; set; }
        public string CustomerID { get; set; }
        public bool? Output { get; set; }
        public string ProductionMethodCode { get; set; }
        public virtual List<StorageImportDetail> Details { get; set; }
            = new List<StorageImportDetail>();
    }
}
