using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class BulkStorageRequest
    {
        public string CustomerID { get; set; }
        public string ProductionMethodCode { get; set; }
        public string StorageCode { get; set; }
        public string FileName { get; set; }
        public string UserName { get; set; }
        public bool Output { get; set; }
        public List<ImportStorageData> Data { get; set; }
    }
}
