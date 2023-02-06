using DevExpress.ExpressApp.DC;

using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class LoadPackingListSearchParam
    {
        
        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string InvoiceNO { get; set; }
      
        public string PurcharseNumerList { get; set; }

        public List<PackingListPopupModel> PackingLists
            = new List<PackingListPopupModel>();
    }
}
