using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.SalesContract;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateSalesContract_ViewController : ObjectViewController<ListView, SalesContract>
    {
        public UpdateSalesContract_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction updateSalesOrder = new PopupWindowShowAction(this, "UpdateSalesContract", PredefinedCategory.Unspecified);
            updateSalesOrder.ImageName = "Update";
            //updateSalesOrder.TargetObjectType = typeof(SalesContract);
            //updateSalesOrder.TargetViewType = ViewType.ListView;
            updateSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateSalesOrder.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            updateSalesOrder.CustomizePopupWindowParams += UpdateSalesContract_CustomizePopupWindowParams;
            updateSalesOrder.Execute += UpdateSalesContract_Execute;

        }

        private void UpdateSalesContract_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesContractImportParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        public virtual void UpdateSalesContract_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as SalesContractImportParam;
            var salesContract = e.CurrentObject as SalesContract;
            var service = new SalesContractService();

            var request = new UpdateSalesContractRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.ImportFilePath,
                SalesContractID = salesContract.ID,
                UserName = SecuritySystem.CurrentUserName
            };

            var updateResponse = service.UpdateSalesContract(request).Result;
            var messageOptions = new MessageOptions();

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
