using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class InventoryPeriodEntry : Audit
    {
        public int ID { get; set; }
        public long InventoryPeriodID { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// CLOSED - Đóng kỳ
        /// RE-OPEN - Mở lại kỳ đã đóng
        /// OPEN - Mở kỳ kho
        /// </summary>
        public string EntryType { get; set; }

        public virtual InventoryPeriod InventoryPeriod { get; set; }
    }
}
