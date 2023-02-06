using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    [XafDisplayName("Approve Quality Assurance")]
    public class QualityAssuranceApprovedPopupModel
    {
        public QualityStatus Status { get; set; }
        public string Remark { get; set; }
        [VisibleInDetailView(false)]
        public long ID { get; set; } = 0;
        [VisibleInDetailView(false)]
        public string PurchaseOrderNumber { get; set; }
        [VisibleInDetailView(false)]
        public string ItemStyleNumber { get; set; }
        [VisibleInDetailView(false)]
        public string GarmentSize { get; set; }
    }
}
