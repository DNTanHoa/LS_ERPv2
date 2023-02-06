using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ProductionOrderSearchParam
    {
        public Customer Customer { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<ProductionOrder> ProductionOrders { get; set; }
        = new List<ProductionOrder>();
    }
}
