using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderOffsetSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public string Style { get; set; } = string.Empty;
        public Customer Customer { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<SalesOrderOffset> SalesOrderOffsets
            = new List<SalesOrderOffset>();
    }
}
