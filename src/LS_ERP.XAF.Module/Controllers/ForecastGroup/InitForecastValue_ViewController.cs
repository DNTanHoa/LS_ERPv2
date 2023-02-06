using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class InitForecastValue_ViewController : ViewController
    {
        public InitForecastValue_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(ForecastGroup);
            TargetViewType = ViewType.DetailView;
        }
        
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            var viewObject = View.CurrentObject as ForecastGroup;

            if(viewObject != null)
            {
                if(viewObject.Customer == null)
                {
                    viewObject.Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault();
                }
                if (string.IsNullOrEmpty(viewObject.CreatedBy))
                {
                    viewObject.SetCreateAudit(SecuritySystem.CurrentUserName);
                }
                else
                {
                    viewObject.SetUpdateAudit(SecuritySystem.CurrentUserName);
                }
            }
        }
        
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
