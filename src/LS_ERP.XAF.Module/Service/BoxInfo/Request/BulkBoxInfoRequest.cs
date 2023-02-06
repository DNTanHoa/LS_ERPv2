using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class BulkBoxInfoRequest
    {
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public List<BoxInfo> Data { get; set; }
    }
}
