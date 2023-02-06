using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DeletePurchaseOrderGroupLine_ViewController : ViewController
    {
        public DeletePurchaseOrderGroupLine_ViewController()
        {
            InitializeComponent();

            SimpleAction deletePurchaseOrderGroupLineAction = new SimpleAction(this, "DeletePurchaseOrderGroupLine", PredefinedCategory.Unspecified);
            deletePurchaseOrderGroupLineAction.ImageName = "Delete";
            deletePurchaseOrderGroupLineAction.Caption = "Delete";
            deletePurchaseOrderGroupLineAction.TargetObjectType = typeof(PurchaseOrderGroupLine);
            deletePurchaseOrderGroupLineAction.TargetViewType = ViewType.Any;
            deletePurchaseOrderGroupLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deletePurchaseOrderGroupLineAction.SelectionDependencyType =
                SelectionDependencyType.RequireSingleObject;
            deletePurchaseOrderGroupLineAction.Shortcut = "CtrlE";

            deletePurchaseOrderGroupLineAction.Execute += DeletePurchaseOrderGroupLineAction_Execute;
        }

        private void DeletePurchaseOrderGroupLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            
        }

        protected override void OnActivated()
        {
            base.OnActivated();
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
