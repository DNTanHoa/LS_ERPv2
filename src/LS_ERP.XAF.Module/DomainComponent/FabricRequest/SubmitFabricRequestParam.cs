using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SubmitFabricRequestParam
    {
        //private ApplicationUser user;
        //[XafDisplayName("Submit to")]
        //public ApplicationUser User
        //{
        //    get => user;
        //    set
        //    {
        //        this.user = value;
        //        this.Email = value?.Email;
        //    }
        //}
        //public string Email { get; set; }
        public string Remarks { get; set; }

        public List<FabricRequest> FabricRequests { get; set; }
        //public IList<PartMaterial> PartMaterials { get; set; }

    }
}
