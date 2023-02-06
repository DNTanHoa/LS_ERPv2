using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ItemPriceSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public Vendor Vendor { get; set; }
        public DateTime? EffectFrom { get; set; }
        public DateTime? EffectTo { get; set; }

        public List<ItemPrice> ItemPrices { get; set; }
    }
}
