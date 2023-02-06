using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportDailyTargetPopupAction_ViewController : ViewController
    {
        public ImportDailyTargetPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportDailyTarget = new SimpleAction(this, "BrowserDailyTargetImportFile", PredefinedCategory.Unspecified);
            browserImportDailyTarget.ImageName = "Open";
            browserImportDailyTarget.Caption = "Browser";
            browserImportDailyTarget.TargetObjectType = typeof(DailyTargetImportParam);
            browserImportDailyTarget.TargetViewType = ViewType.DetailView;
            browserImportDailyTarget.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportDailyTarget.Execute += BrowserImportDailyTarget_Execute;

            SimpleAction ImportDailyTarget = new SimpleAction(this, "ImportDailyTargetAction", PredefinedCategory.Unspecified);
            ImportDailyTarget.ImageName = "Import";
            ImportDailyTarget.Caption = "Import";
            ImportDailyTarget.TargetObjectType = typeof(DailyTargetImportParam);
            ImportDailyTarget.TargetViewType = ViewType.DetailView;
            ImportDailyTarget.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            ImportDailyTarget.Execute += ImportDailyTarget_Execute;
        }      

        public virtual void BrowserImportDailyTarget_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            
        }

        private void ImportDailyTarget_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as DailyTargetImportParam;
             MessageOptions message = null;

            if (viewObject != null)
            {
                var service = new DailyTargetService();
                var request = new ImportDailyTargetRequest()
                {
                    FilePath = viewObject.ImportFilePath,
                    UserName = SecuritySystem.CurrentUserName
                };
                var response = service.ImportDailyTarget(request).Result;
                if (response != null)
                {
                    viewObject.Data = response.Data;
                    View.Refresh();
                }
            }
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
