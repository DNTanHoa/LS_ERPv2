using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PackingListCreateParam
    {
        public BoxDimension Dimension { get; set; }
        public BoxDimension InnerDimension { get; set; }
        public List<ItemStyle> PackingStyles { get; set; }
        public List<PackingLine> PackingLines { get; set; }

        [VisibleInDetailView(false)]
        public int CartonNo { get; set; } = 1;
        [VisibleInListView(false)]
        public int LastNoCartonShortShip { get; set; } = 0;
        [VisibleInListView(false)]
        public int OrdinalShip { get; set; } = 0; 
        [VisibleInListView(false)]
        public Dictionary<string,int> RemainQuantity { get; set; }   
    }
}
