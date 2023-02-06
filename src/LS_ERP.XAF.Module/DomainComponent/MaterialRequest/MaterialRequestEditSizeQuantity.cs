using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class MaterialRequestEditSizeQuantity
    {
        public List<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();
    }
}
