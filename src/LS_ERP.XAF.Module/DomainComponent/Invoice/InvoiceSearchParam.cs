using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class InvoiceSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }

        public Customer Customer { get; set; }
        public DateTime? OrderFromDate { get; set; }
        public DateTime? OrderToDate { get; set; }

        public List<Invoice> Invoices { get; set; }
    }
}
