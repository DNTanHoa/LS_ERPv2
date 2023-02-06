using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class InitIssuedDetail_ViewController : ViewController
    {
        public InitIssuedDetail_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(Issued);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            var viewObject = View.CurrentObject as Issued;

            if (viewObject != null &&
               string.IsNullOrEmpty(viewObject.Number))
            {
                viewObject.Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault();
                viewObject.Storage = ObjectSpace.GetObjects<Storage>().FirstOrDefault();
                viewObject.IssuedBy = SecuritySystem.CurrentUserName;
                viewObject.IssuedDate = DateTime.Now;
                viewObject.SetCreateAudit(SecuritySystem.CurrentUserName);
                viewObject.ReceivedDate = DateTime.Now;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
