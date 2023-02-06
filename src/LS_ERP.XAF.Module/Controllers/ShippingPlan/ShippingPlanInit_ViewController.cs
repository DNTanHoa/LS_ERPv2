using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShippingPlanInit_ViewController : ObjectViewController<DetailView, ShippingPlan>
    {
        public ShippingPlanInit_ViewController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();

        }
        protected override void OnViewControlsCreated()
        {
            var ShippingPlan = View.CurrentObject as ShippingPlan;

            if (ShippingPlan != null)
            {               
                if (ShippingPlan.Company == null || ShippingPlan.Customer == null)
                {                  
                    ShippingPlan.SetCreateAudit(SecuritySystem.CurrentUserName);
                }
            }

            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {

            base.OnDeactivated();
        }
    }
}
