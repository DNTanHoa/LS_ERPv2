using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.BusinessObjects;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SubmitBomParam
    {
        private ApplicationUser user;
        [XafDisplayName("Submit to")]
        public ApplicationUser User
        {
            get => user;
            set
            {
                this.user = value;
                this.Email = value?.Email;
            }
        }
        public string Email { get; set; }
        public string Remarks { get; set; }

        public IList<PartMaterial> PartMaterials { get; set; }
    }
}
