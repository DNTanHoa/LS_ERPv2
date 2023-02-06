using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportLoadingPlanPopupAction_ViewController : ObjectViewController<DetailView, LoadingPlanImportParam>
    {
        public ImportLoadingPlanPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportLoadingPlan = new SimpleAction(this, "BrowserImportLoadingPlan", PredefinedCategory.Unspecified);
            browserImportLoadingPlan.ImageName = "Open";
            browserImportLoadingPlan.Caption = "Browser";
            browserImportLoadingPlan.TargetObjectType = typeof(LoadingPlanImportParam);
            browserImportLoadingPlan.TargetViewType = ViewType.DetailView;
            browserImportLoadingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportLoadingPlan.Execute += BrowserImportLoadingPlan_Execute;

            SimpleAction importLoadingPlan = new SimpleAction(this, "ImportLoadingPlan", PredefinedCategory.Unspecified);
            importLoadingPlan.ImageName = "Import";
            importLoadingPlan.Caption = "Import";
            importLoadingPlan.TargetObjectType = typeof(LoadingPlanImportParam);
            importLoadingPlan.TargetViewType = ViewType.DetailView;
            importLoadingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importLoadingPlan.Execute += ImportLoadingPlan_Execute;
        }
        public virtual void BrowserImportLoadingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }
        private void ImportLoadingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as LoadingPlanImportParam;
            MessageOptions message = null;

            if (viewObject != null)
            {
                var service = new LoadingPlanService();

                var request = new ImportLoadingPlanRequest()
                {
                    FilePath = viewObject.ImportFilePath,
                    CustomerID = viewObject.Customer?.ID,
                    UserName = SecuritySystem.CurrentUserName
                };

                var response = service.Import(request).Result;

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
