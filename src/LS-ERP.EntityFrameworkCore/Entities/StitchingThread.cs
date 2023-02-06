using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class StitchingThread : Audit
    {
        public long ID { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string CustomerID { get; set; }
        
    }
}
