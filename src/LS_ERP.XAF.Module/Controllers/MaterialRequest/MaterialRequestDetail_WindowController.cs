using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MaterialRequestDetail_WindowController : WindowController
    {
        public MaterialRequestDetail_WindowController()
        {
            InitializeComponent();
            TargetWindowType = WindowType.Main;
        }

        void showNavigationItemController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Id == "request-entry")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(MaterialRequest));
                MaterialRequest newIssue = objectSpace.CreateObject<MaterialRequest>();
                newIssue.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();
                newIssue.RequestDate = DateTime.Now;
                newIssue.DueDate = DateTime.Now.AddDays(1);

                DetailView detailView = Application.CreateDetailView(objectSpace, newIssue);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ShowNavigationItemController showNavigationItemController = 
                Frame.GetController<ShowNavigationItemController>();
            showNavigationItemController.CustomShowNavigationItem += 
                showNavigationItemController_CustomShowNavigationItem;
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
