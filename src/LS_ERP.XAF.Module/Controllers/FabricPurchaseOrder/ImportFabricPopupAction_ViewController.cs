using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportFabricPopupAction_ViewController : ObjectViewController<DetailView, FabricPurchaseOrderImportParam>
    {
        public ImportFabricPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportSalesOrder = new SimpleAction(this, "BrowserFabricPurchaseOrderImportFile", PredefinedCategory.Unspecified);
            browserImportSalesOrder.ImageName = "Open";
            browserImportSalesOrder.Caption = "Browser";
            browserImportSalesOrder.TargetObjectType = typeof(FabricPurchaseOrderImportParam);
            browserImportSalesOrder.TargetViewType = ViewType.DetailView;
            browserImportSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportSalesOrder.Execute += BrowserImportFabricPurchaseOrder_Execute;
        }
        public virtual void BrowserImportFabricPurchaseOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
