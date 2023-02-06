using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class InitPackingListDetail_ViewController : ViewController
    {
        public InitPackingListDetail_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(PackingList);
            TargetViewType = ViewType.DetailView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            var viewObject = View.CurrentObject as PackingList;

            if (viewObject != null &&
               string.IsNullOrEmpty(viewObject.PackingListCode))
            {
                viewObject.Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault();
                viewObject.PackingListDate = DateTime.Now;
                viewObject.SetCreateAudit(SecuritySystem.CurrentUserName);
                viewObject.CompanyCode = "LS";
                viewObject.PackingListCode =
                    "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6);

            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
