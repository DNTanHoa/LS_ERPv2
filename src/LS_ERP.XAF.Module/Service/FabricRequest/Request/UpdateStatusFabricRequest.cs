using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class UpdateStatusFabricRequest
    {
        public long ID { get; set; }
        public string StatusID { get; set; }
        public string UserName { get; set; }
        public string Remark { get; set; }
    }
}
