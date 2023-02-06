using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MaterialRequestInit_ViewController : ViewController
    {
        public MaterialRequestInit_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(MaterialRequest);
            TargetViewType = ViewType.DetailView;

        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
            var materialRequest = View.CurrentObject as MaterialRequest;
            
            if(materialRequest != null)
            {
                materialRequest.RequestDate = DateTime.Now;
                materialRequest.DueDate = DateTime.Now;
                materialRequest.Storage = ObjectSpace.GetObjects<Storage>().FirstOrDefault();
                materialRequest.SetCreateAudit(SecuritySystem.CurrentUserName);
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
