using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Shipment : Audit
    {
        public long ID { get; set; }
        public string FileName { get; set; }
        public string FileNameServer { get; set; }
        public string FilePath { get; set; }
        public string CustomerID { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual List<ShipmentDetail> Details { get; set; }
            = new List<ShipmentDetail>();
        public virtual List<ShipmentSummary> Summaries { get; set; }
            = new List<ShipmentSummary>();
    }
}
