using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class QualityAudit
    {
        public long ID { get; set; }
        public long? QualityAssuranceID { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; } 
        public string QualityStatusID { get; set; }
        public string Remark { get; set; }  
    }
}
