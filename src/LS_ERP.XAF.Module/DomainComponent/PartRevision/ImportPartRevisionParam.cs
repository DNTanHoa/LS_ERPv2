using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ImportPartRevisionParam
    {
        public long ID { get; set; }
        public string StyleNumber { get; set; }
        public Customer Customer { get; set; }
        public string RevisionNumber { get; set; }
        public string Season { get; set; }
        public DateTime? EffectDate { get; set; }
        public bool? IsConfirmed { get; set; }

        public string FilePath { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public string FileName { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public string FileNameServer { get; set; }

        public List<PartMaterial> PartMaterials { get; set; }
        public List<Item> Items { get; set; }
    }
}
