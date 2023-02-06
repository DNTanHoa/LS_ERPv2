using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportUPCPopupAction_ViewController : ViewController
    {
        public ImportUPCPopupAction_ViewController()
        {
            InitializeComponent();
            
            SimpleAction browserImportUPC = new SimpleAction(this, "BrowserUPCImportFile", PredefinedCategory.Unspecified);
            browserImportUPC.ImageName = "Open";
            browserImportUPC.Caption = "Browser";
            browserImportUPC.TargetObjectType = typeof(UPCImportParam);
            browserImportUPC.TargetViewType = ViewType.DetailView;
            browserImportUPC.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportUPC.Execute += BrowserImportUPC_Execute;
        }

        public virtual void BrowserImportUPC_Execute(object sender, SimpleActionExecuteEventArgs e)
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
