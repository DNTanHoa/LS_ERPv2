using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UploadDocumentAction_ViewController : ViewController
    {
        public UploadDocumentAction_ViewController()
        {
            InitializeComponent();
            SimpleAction browserImportSalesOrder = new SimpleAction(this, "BrowserInvoiceDocumentFile", PredefinedCategory.Unspecified);
            browserImportSalesOrder.ImageName = "Open";
            browserImportSalesOrder.Caption = "Browser";
            browserImportSalesOrder.TargetObjectType = typeof(InvoiceDocumentParam);
            browserImportSalesOrder.TargetViewType = ViewType.DetailView;
            browserImportSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportSalesOrder.Execute += BrowserUploadDocumentInvoice_Execute;
        }

        public virtual void BrowserUploadDocumentInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
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
