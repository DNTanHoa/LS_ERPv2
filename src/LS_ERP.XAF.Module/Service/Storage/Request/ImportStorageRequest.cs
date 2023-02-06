using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportStorageRequest
    {
        public string FilePath { get; set; }
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public string ProductionMethodCode { get; set; }
        public string StorageCode { get; set; }
        public bool Output { get; set; }
    }
}
