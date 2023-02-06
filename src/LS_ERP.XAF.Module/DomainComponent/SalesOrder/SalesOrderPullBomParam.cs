using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderPullBomParam
    {
        public string Season { get; set; }
        public string Styles { get; set; }
        public List<SalesOrder> SalesOrders { get; set; }
        public List<ItemStyle> ItemStyles { get; set; }
    }
}
