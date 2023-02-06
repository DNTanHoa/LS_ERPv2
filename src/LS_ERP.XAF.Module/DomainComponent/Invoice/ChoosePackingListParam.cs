using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ChoosePackingListParam
    {
        public string LSStyles { get; set; }
        public string CustomerID { get; set; }
        public List<PackingList> ExistPackingList { get; set; }
        public List<ChoosePackingListPopupModel> ChoosePackingListPopupModel { get; set; }
    }
}
