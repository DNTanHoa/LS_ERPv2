using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.SalesContract;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdatePurchaseOrderNumberSalesContract_ViewController : ObjectViewController<DetailView, SalesContractSearchParam>
    {
        public UpdatePurchaseOrderNumberSalesContract_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction updateSalesOrder = new PopupWindowShowAction(this, "UpdatePurchaseOrderNumberSalesContract", PredefinedCategory.Unspecified);
            updateSalesOrder.ImageName = "Update";
            updateSalesOrder.Caption = "Update PO Number";
            //updateSalesOrder.TargetObjectType = typeof(SalesContractSearchParam);
            //updateSalesOrder.TargetViewType = ViewType.DetailView;
            updateSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //updateSalesOrder.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

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
            var salesContract = e.SelectedObjects;
            var service = new SalesContractService();

            var request = new UpdateSalesContractRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.ImportFilePath,
                //SalesContractID = salesContract.ID,
                UserName = SecuritySystem.CurrentUserName
            };

            var updateResponse = service.UpdatePurchaseOrderNumberSalesContract(request).Result;
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
