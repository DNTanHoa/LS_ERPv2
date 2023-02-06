using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShipmentImportAction_ViewController : ObjectViewController<DetailView, ShipmentImportParam>
    {
        public ShipmentImportAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportShipment = new SimpleAction(this, "BrowserShipmentFile", PredefinedCategory.Unspecified);
            browserImportShipment.ImageName = "Open";
            browserImportShipment.Caption = "Browser";
            //browserImportShipment.TargetObjectType = typeof(InvoiceDocumentParam);
            //browserImportShipment.TargetViewType = ViewType.DetailView;
            browserImportShipment.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportShipment.Execute += BrowserShipmentImport_Execute;

        }

        public virtual void BrowserShipmentImport_Execute(object sender, SimpleActionExecuteEventArgs e)
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
