using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class ItemReportDto
    {
        /// <summary>
        /// Garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Postion { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// Số lượng yêu cầu
        /// </summary>
        public decimal? RequiredQuantity { get; set; }
        /// <summary>
        /// Số lượng mua forecast
        /// </summary>
        public decimal? ForecastQuantity { get; set; }
        /// <summary>
        /// Số lượng mua theo order
        /// </summary>
        public decimal? PurchaseQuantity { get; set; }
        /// <summary>
        /// Số lượng đã nhập kho
        /// </summary>
        public decimal? ReceiptQuantity { get; set; }
        /// <summary>
        /// Số lượng đã xuất kho
        /// </summary>
        public decimal? IssuedQuantity { get; set; }
        /// <summary>
        /// Số lượng còn lại của item
        /// </summary>
        public decimal? RemainQuantity { get; set; }
    }
}
