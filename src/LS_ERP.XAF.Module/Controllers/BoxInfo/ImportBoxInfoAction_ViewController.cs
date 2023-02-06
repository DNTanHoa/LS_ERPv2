using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportBoxInfoAction_ViewController : ObjectViewController<DetailView, BoxInfoSearchParam>
    {
        public ImportBoxInfoAction_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupBoxInfoImportAction = new PopupWindowShowAction(this, "PopupBoxInfoImportAction", PredefinedCategory.Unspecified);
            popupBoxInfoImportAction.ImageName = "Import";
            popupBoxInfoImportAction.Caption = "Import (Ctrl + I)";
            popupBoxInfoImportAction.TargetObjectType = typeof(BoxInfoSearchParam);
            popupBoxInfoImportAction.TargetViewType = ViewType.DetailView;
            popupBoxInfoImportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupBoxInfoImportAction.Shortcut = "CtrlI";

            popupBoxInfoImportAction.CustomizePopupWindowParams +=
                PopupBoxInfoImportdAction_CustomizePopupWindowParams;
            popupBoxInfoImportAction.Execute += PopupBoxInfoImportAction_Execute;
        }
        private void PopupBoxInfoImportdAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new BoxInfoImportParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.Maximized = true;
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        private void PopupBoxInfoImportAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as BoxInfoImportParam;
            var service = new BoxInfoService();
            var messageOptions = new MessageOptions();

            var request = new BulkBoxInfoRequest()
            {
                CustomerID = viewObject.Customer?.ID,
                UserName = SecuritySystem.CurrentUserName,
                Data = viewObject.Data
            };
            var bulkResponse = service.Bulk(request).Result;

            if (bulkResponse != null)
            {
                if (bulkResponse.Success)
                {
                    messageOptions = Message.GetMessageOptions("Action successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(bulkResponse.Message, "Error",
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
