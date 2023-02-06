using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportPurchaseOrder_ViewController : ObjectViewController<DetailView, PurchaseOrderSearchParam>
    {
        public ImportPurchaseOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importPurchaseOrder = new PopupWindowShowAction(this, "ImportPurchaseOrder", PredefinedCategory.Unspecified);
            importPurchaseOrder.ImageName = "Import";
            importPurchaseOrder.Caption = "Import PO";
            importPurchaseOrder.TargetObjectType = typeof(PurchaseOrderSearchParam);
            importPurchaseOrder.TargetViewType = ViewType.DetailView;
            importPurchaseOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importPurchaseOrder.CustomizePopupWindowParams += ImportSalesOrder_CustomizePopupWindowParams;
            importPurchaseOrder.Execute += ImportSalesOrder_Execute;

        }

        private void ImportSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as PurchaseOrderImportParam;
            var service = new PurchaseOrderService();
            var messageOptions = new MessageOptions();

            var request = new ImportPurchaseOrderRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.ImportFilePath,
                UserName = SecuritySystem.CurrentUserName,
                Type = importParam.Type
            };

            var compare = service.ImportPurchaseOrder(request).Result;

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

        private void ImportSalesOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PurchaseOrderImportParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
                Type = TypeImportPurchaseOrder.TrimTracking
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
