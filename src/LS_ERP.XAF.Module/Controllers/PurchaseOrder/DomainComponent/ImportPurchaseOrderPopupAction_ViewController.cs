using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ImportPurchaseOrderPopupAction_ViewController : ViewController
    {
        public ImportPurchaseOrderPopupAction_ViewController()
        {
            InitializeComponent();
            SimpleAction browserImportSalesOrder = new SimpleAction(this, "BrowserPurchaseOrderImportFile", PredefinedCategory.Unspecified);
            browserImportSalesOrder.ImageName = "Open";
            browserImportSalesOrder.Caption = "Browser";
            browserImportSalesOrder.TargetObjectType = typeof(PurchaseOrderImportParam);
            browserImportSalesOrder.TargetViewType = ViewType.DetailView;
            browserImportSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportSalesOrder.Execute += BrowserImportPurchaseOrder_Execute;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }

        public virtual void BrowserImportPurchaseOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
