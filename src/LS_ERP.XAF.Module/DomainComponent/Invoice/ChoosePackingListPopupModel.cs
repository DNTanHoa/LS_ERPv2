using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ChoosePackingListPopupModel
    {
        public int ID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PackingListCode { get; set; }
        public DateTime? PackingListDate { get; set; }
        public string CustomerID { get; set; }
        public string SalesOrderID { get; set; }
        public string DeliveryGroup { get; set; }
        public string ShippingMethodCode { get; set; }
        public string ShippingMark1Code { get; set; }
        public string ShippingMark2Code { get; set; }
        public string RatioLeft { get; set; }
        public string RatioRight { get; set; }
        public string RatioBottom { get; set; }
        public string LSStyles { get; set; }
        public decimal? TotalQuantity { get; set; }
        public bool? Confirm { get; set; }
        public bool? DontShortShip { get; set; }
        public string CompanyCode { get; set; }
        public string Factory { get; set; }
        public int? OrdinalShip { get; set; } /// Multi ship
        public string BrandPurchaseOrder { get; set; } /// HA import
        public bool? BarCodeCompleted { get; set; } /// GA check inputted barcode
        public int? TotalBarCodeCarton { get; set; }  /// GA pull barcode
        public long? SheetNameID { get; set; } /// GA export excel
        public bool? IsShipped { get; set; }    /// Shipping Plan is shipped
        public DateTime? PPCBookDate { get; set; }

        ///////// Invoice GA separete packing list  
        public bool? IsSeparated { get; set; }
        public int? ParentPackingListID { get; set; }
        public int? SeparatedOrdinal { get; set; }
        public bool? IsRevised { get; set; }
        //
        public virtual IList<ItemStyle> ItemStyles { get; set; }
        public virtual IList<PackingLine> PackingLines { get; set; }
        public virtual PackingUnit PackingUnit { get; set; }
        public string SheetName { get; set; }
        
    }
}
