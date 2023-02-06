using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportLabelPortRequest
    {
        public string CustomerID { get; set; }
        public string UserName { get; set; }
        public bool Update { get; set; }
        public string FilePath { get; set; }
    }
}
