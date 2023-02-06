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
    public partial class UpdateQuantitySalesOrder_ViewController : ViewController
    {
        public UpdateQuantitySalesOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction updateSalesOrderQuantity = new PopupWindowShowAction(this, 
                "UpdateQuantitySalesOrder", PredefinedCategory.Unspecified);
            updateSalesOrderQuantity.ImageName = "Import";
            updateSalesOrderQuantity.Caption = "Update Quantity";
            updateSalesOrderQuantity.TargetObjectType = typeof(SalesOrder);
            updateSalesOrderQuantity.TargetViewType = ViewType.ListView;
            updateSalesOrderQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;


            updateSalesOrderQuantity.CustomizePopupWindowParams += 
                UpdateSalesOrderQuantity_CustomizePopupWindowParams;
            updateSalesOrderQuantity.Execute += UpdateSalesOrderQuantity_Execute;
        }

        private void UpdateSalesOrderQuantity_Execute(object sender, 
            PopupWindowShowActionExecuteEventArgs e)
        {
            var updateParam = e.PopupWindowView.CurrentObject as SalesOrderUpdateQuantityParam;
            var service = new SalesOrderService();
            var messageOptions = new MessageOptions();

            var request = new UpdateSalesOrderQuantityRequest()
            {
                CustomerID = updateParam.Customer?.ID,
                FilePath = updateParam.File,
                UserName = SecuritySystem.CurrentUserName
            };
            var updateResponse = service.UpdateQuantitySaleOrder(request).Result;

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

        private void UpdateSalesOrderQuantity_CustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderUpdateQuantityParam() 
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
