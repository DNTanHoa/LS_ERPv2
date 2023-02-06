using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ViewPackingLineBarCodeParam
    {
        public List<BarCodeForPacking> BarCodes { get; set; }
    }

    [DomainComponent]
    public class BarCodeForPacking
    {
        //[VisibleInListView(false)]
        //public long PackingListID { get; set; }
        public int FromNo { get; set; }
        public int ToNo { get; set; }
        public int TotalCarton { get; set; }
        public string Size { get; set; }
        public string BarCode { get; set; }

        [VisibleInListView(false)]
        public string PrePack { get; set; }
    }
}
