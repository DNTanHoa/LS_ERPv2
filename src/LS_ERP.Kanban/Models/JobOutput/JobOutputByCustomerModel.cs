using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Kanban.Models
{
    public class JobOutputByCustomerModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public int Lines { get; set; } 
    }
}
