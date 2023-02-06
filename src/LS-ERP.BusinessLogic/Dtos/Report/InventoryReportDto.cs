using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class InventoryReportDto
    {
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
        /// Garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyles { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }

        /// <summary>
        /// Số lượng mua theo order
        /// </summary>
        public decimal? PurchaseQuantity { get; set; }
        /// <summary>
        /// Số tồn
        /// </summary>
        public decimal? OnHandQuantity { get; set; }
        /// <summary>
        /// Số lượng đã nhập kho
        /// </summary>
        public decimal? ReceiptQuantity { get; set; }
        /// <summary>
        /// Số lượng đã xuất kho
        /// </summary>
        public decimal? IssuedQuantity { get; set; }
        /// <summary>
        /// Số lượng chưa nhận hàng
        /// </summary>
        public decimal? NotYetQuantity { get; set; }
        /// <summary>
        /// Số lượng còn lại của item
        /// </summary>
        public decimal? RemainQuantity { get; set; }
        public string BinCode { get; set; }
        public int StorageDetailID { get; set; }
    }
}
