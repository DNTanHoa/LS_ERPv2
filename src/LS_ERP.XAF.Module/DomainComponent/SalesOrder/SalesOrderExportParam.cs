using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderExportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }
 
        public DateTime? ShipDate { get; set; }
    }
}
