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
    public partial class UpdateCAC_CHD_EHDSalesOrder_ViewController : ObjectViewController<ListView, SalesOrder>
    {
        public UpdateCAC_CHD_EHDSalesOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction updateSalesOrderCAC_CHD_EHD = new PopupWindowShowAction(this,
                "UpdateCAC_CHD_EHDSalesOrder", PredefinedCategory.Unspecified);
            updateSalesOrderCAC_CHD_EHD.ImageName = "Import";
            updateSalesOrderCAC_CHD_EHD.Caption = "Update CAC/CHD/EHD";
            updateSalesOrderCAC_CHD_EHD.TargetObjectType = typeof(SalesOrder);
            updateSalesOrderCAC_CHD_EHD.TargetViewType = ViewType.ListView;
            updateSalesOrderCAC_CHD_EHD.PaintStyle = ActionItemPaintStyle.CaptionAndImage;


            updateSalesOrderCAC_CHD_EHD.CustomizePopupWindowParams +=
                UpdateSalesOrderCAC_CHD_EHD_CustomizePopupWindowParams;
            updateSalesOrderCAC_CHD_EHD.Execute += UpdateSalesOrderCAC_CHD_EHD_Execute;
        }
        private void UpdateSalesOrderCAC_CHD_EHD_Execute(object sender,
            PopupWindowShowActionExecuteEventArgs e)
        {
            var updateParam = e.PopupWindowView.CurrentObject as SalesOrderUpdateCAC_CHD_EHDParam;
            var service = new SalesOrderService();
            var messageOptions = new MessageOptions();

            var request = new UpdateSalesOrderCAC_CHD_EHDRequest()
            {
                CustomerID = updateParam.Customer?.ID,
                FilePath = updateParam.File,
                UserName = SecuritySystem.CurrentUserName
            };
            var updateResponse = service.UpdateCAC_CHD_EHDSaleOrder(request).Result;

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
        private void UpdateSalesOrderCAC_CHD_EHD_CustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderUpdateCAC_CHD_EHDParam()
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
