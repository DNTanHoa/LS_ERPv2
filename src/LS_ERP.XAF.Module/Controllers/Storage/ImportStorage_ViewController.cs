using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.IO;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportStorage_ViewController : ViewController
    {
        public ImportStorage_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importStorageDetail = new PopupWindowShowAction(this, "ImportStorageDetail", PredefinedCategory.Unspecified);
            importStorageDetail.ImageName = "Import";
            importStorageDetail.Caption = "Import";
            importStorageDetail.TargetObjectType = typeof(Storage);
            importStorageDetail.TargetViewType = ViewType.Any;
            importStorageDetail.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importStorageDetail.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            importStorageDetail.CustomizePopupWindowParams += ImportStorageDetail_CustomizePopupWindowParams;
            importStorageDetail.Execute += ImportStorageDetail_Execute;
        }

        private void ImportStorageDetail_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as ImportStorageParam;
            var service = new StorageService();
            var messageOptions = new MessageOptions();

            var request = new BulkStorageRequest()
            {
                CustomerID = importParam.Customer?.ID,
                StorageCode = importParam.Storage?.Code,
                FileName = Path.GetFileName(importParam.FilePath),
                UserName = SecuritySystem.CurrentUserName,
                Data = importParam.Data,
                ProductionMethodCode = importParam.ProductionMethod?.Code,
                Output = importParam.Output
            };

            // import input
            var importResponse = service.Bulk(request).Result;

            if (importResponse != null)
            {
                if (importResponse.Success)
                {
                    messageOptions = Message.GetMessageOptions("Action successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Message, "Error",
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

        private void ImportStorageDetail_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ImportStorageParam()
            {
                Storage = objectSpace.GetObjects<Storage>().FirstOrDefault(),
                ProductionMethod = objectSpace.GetObjects<PriceTerm>().FirstOrDefault(x => x.Code == "CMT"),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.Maximized = true;
            e.DialogController.SaveOnAccept = false;
            e.View = view;
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
