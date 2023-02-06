using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportSalesOrderOffsetResponse : CommonRespone
    {
        public List<SalesOrderOffsetMaterial> Data { get; set; }
    }

    public class SalesOrderOffsetResponseData
    {
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
