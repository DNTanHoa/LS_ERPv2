using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ViewPackingLineHangerParam
    {
        public List<HangerForPacking> Hangers { get; set; }
    }

    [DomainComponent]
    public class HangerForPacking
    {
        public int FromNo { get; set; }
        public int ToNo { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalCarton { get; set; }
        public string Size { get; set; }
        public Hanger Hanger { get; set; }
        public Unit Unit { get; set; }

        [VisibleInListView(false)]
        public string PrePack { get; set; }
    }
}
