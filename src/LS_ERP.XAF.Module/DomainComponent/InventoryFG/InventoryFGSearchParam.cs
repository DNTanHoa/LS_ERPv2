using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class InventoryFGSearchParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public string Search { get; set; }
        public List<InventorySummaryFG> Inventory { get; set; }
    }

    [DomainComponent]
    public class InventorySummaryFG
    {
        public InventoryFG Inventory { get; set; }
        public decimal? ReceiptQuantity { get; set; }
        public decimal? IssueQuantity { get; set; }
    }
}
