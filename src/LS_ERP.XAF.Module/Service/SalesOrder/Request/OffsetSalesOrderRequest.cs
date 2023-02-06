using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class OffsetSalesOrderRequest
    {
        public string CustomerID { get; set; }
        public string UserName { get; set; }
        public List<SalesOrderOffset> Data { get; set; }
        = new List<SalesOrderOffset>();
    }
}
