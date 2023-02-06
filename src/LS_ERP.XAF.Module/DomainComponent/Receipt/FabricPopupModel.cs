using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class FabricPopupModel
    {
        public FabricPurchaseOrder FabricPurchaseOrder { get; set; }
        public Storage Storage { get; set; }
        public Unit Unit { get; set; }
        public string CustomerStyle { get; set; }
        public string BinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        [VisibleInDetailView(true)]
        public DateTime? ReceiptDate { get; set; }
        public DateTime? ArrivedDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string VendorName { get; set; }
        public string Season { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public PriceTerm ProductionMethods { get; set; }
        public bool? Offset { get; set; }
        public List<FabricPurchaseOrderInforData> FabricPurchaseInfor { get; set; }
    }

    [DomainComponent]
    public class FabricPurchaseOrderInforData
    {
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public long? FabricPurchaseOrderLineID { get; set; }
        public string FabricPurchaseOrderLineNumber { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Line { get; set; }
        public string FabricSupplier { get; set; }
        public string GarmentColorCodes { get; set; }
        public string CustomerID { get; set; }
        public string CustomerStyle { get; set; }
        public string Season { get; set; }
        public string PurchaseUnitID { get; set; }
        public string EntryUnitID { get; set; }
        public decimal? EntryQuantity { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? ReceiptQuantity { get; set; }
        public decimal? RemainQuantity => Math.Round((PurchaseQuantity ?? 0) - (EntryQuantity ?? 0) - (ReceiptQuantity ?? 0));
        public decimal? Roll { get; set; }
        public string BinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeNumber { get; set; }
        public string Remark { get; set; }
        public bool? Offset { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public decimal? FactorUnit { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public int? RoundingUnit { get; set; }
    }
}
