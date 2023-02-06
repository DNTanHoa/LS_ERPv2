using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos.SalesOrder
{
    [DomainComponent]
    [DefaultClassOptions]
    [Appearance("CompareItemList_DetailView", AppearanceItemType = "ViewItem", TargetItems = "Status",
     Criteria = "Status = 1", Context = "ListView", FontColor = "Green", Priority = 1)]
    [Appearance("CompareItemList_DetailView1", AppearanceItemType = "ViewItem", TargetItems = "Status",
     Criteria = "Status = 2", Context = "ListView", FontColor = "Goldenrod", Priority = 2)]
    [Appearance("CompareItemList_DetailView2", AppearanceItemType = "ViewItem", TargetItems = "Status",
     Criteria = "Status = 3", Context = "ListView", FontColor = "Red", Priority = 3)]
    public class SalesOrderCompareDto
    {
        public SalesOrderCompareDto()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
        }

        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]  // Hide the entity identifier from UI.
        public string ID { get; set; }

        public string PurchaseOrderNumber { get; set; }
        public int PurchaseOrderNumberIndex { get; set; }

        [XafDisplayName("New Qty")]
        public decimal? NewPlannedQty { get; set; }

        [XafDisplayName("Old Qty")]
        public decimal? OldPlannedQty { get; set; }
        public string Division { get; set; }
        public string OldDivision { get; set; }
        public string Season { get; set; }
        public string OldSeason { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? OldShipDate { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? OldETD { get; set; }

        public string ContractNo { get; set; }
        public string OldContractNo { get; set; }

        public string CustomerStyle { get; set; }

        public string OldCustomerStyle { get; set; }

        [XafDisplayName("NewPackRatio")]
        public string Packing { get; set; }

        [XafDisplayName("OldPackRatio")]
        public string OldPacking { get; set; }

        [XafDisplayName("Status")]
        public StatusCompare Status { get; set; }

        public string ColorCode { get; set; }
        public string OldColor { get; set; }
        public string ColorName { get; set; }
        public string LabelName { get; set; }
        public string OldLabelName { get; set; }

        public string LabelCode { get; set; }
        public string OldLabelCode { get; set; }

        [XafDisplayName("Fashion Color")]
        public string ProductionDescription { get; set; }

        [XafDisplayName("Old Fashion Color")]
        public string OldProductionDescription { get; set; }

        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string Size { get; set; }
        public string OldSize { get; set; }
        public string HangFlat { get; set; }
        public string OldHangFlat { get; set; }
        public bool Confirm { get; set; }

        [VisibleInDashboards(false)]
        [VisibleInLookupListView(false)]
        [VisibleInReports(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public bool BomPulled { get; set; }

        [VisibleInDashboards(false)]
        [VisibleInLookupListView(false)]
        [VisibleInReports(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public bool QtyCalculated { get; set; }

        [VisibleInDashboards(false)]
        [VisibleInLookupListView(false)]
        [VisibleInReports(false)]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        public bool QtyBalanced { get; set; }

        //[VisibleInDashboards(false)]
        //[VisibleInLookupListView(false)]
        //[VisibleInReports(false)]
        //[VisibleInListView(false)]
        //[VisibleInDetailView(false)]
        public string StyleNumber { get; set; }
        public string SalesOrderID { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string UE { get; set; }
        public string PCB { get; set; }
        public string OldPCB { get; set; }
        public decimal? MSRP { get; set; }
        public DateTime? ContractDate { get; set; }
        public string ShipMode { get; set; }
        public string Year { get; set; }
        public string UnitID { get; set; }
        public IList<OrderDetailsDto> OrderDetails { get; set; }
        public enum StatusCompare
        {
            Normal, New, Update, Cancel, Confirm
        }
    }
}
