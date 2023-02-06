using DevExpress.ExpressApp.DC;
using LS_ERP.XAF.Module.BusinessObjects;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    [XafDisplayName("Submit Purchase Request")]
    public class SubmitPurchaseRequestParam
    {
        private ApplicationUser user;
        [XafDisplayName("Submit to")]
        public ApplicationUser User
        {
            get => user;
            set 
            {
                this.user = value;
                this.Email = value.Email;
            }
        }
        public string Email { get; set; }
        public string Remarks { get; set; }
    }
}
