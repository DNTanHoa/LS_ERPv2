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
    public partial class ImportLoadingPlanAction_ViewController : ObjectViewController<DetailView, LoadingPlanSearchParam>
    {
        public ImportLoadingPlanAction_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupLoadingPlanImportAction = new PopupWindowShowAction(this, "PopupLoadingPlanImportAction", PredefinedCategory.Unspecified);
            popupLoadingPlanImportAction.ImageName = "Import";
            popupLoadingPlanImportAction.Caption = "Import (Ctrl + I)";
            popupLoadingPlanImportAction.TargetObjectType = typeof(LoadingPlanSearchParam);
            popupLoadingPlanImportAction.TargetViewType = ViewType.DetailView;
            popupLoadingPlanImportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupLoadingPlanImportAction.Shortcut = "CtrlI";

            popupLoadingPlanImportAction.CustomizePopupWindowParams +=
                PopupLoadingPlanImportAction_CustomizePopupWindowParams;
            popupLoadingPlanImportAction.Execute += PopupLoadingPlanImportAction_Execute;
        }
        private void PopupLoadingPlanImportAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new LoadingPlanImportParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.Maximized=true;  
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        private void PopupLoadingPlanImportAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as LoadingPlanImportParam;
            var service = new LoadingPlanService();
            var messageOptions = new MessageOptions();

            var request = new BulkLoadingPlanRequest()
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
