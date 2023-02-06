using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PartRevision : Audit
    {
        public PartRevision()
        {
            PartMaterials = new List<PartMaterial>();
        }

        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string PartNumber { get; set; }
        public string RevisionNumber { get; set; }
        public DateTime? EffectDate { get; set; }
        public bool? IsConfirmed { get; set; }

        /// <summary>
        /// Should remove in part revision, it must be in part
        /// </summary>
        public string Season { get; set; }

        public string PullBomTypeCode { get; set; }

        public string FileName { get; set; }
        public string FileNameServer { get; set; }
        public string FilePath { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual PullBomType PullBomType { get; set; }
        public virtual IList<PartMaterial> PartMaterials { get; set; }
        public virtual IList<PartRevisionLog> PartRevisionLogs { get; set; }

        public virtual List<JobHead> JobHeads { get; set; }
    }
}
