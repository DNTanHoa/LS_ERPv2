using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportForecastAction_ViewController : ViewController
    {
        public ImportForecastAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportForecastGroup = new SimpleAction(this, "BrowserForecastGroupImportFile", PredefinedCategory.Unspecified);
            browserImportForecastGroup.ImageName = "Open";
            browserImportForecastGroup.Caption = "Browser";
            browserImportForecastGroup.TargetObjectType = typeof(ForecastGroupImportParam);
            browserImportForecastGroup.TargetViewType = ViewType.DetailView;
            browserImportForecastGroup.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportForecastGroup.Execute += BrowserImportForecastGroup_Execute;


            SimpleAction importForecastGroup = new SimpleAction(this, "ImportForecastGroupAction", PredefinedCategory.Unspecified);
            importForecastGroup.Caption = "Import";
            importForecastGroup.ImageName = "Import";
            importForecastGroup.TargetObjectType = typeof(ForecastGroupImportParam);
            importForecastGroup.TargetViewType = ViewType.DetailView;
            importForecastGroup.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importForecastGroup.Execute += ImportForecastGroup_Execute;

        }

        private void ImportForecastGroup_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var importParam = e.CurrentObject as ForecastGroupImportParam;
            var service = new ForecastGroupService();
            var request = new ImportForecastGroupRequest()
            {
                FilePath = importParam.FilePath,
                UserName = SecuritySystem.CurrentUserName,
                CustomerID = importParam.Customer?.ID
            };
            var importResponse = service.ImportForecast(request).Result;
            var messageOptions = new MessageOptions();

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);

                    importParam.Overalls = importResponse.Data;
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            View.Refresh();
            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        public virtual void BrowserImportForecastGroup_Execute(object sender, SimpleActionExecuteEventArgs e)
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
