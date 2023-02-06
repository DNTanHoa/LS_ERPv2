using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using Ultils.Calculators;
using Ultils.Extensions;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchasePopupModel
    {
        public PurchaseOrder PurchaseOrder { get; set; }
        public Storage Storage { get; set; }
        public string BinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        [VisibleInDetailView(true)]
        public DateTime? ReceiptDate { get; set; }
        public DateTime? ArrivedDate { get; set; }
        public bool GetGroup { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public Unit Unit { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public List<PurchaseOrderInforData> PurchaseInfor { get; set; }
        public List<PurchaseOrderGroupInforData> PurchaseGroup { get; set; }
    }

    [DomainComponent]
    public class PurchaseOrderInforData
    {
        public long ID { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public long? PurchaseOrderGroupLineID { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string ContractNo { get; set; }
        public string PurchaseUnitID { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? ReceiptQuantity { get; set; }
        public decimal? RemainQuantity => Math.Round((PurchaseQuantity ?? 0) - (ReceiptQuantity ?? 0));

        public string EntryUnitID { get; set; }
        public decimal? EntryQuantity { get; set; }
        public decimal? Roll { get; set; }
        public string BinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }

        public string Remark { get; set; }

        public IList<PurchaseOrderDetailInforData> DetailInforDatas { get; set; }
    }

    [DomainComponent]
    public class PurchaseOrderDetailInforData
    {
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public long? PurchaseOrderGroupLineID { get; set; }
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public long? PurchaseOrderLineID { get; set; }

        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }

        public string PurchaseUnitID { get; set; }
        public decimal? PurchaseQuantity { get; set; }

        public string EntryUnitID { get; set; }
        public decimal? EntryQuantity { get; set; }

        public string BinCode { get; set; }
        public string LotNumber { get; set; }

        public string Remark { get; set; }
    }

    [DomainComponent]
    public class PurchaseOrderGroupInforData
    {
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string ContractNo { get; set; }
        public string PurchaseUnitID { get; set; }
        public decimal? PurchaseQuantity { get; set; }

        public string EntryUnitID { get; set; }
        public decimal? EntryQuantity { get; set; }

        //public decimal? entryQuantity { get; set; }


        //public string EntryQuantity
        //{
        //    get => entryQuantity?.ToString();
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            var expression = value.CalculateExpression();
        //            entryQuantity = (int)InfixEvaluator.EvaluateInfix(value.ToString());
        //        }
        //        else
        //        {
        //            entryQuantity = null;
        //        }
        //    }
        //}

        public decimal? Roll { get; set; }
        public string BinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }

        public string Remark { get; set; }
    }
}
