using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DisableIssuedActions_ViewController : ViewController
    {
        public DisableIssuedActions_ViewController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.Id == "Issued_DetailView")
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
