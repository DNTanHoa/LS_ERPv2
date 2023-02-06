using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class IssuedFabricParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string IssuedBy { get; set; }
        public string ReceivedBy { get; set; }
        public Storage Storage { get; set; }
        public Customer Customer { get; set; }
        public string Description { get; set; } = string.Empty;
        public PriceTerm ProductionMethodCode { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public long? FabricRequestID { get; set; }

        public List<IssuedFabric> Fabrics { get; set; } = new List<IssuedFabric>();
        public List<FabricRequestDetail> FabricRequestDetails { get; set; } = new List<FabricRequestDetail>();
    }

    [DomainComponent]
    public class IssuedFabric
    {
        public string FabricPurchaseOrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public long StorageDetailID { get; set; }
        public string CustomerStyle { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string UnitID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Season { get; set; }
        public string StorageStatusID { get; set; }
        public long? FabricRequestDetailID { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        public decimal? InStockQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? RequestQuantity { get; set; }
        public decimal? RollInStock { get; set; }

        [XafDisplayName("Roll Issued")]
        public decimal? Roll { get; set; }
    }
}
