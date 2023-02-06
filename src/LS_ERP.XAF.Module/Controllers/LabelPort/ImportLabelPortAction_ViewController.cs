using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportLabelPortAction_ViewController : ObjectViewController<DetailView, ImportLabelPortParam>
    {
        public ImportLabelPortAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportLabelPort = new SimpleAction(this, "BrowserLabelPortImportFile", PredefinedCategory.Unspecified);
            browserImportLabelPort.ImageName = "Open";
            browserImportLabelPort.Caption = "Browser";
            browserImportLabelPort.TargetObjectType = typeof(ImportLabelPortParam);
            browserImportLabelPort.TargetViewType = ViewType.DetailView;
            browserImportLabelPort.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportLabelPort.Execute += BrowserImportLabelPort_Execute;

        }

        public virtual void BrowserImportLabelPort_Execute(object sender, SimpleActionExecuteEventArgs e)
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
