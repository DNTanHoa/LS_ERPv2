using DevExpress.ExpressApp.DC;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ViewItemStyleMaterialParam
    {
        public List<ItemStyleMaterial> Material { get; set; }
            = new List<ItemStyleMaterial>();
    }
}
