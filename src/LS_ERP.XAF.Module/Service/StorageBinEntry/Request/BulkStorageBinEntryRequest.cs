using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class BulkStorageBinEntryRequest
    {
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public string Factory { get; set; }
        public string StorageCode { get; set; }
        public List<StyleStorageBinEntry> Data { get; set; }
    }
}
