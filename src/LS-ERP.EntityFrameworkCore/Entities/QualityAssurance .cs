using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class QualityAssurance : Audit
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerID { get; set; }
        public string StorageCode { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string LSStyle { get; set; }
        public string CustomerStyle { get; set; }
        public string Season { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string BinCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Percent { get; set; }
        public string QualityStatusID { get; set; }
        public string Remark { get; set; }
        public virtual QualityStatus QualityStatus { get; set; }
    }
}
