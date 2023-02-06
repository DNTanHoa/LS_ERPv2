using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MatchingShipment_ViewController : ObjectViewController<DetailView, PurchaseOrderSearchParam>
    {
        public MatchingShipment_ViewController()
        {
            InitializeComponent();

            //PopupWindowShowAction matchingPurchaseOrder = new PopupWindowShowAction(this, "MatchingShipmentPurchaseOrder", PredefinedCategory.Unspecified);
            //matchingPurchaseOrder.ImageName = "Import";
            //matchingPurchaseOrder.Caption = "Matching Shipment";
            //matchingPurchaseOrder.TargetObjectType = typeof(PurchaseOrderSearchParam);
            //matchingPurchaseOrder.TargetViewType = ViewType.DetailView;
            //matchingPurchaseOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            //matchingPurchaseOrder.CustomizePopupWindowParams += MatchingPurchaseOrder_CustomizePopupWindowParams;
            //matchingPurchaseOrder.Execute += MatchingShipment_Execute;
        }


        private void MatchingShipment_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as PurchaseOrderMatchingShipmentParam;
            var service = new PurchaseOrderService();
            var messageOptions = new MessageOptions();

            var request = new MatchingShipmentPurchaseOrderRequest()
            {
                CustomerID = importParam.Customer?.ID,
                UserName = SecuritySystem.CurrentUserName
            };

            var compare = service.MatchingShipmentPurchaseOrder(request).Result;

            if (compare != null)
            {
                if (compare.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(compare.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(compare.Result.Message, "Error",
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

        private void MatchingPurchaseOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PurchaseOrderMatchingShipmentParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            };
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
