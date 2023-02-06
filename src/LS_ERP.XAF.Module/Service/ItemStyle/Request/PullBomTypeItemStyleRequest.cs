using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class PullBomTypeItemStyleRequest
    {
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public string PullBomTypeCode { get; set; }
        public string Season { get; set; }
        public string SalesOrderID { get; set; }
        public List<string> ItemStyleNumbers { get; set; }
        public List<string> Seasons { get; set; }
    }
}
