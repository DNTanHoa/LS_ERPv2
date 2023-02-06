using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    [XafDisplayName("Packing Lists")]
    public class PackingListSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public DateTime? PackingFromDate { get; set; }
        public DateTime? PackingToDate { get; set; }

        public List<PackingList> PackingLists { get; set; }
    }
}
