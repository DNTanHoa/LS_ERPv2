using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class SalesOrderOffsetDto
    {
        /// <summary>
        /// Đắp thành phẩm: FG
        /// Đắp phụ liệu: MTL
        /// </summary>
        public string Type { get; set; } = string.Empty;
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
    }
}
