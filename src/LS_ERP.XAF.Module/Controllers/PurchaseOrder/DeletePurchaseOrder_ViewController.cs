using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DeletePurchaseOrder_ViewController : ViewController
    {
        public DeletePurchaseOrder_ViewController()
        {
            InitializeComponent();

            SimpleAction deletePurchaseOrderAction = new SimpleAction(this, "DeletePurchaseOrder", PredefinedCategory.Unspecified);
            deletePurchaseOrderAction.ImageName = "Delete";
            deletePurchaseOrderAction.Caption = "Delete";
            deletePurchaseOrderAction.TargetObjectType = typeof(PurchaseOrder);
            deletePurchaseOrderAction.TargetViewType = ViewType.Any;
            deletePurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deletePurchaseOrderAction.SelectionDependencyType =
                SelectionDependencyType.RequireSingleObject;
            deletePurchaseOrderAction.Shortcut = "CtrlE";

            deletePurchaseOrderAction.Execute += DeletePurchaseOrderAction_Execute;
        }

        private void DeletePurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var purchaseOrder = View.CurrentObject as PurchaseOrder;
            MessageOptions message = null;
            var service = new PurchaseOrderService();
            var request = new DeletePurchaseOrderRequest()
            {
                PurchaseOrderID = purchaseOrder.ID,
                Username = SecuritySystem.CurrentUserName,
            };

            var response = service.DeletePurchaseOrder(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                        null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                        null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

            View.Refresh();
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
