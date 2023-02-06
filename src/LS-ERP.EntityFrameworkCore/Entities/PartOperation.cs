using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PartOperation : Audit
    {
        public long ID { get; set; }
        public string PartNumber { get; set; }
        public long PartRevisionID { get; set; }
        public long OperationID { get; set; }
    }
}
