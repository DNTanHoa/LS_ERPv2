using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DisableActionsInPurchaseRequest_ViewController : ViewController
    {
        public DisableActionsInPurchaseRequest_ViewController()
        {
            InitializeComponent();
        }
        
        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.Id == "PurchaseRequest_DetailView")
            {
                Frame.GetController<ModificationsController>().SaveAction.Active["1"] = false;
                Frame.GetController<ModificationsController>().SaveAndCloseAction.Active["1"] = false;
                Frame.GetController<ModificationsController>().SaveAndNewAction.Active["1"] = false;
            }
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
