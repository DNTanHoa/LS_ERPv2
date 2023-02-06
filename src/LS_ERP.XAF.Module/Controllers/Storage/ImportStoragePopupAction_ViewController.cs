using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportStoragePopupAction_ViewController : ViewController
    {
        public ImportStoragePopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportStorageDetail = new SimpleAction(this, "BrowserStorageDetailImportFile", PredefinedCategory.Unspecified);
            browserImportStorageDetail.ImageName = "Open";
            browserImportStorageDetail.Caption = "Browser";
            browserImportStorageDetail.TargetObjectType = typeof(ImportStorageParam);
            browserImportStorageDetail.TargetViewType = ViewType.DetailView;
            browserImportStorageDetail.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportStorageDetail.Execute += BrowserImportStorageDetail_Execute;

            SimpleAction ImportStorageDetail = new SimpleAction(this, "ImportStorageDetailAction", PredefinedCategory.Unspecified);
            ImportStorageDetail.ImageName = "Import";
            ImportStorageDetail.Caption = "Import";
            ImportStorageDetail.TargetObjectType = typeof(ImportStorageParam);
            ImportStorageDetail.TargetViewType = ViewType.DetailView;
            ImportStorageDetail.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            ImportStorageDetail.Execute += ImportStorageDetail_Execute;
        }

        private void ImportStorageDetail_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ImportStorageParam;
            MessageOptions messageOptions = new MessageOptions();

            if (viewObject != null)
            {
                var service = new StorageService();

                var request = new ImportStorageRequest()
                {
                    FilePath = viewObject.FilePath,
                    CustomerID = viewObject.Customer?.ID,
                    UserName = SecuritySystem.CurrentUserName,
                    ProductionMethodCode = viewObject.ProductionMethod?.Code,
                    Output = viewObject.Output,
                    StorageCode = viewObject.Storage?.Code
                };

                var response = service.Import(request).Result;

                if (response != null)
                {
                    //if (request.Output && response.Result.IsSuccess)
                    //{
                    //    viewObject.Data = response.Data;
                    //    messageOptions = Message.GetMessageOptions("Action successfully", "Success",
                    //        InformationType.Success, null, 5000);
                    //}
                    //else
                    if (response.Data != null)
                    {
                        viewObject.Data = response.Data;
                        messageOptions = Message.GetMessageOptions("Action successfully", "Success",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        messageOptions = Message.GetMessageOptions(response.Result.Message, "Error",
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
        }

        public virtual void BrowserImportStorageDetail_Execute(object sender, SimpleActionExecuteEventArgs e)
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
