using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using System.Linq;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateShipQuantitySalesOrder_ViewController : ObjectViewController<ListView,SalesOrder>
    {
        public UpdateShipQuantitySalesOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction updateSalesOrderShipQuantity = new PopupWindowShowAction(this,
                "UpdateShipQuantitySalesOrder", PredefinedCategory.Unspecified);
            updateSalesOrderShipQuantity.ImageName = "Import";
            updateSalesOrderShipQuantity.Caption = "Update Ship Quantity";
            updateSalesOrderShipQuantity.TargetObjectType = typeof(SalesOrder);
            updateSalesOrderShipQuantity.TargetViewType = ViewType.ListView;
            updateSalesOrderShipQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;


            updateSalesOrderShipQuantity.CustomizePopupWindowParams +=
                UpdateSalesOrderShipQuantity_CustomizePopupWindowParams;
            updateSalesOrderShipQuantity.Execute += UpdateSalesOrderShipQuantity_Execute;
        }
        private void UpdateSalesOrderShipQuantity_Execute(object sender,
            PopupWindowShowActionExecuteEventArgs e)
        {
            var updateParam = e.PopupWindowView.CurrentObject as SalesOrderUpdateShipQuantityParam;
            var service = new SalesOrderService();
            var messageOptions = new MessageOptions();

            var request = new UpdateSalesOrderShipQuantityRequest()
            {
                CustomerID = updateParam.Customer?.ID,
                FilePath = updateParam.File,
                UserName = SecuritySystem.CurrentUserName
            };
            var updateResponse = service.UpdateShipQuantitySaleOrder(request).Result;

            if (updateResponse != null)
            {
                if (updateResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(updateResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(updateResponse.Result.Message, "Error",
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

        private void UpdateSalesOrderShipQuantity_CustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderUpdateShipQuantityParam()
            {
                Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault(),
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
