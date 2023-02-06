using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.FabricPurchaseOrder;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportFabricPurchaseOrder_ViewController : ObjectViewController<ListView, FabricPurchaseOrder>
    {
        public ImportFabricPurchaseOrder_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction popupFabricPurchaseOrder = new PopupWindowShowAction(this,
                "PopupFabricPurchaseOrder", PredefinedCategory.Unspecified);
            popupFabricPurchaseOrder.ImageName = "Import";
            popupFabricPurchaseOrder.Caption = "Import (Ctrl + I)";
            popupFabricPurchaseOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupFabricPurchaseOrder.Shortcut = "CtrlI";

            popupFabricPurchaseOrder.CustomizePopupWindowParams += PopupFabricPurchaseOrder_CustomizePopupWindowParams;
            popupFabricPurchaseOrder.Execute += PopupFabricPurchaseOrder_Execute;
        }

        private void PopupFabricPurchaseOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as FabricPurchaseOrderImportParam;
            var service = new FabricPurchaseOrderService();
            var messageOptions = new MessageOptions();


            var request = new ImportFabricPurchaseOrderRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.ImportFilePath,
                UserName = SecuritySystem.CurrentUserName,
                ProductionMethodCode = importParam.ProductionMethod?.Code
            };


            var compare = service.ImportFabricPurchaseOrder(request).Result;

            if (compare != null)
            {
                if (compare.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(compare.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                    //selected = updateResponse.Data;
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

        private void PopupFabricPurchaseOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new FabricPurchaseOrderImportParam();

            model.ProductionMethod = objectSpace.FirstOrDefault<PriceTerm>(x => x.Code == "FOB");

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = false;
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
