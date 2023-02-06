using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PackingListSetCRDParam
    {
        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public DateTime? PackingFromDate { get; set; }
        public DateTime? PackingToDate { get; set; }
        public string LSStyles { get; set; }
        public DateTime? CargoReadyDate  { get; set; }

        public List<PackingListPopupModel> PackingLists = new List<PackingListPopupModel>();
    }
}
