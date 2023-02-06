using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PartRevisionLog : Audit
    {
        public PartRevisionLog()
        {
            Code = string.Empty;
        }

        public string Code { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        /// <summary>
        /// Reference information
        /// </summary>
        public string PartRevisionLogReferenceCode { get; set; }
        public long PartRevisionID { get; set; }

        public virtual PartRevisionLog PartRevisionLogReference { get; set; }
        public virtual PartRevision PartRevision { get; set; }

        public virtual IList<PartRevisionLogDetail> PartRevisionLogDetails { get; set; }
    }
}
