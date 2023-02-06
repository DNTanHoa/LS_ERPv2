using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportPurchaseOrder_ViewController : ObjectViewController<ListView, PurchaseReceivedDetails>
    {
        public ExportPurchaseOrder_ViewController()
        {
            InitializeComponent();

            SimpleAction exportPurchaseOrderAction = new SimpleAction(this, "ExportPurchaseOrder", PredefinedCategory.Unspecified);
            exportPurchaseOrderAction.ImageName = "Export";
            exportPurchaseOrderAction.Caption = "Export Received";
            exportPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //exportPurchaseOrderAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            exportPurchaseOrderAction.Shortcut = "CtrlE";
            exportPurchaseOrderAction.Execute += ExportPurchaseOrderAction_Execute;
        }

        public virtual void ExportPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
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
