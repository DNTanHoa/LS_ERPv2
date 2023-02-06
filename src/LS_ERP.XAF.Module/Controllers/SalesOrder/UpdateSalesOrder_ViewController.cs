using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateSalesOrder_ViewController : ViewController
    {
        public UpdateSalesOrder_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction updateSalesOrderQuantity = new PopupWindowShowAction(this,
                "UpdateSalesOrder", PredefinedCategory.Unspecified);
            updateSalesOrderQuantity.ImageName = "Update";
            updateSalesOrderQuantity.Caption = "Update SO";
            updateSalesOrderQuantity.TargetObjectType = typeof(SalesOrder);
            updateSalesOrderQuantity.TargetViewType = ViewType.DetailView;
            updateSalesOrderQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;


            updateSalesOrderQuantity.CustomizePopupWindowParams +=
                UpdateSalesOrder_CustomizePopupWindowParams;
            updateSalesOrderQuantity.Execute += UpdateSalesOrder_Execute;
        }

        public virtual void UpdateSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var updateParam = e.PopupWindowView.CurrentObject as SalesOrderUpdateParam;
            var selected = View.CurrentObject as SalesOrder;
            var service = new SalesOrderService();
            var messageOptions = new MessageOptions();

            var request = new UpdateSalesOrderRequest()
            {
                CustomerID = selected.CustomerID,
                FilePath = updateParam.File,
                UserName = SecuritySystem.CurrentUserName,
                ID = selected.ID,
                BrandCode = selected.BrandCode,
                OrderDate = selected.OrderDate,
                ConfirmDate = selected.ConfirmDate,
                DivisionID = selected.DivisionID,
                PaymentTermCode = selected.PaymentTermCode,
                CurrencyID = selected.CurrencyID,
                PriceTermCode = selected.PriceTermCode,
                Year = selected.Year,
                SalesOrderStatusCode = selected.SalesOrderStatusCode,
                SalesOrderOrderTypeCode = selected.SalesOrderTypeCode

            };
            var updateResponse = service.UpdateSaleOrder(request).Result;

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

        private void UpdateSalesOrder_CustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderUpdateParam()
            {

            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
