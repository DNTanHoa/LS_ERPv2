using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class InitLoadingPlanDetail_ViewController : ObjectViewController<DetailView, LoadingPlan>
    {
        public InitLoadingPlanDetail_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(LoadingPlan);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var viewObject = View.CurrentObject as LoadingPlan;

            if (viewObject != null && viewObject.ID == 0)
            {
                viewObject.Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault();
                viewObject.SetCreateAudit(SecuritySystem.CurrentUserName);
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
