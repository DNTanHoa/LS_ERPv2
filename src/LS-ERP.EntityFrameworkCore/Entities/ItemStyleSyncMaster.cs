using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    /// <summary>
    /// Đồng bộ nguyên phụ liệu
    /// </summary>
    public class ItemStyleSyncMaster : Audit
    {
        /// <summary>
        /// Reference
        /// </summary>
        public int ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string CustomerID { get; set; }

        /// <summary>
        /// Tháng
        /// </summary>
        public string Monthly { get; set; }
        /// <summary>
        /// Thương hiệu: DE -> Domyos, Kipstra
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// Garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string OldLSStyle { get; set; }

        /// <summary>
        /// Customer purchase order number
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
        public string OldPurchaseOrderNumber { get; set; }
        /// <summary>
        /// ACJL, ACHJ, ABYA
        /// </summary>
        public string ProductionDescription { get; set; }
        /// <summary>
        /// Ngảy giao hàng trượt sản xuất
        /// 7 - 15 ngày trước ngày giao hàng cho khách
        /// </summary>
        public DateTime? ProductionSkedDeliveryDate { get; set; }
        public DateTime? OldProductionSkedDeliveryDate { get; set; }
        /// <summary>
        /// Ngày xuất hàng
        /// </summary>
        public DateTime? ContractualSupplierHandover { get; set; }
        public DateTime? EstimateDate { get; set; }
        /// <summary>
        /// Order Type: Replenishement, Implantation
        /// </summary>
        public string PurchaseOrderType { get; set; }
        /// <summary>
        /// Model
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string OldGarmentColorCode { get; set; }
        /// <summary>
        /// Garment color name
        /// </summary>
        public string Description { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal OldTotalQuantity { get; set; }
        public string UnitID { get; set; }
        public string DeliveryPlace { get; set; }
        public string OldDeliveryPlace { get; set; }

        /// <summary>
        /// Issued date
        /// </summary>
        public DateTime? IssuedDate { get; set; }
        public DateTime? IssuedDateBU { get; set; }
        public string Season { get; set; }
        public string OldSeason { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// Product type: Legging, T-shirt,....
        /// </summary>
        public string ProductType { get; set; }
        public string Merchandiser { get; set; }
        /// <summary>
        /// Ngày đồng bộ
        /// </summary>
        public DateTime? AccessoriesDate { get; set; }
        public DateTime? FabricDate { get; set; }

        /// <summary>
        /// Ngày thực tế
        /// </summary>
        public DateTime? ActualAccessoriesDate { get; set; }
        public DateTime? ActualFabricDate { get; set; }

        public string Cutting { get; set; }
        public string Sewing { get; set; }

        public virtual ItemStyle ItemStyle { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual List<ItemStyleSyncAction> ItemStyleSyncActions { get; set; }
            = new List<ItemStyleSyncAction>();

        public virtual List<OrderDetailSync> OrderDetailSyncs { get; set; }
            = new List<OrderDetailSync>();

        public virtual List<MaterialSync> MaterialSyncs { get; set; }
            = new List<MaterialSync>();
    }
}
