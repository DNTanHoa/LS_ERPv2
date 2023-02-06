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
    public partial class UpdateSampleQuantity_ViewController : ObjectViewController<ListView, SalesOrder>
    {
        public UpdateSampleQuantity_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction updateSalesOrderSampleQuantity = new PopupWindowShowAction(this,
                "UpdateSampleQuantitySalesOrder", PredefinedCategory.Unspecified);
            updateSalesOrderSampleQuantity.ImageName = "Import";
            updateSalesOrderSampleQuantity.Caption = "Update Sample Quantity";
            //updateSalesOrderSimpleQuantity.TargetObjectType = typeof(SalesOrder);
            //updateSalesOrderSimpleQuantity.TargetViewType = ViewType.ListView;
            updateSalesOrderSampleQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;


            updateSalesOrderSampleQuantity.CustomizePopupWindowParams +=
                UpdateSalesOrderSimpleQuantity_CustomizePopupWindowParams;
            updateSalesOrderSampleQuantity.Execute += UpdateSalesOrderSimpleQuantity_Execute;
        }

        private void UpdateSalesOrderSimpleQuantity_Execute(object sender,
            PopupWindowShowActionExecuteEventArgs e)
        {
            var updateParam = e.PopupWindowView.CurrentObject as SalesOrderUpdateSampleQuantityParam;
            var service = new SalesOrderService();
            var messageOptions = new MessageOptions();

            var request = new UpdateSampleQuantityRequest()
            {
                CustomerID = updateParam.Customer?.ID,
                FilePath = updateParam.File,
                UserName = SecuritySystem.CurrentUserName
            };
            var updateResponse = service.UpdateSampleQuantitySaleOrder(request).Result;

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

        private void UpdateSalesOrderSimpleQuantity_CustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderUpdateSampleQuantityParam()
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
