using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderOffsetParam
    {
        public Customer Customer { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public List<SalesOrderOffsetMaterial> OffsetDetails
            = new List<SalesOrderOffsetMaterial>();
    }

    [DomainComponent]
    public class SalesOrderOffsetMaterial
    {
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
    }
}
