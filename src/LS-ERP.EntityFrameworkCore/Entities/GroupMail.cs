using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class GroupMail : Audit
    {
        public long? ID { get; set; }
        public string DepartmentName { get; set; }
        public string Mail { get; set; }
        public string CCs { get; set; }
        public string BCCs { get; set; }
        public string CustomerID { get; set; }
        public string CompanyCode { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Company Company { get; set; }
    }
}
