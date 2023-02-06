using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportBoxInfoPopupAction_ViewController : ObjectViewController<DetailView, BoxInfoImportParam>
    {
        public ImportBoxInfoPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportBoxInfo = new SimpleAction(this, "BrowserImportBoxInfo", PredefinedCategory.Unspecified);
            browserImportBoxInfo.ImageName = "Open";
            browserImportBoxInfo.Caption = "Browser";
            browserImportBoxInfo.TargetObjectType = typeof(BoxInfoImportParam);
            browserImportBoxInfo.TargetViewType = ViewType.DetailView;
            browserImportBoxInfo.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportBoxInfo.Execute += BrowserImportBoxInfo_Execute;

            SimpleAction importBoxInfo = new SimpleAction(this, "ImportBoxInfo", PredefinedCategory.Unspecified);
            importBoxInfo.ImageName = "Import";
            importBoxInfo.Caption = "Import";
            importBoxInfo.TargetObjectType = typeof(BoxInfoImportParam);
            importBoxInfo.TargetViewType = ViewType.DetailView;
            importBoxInfo.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importBoxInfo.Execute += ImportBoxInfo_Execute;
        }
        public virtual void BrowserImportBoxInfo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }
        private void ImportBoxInfo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as BoxInfoImportParam;
            MessageOptions message = null;

            if (viewObject != null)
            {
                var service = new BoxInfoService();

                var request = new ImportBoxInfoRequest()
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
