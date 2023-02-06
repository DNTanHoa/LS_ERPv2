using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    /// <summary>
    /// Báo cáo theo dõi tiến độ đơn hàng đóng gói
    /// </summary>
    public class PackingOutput : Audit
    {
        public int Id { get; set; }
        public string LSStyle { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime ProductionSketDeliveryDate { get; set; }
        public string WorkCenters { get; set; }
        public string Destination { get; set; }
        public decimal Quantity { get; set; }
        public decimal Company { get; set; }
        public decimal PackQuantity { get; set; }
        public decimal PackPercent { get; set; }
        public string NoteProduction { get; set; }
        public string NotePacking { get; set; }
    }
}
