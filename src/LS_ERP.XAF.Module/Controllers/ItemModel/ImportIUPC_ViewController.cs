using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportIUPC_ViewController : ViewController
    {
        public ImportIUPC_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importUPC = new PopupWindowShowAction(this, "ImportUPC", PredefinedCategory.Unspecified);
            importUPC.ImageName = "Import";
            importUPC.Caption = "Import UPC";
            importUPC.TargetObjectType = typeof(ItemModel);
            importUPC.TargetViewType = ViewType.ListView;
            importUPC.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importUPC.CustomizePopupWindowParams += ImportUPC_CustomizePopupWindowParams;
            importUPC.Execute += ImportUPC_Execute;
        }

        private void ImportUPC_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as UPCImportParam;
            var service = new ItemModelService();
            var request = new ImportUPCRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.FilePath,
                UserName = SecuritySystem.CurrentUserName

            };
            var importResponse = service.ImportItemModel(request).Result;
            var messageOptions = new MessageOptions();

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);
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

            ObjectSpace.CommitChanges();
            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        private void ImportUPC_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new UPCImportParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
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
