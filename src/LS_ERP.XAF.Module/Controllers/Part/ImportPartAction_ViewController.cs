using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportPartAction_ViewController : ObjectViewController<DetailView, ImportPartParam>
    {
        public ImportPartAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportPart = new SimpleAction(this, "BrowserPartImportFile", PredefinedCategory.Unspecified);
            browserImportPart.ImageName = "Open";
            browserImportPart.Caption = "Browser";
            browserImportPart.TargetObjectType = typeof(ImportPartParam);
            browserImportPart.TargetViewType = ViewType.DetailView;
            browserImportPart.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportPart.Execute += BrowserImportPart_Execute;
        }

        public virtual void BrowserImportPart_Execute(object sender, SimpleActionExecuteEventArgs e)
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
