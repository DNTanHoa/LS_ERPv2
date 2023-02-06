using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos.SalesOrder
{
    public class SalesOrderCompareDto
    {
        public SalesOrderCompareDto()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
        }

        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]  // Hide the entity identifier from UI.
        public string ID { get; set; }

        [XafDisplayName("PONum")]
        public string PONum { get; set; }

        [XafDisplayName("New Qty")]
        public decimal NewPlannedQty { get; set; }

        [XafDisplayName("Old Qty")]
        public decimal OldPlannedQty { get; set; }

        [XafDisplayName("Division")]
        public string Division { get; set; }
        public string OldDivision { get; set; }

        [XafDisplayName("Season")]
        public string Seasons { get; set; }
        public string OldSeason { get; set; }

        [XafDisplayName("Ship Date")]
        public DateTime ShipDate { get; set; }

        [XafDisplayName("Old Ship Date")]
        public DateTime OldShipDate { get; set; }

        [XafDisplayName("ETD")]
        public DateTime ETDs { get; set; }
        public DateTime OldETD { get; set; }

        [XafDisplayName("Contract No")]
        public string ContractNos { get; set; }
        public string OldContractNo { get; set; }

        [XafDisplayName("Customer Style")]
        public string CustomerStyle { get; set; }

        public string OldCustomerStyle { get; set; }

        [XafDisplayName("New Pack Ratio")]
        public string NewPackRatio { get; set; }

        [XafDisplayName("Old Pack Ratio")]
        public string OldPackRatio { get; set; }

        [XafDisplayName("Status")]
        public StatusCompare Status { get; set; }

        public string Color { get; set; }
        public string OldColor { get; set; }
        [XafDisplayName("Color Name")]
        public string ItemColorName { get; set; }
        public string LabelName { get; set; }
        public string OldLabelName { get; set; }

        public string Label { get; set; }
        public string OldLabel { get; set; }

        [XafDisplayName("Fashion Color")]
        public string FullDescription { get; set; }

        [XafDisplayName("Old Fashion Color")]
        public string OldFullDescription { get; set; }

        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
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
        public IList<OrderDetailsDto> OrderDetails { get; set; }
        public enum StatusCompare
        {
            Normal, New, Update, Cancel, Confirm
        }
    }
}
