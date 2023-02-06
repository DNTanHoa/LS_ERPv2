using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class SeasonReportDto
    {
        /// <summary>
        /// Thông tin đơn hàng, forcast, mua thêm
        /// </summary>
        public string SalesOrderID { get; set; }
        public string ForecastTitle { get; set; }

        /// <summary>
        /// Thông tin item
        /// </summary>
        public string ItemCode { get; set; } = string.Empty;
        public string ItemID { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemColorCode { get; set; } = string.Empty;
        public string ItemColorName { get; set; } = string.Empty;
        public string Specify { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        
        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorCode { get; set; } = string.Empty;
        public string GarmentColorName { get; set; } = string.Empty;
        public string GarmentSize { get; set; } = string.Empty;

        /// <summary>
        /// Style No
        /// </summary>
        public string CustomerStyle { get; set; } = string.Empty;
        public string LSStyle { get; set; } = string.Empty;

        /// <summary>
        /// Đơn vị
        /// </summary>
        public string PerUnitID { get; set; } = string.Empty;
        public string PriceUnitID { get; set; } = string.Empty;

        /// <summary>
        /// Quantity report
        /// </summary>
        public decimal QuantityPerUnit { get; set; }
        public decimal ConsumeQuantity { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal ReceiptQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
    }
}
