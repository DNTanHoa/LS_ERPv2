using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class JobHead : Audit
    {
        public string Number { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentSize { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string Config { get; set; }
        public long? PartRevisionID { get; set; }
        public decimal? ProductionQuantity { get; set; }
        /// <summary>
        /// Số lượng mẫu và hao hụt
        /// </summary>
        public decimal? SampleQuantity { get; set; }
        public decimal? OverPercent { get; set; }
        public DateTime? RequestDueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? Confirmed { get; set; }

        public virtual PartRevision PartRevision { get; set; }
        public virtual IList<ProductionBOM> ProductionBOMs { get; set; }
    }
}
