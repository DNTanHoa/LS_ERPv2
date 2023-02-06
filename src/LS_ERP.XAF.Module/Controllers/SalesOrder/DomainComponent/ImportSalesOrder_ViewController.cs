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
    public partial class ImportSalesOrder_ViewController : ViewController
    {
        public ImportSalesOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importSalesOrder = new PopupWindowShowAction(this, "ImportSalesOrder", PredefinedCategory.Unspecified);
            importSalesOrder.ImageName = "Import";
            importSalesOrder.TargetObjectType = typeof(SalesOrderSearchParam);
            importSalesOrder.TargetViewType = ViewType.DetailView;
            importSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importSalesOrder.CustomizePopupWindowParams += ImportSalesOrder_CustomizePopupWindowParams;
            importSalesOrder.Execute += ImportSalesOrder_Execute;
        }

        private void ImportSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as SalesOrderImportParam;
            var service = new SalesOrderService();
            var messageOptions = new MessageOptions();

            var request = new ImportSalesOrderRequest()
            {
                CustomerID = importParam.Customer?.ID,
                BrandCode = importParam.Brand?.Code,
                Style = importParam.Style,
                ConfirmDate = importParam.ConfirmDate,
                IsUpdate = importParam.IsUpdate,
                FilePath = importParam.ImportFilePath,
                GroupCompare = importParam.GroupCompare,
                UserName = SecuritySystem.CurrentUserName
            };


            if (request.GroupCompare?.CompareItemStyles != null && request.GroupCompare.CompareItemStyles.Count > 0)
            {
                request.IsSaveCompare = true;
                var compare = service.SaveCompareSaleOrder(request).Result;

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
            }
            else
            {
                var importResponse = service.ImportSaleOrder(request).Result;

                if (importResponse != null)
                {
                    if (importResponse.Result.Code == "000")
                    {
                        messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
                }
            }


            ObjectSpace.CommitChanges();
            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        private void ImportSalesOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderImportParam()
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
