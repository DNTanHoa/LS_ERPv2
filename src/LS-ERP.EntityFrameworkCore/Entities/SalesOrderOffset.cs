using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesOrderOffset : Audit
    {
        public int Id { get; set; }

        /// <summary>
        /// ACC: đắp phụ liệu
        /// FG: đắp thành phẩm
        /// </summary>
        public string Type { get; set; }

        public string SalesOrderID { get; set; } = string.Empty;
        public string CustomerStyle { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public string TargetLSStyle { get; set; } = string.Empty;
        public string SourceLSStyle { get; set; } = string.Empty;
        public string GarmentSize { get; set; } = string.Empty;
        public decimal OffsetQuantity { get; set; } = 0;

        /// <summary>
        /// Đắp phụ liệu
        /// </summary>
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Kết quả xử lý đắp đơn
        /// </summary>
        public bool IsProcess { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime ProcessAt { get; set; }
        public string ErrorMessage { get; set; }
    }
}
