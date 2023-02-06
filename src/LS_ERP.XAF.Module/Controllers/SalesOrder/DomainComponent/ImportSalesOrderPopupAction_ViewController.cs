using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Helpers;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportSalesOrderPopupAction_ViewController : ViewController
    {
        public ImportSalesOrderPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportSalesOrder = new SimpleAction(this, "BrowserSalesOrderImportFile", PredefinedCategory.Unspecified);
            browserImportSalesOrder.ImageName = "Open";
            browserImportSalesOrder.Caption = "Browser";
            browserImportSalesOrder.TargetObjectType = typeof(SalesOrderImportParam);
            browserImportSalesOrder.TargetViewType = ViewType.DetailView;
            browserImportSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportSalesOrder.Execute += BrowserImportSalesOrder_Execute;

            PopupWindowShowAction compareSalesOrderData = new PopupWindowShowAction(this, "CompareSalesOrderData", PredefinedCategory.Unspecified);
            compareSalesOrderData.Caption = "Compare";
            compareSalesOrderData.ImageName = "ShowCompactFormPivotTable";
            compareSalesOrderData.TargetObjectType = typeof(SalesOrderImportParam);
            compareSalesOrderData.TargetViewType = ViewType.DetailView;
            compareSalesOrderData.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            compareSalesOrderData.CustomizePopupWindowParams += CompareSalesOrder_CustomizePopupWindowParams;
            compareSalesOrderData.Execute += CompareSalesOrderData_Execute;
        }

        private void CompareSalesOrderData_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var compareData = e.PopupWindowViewCurrentObject as SalesOrderCompareParam;
            var importParam = e.CurrentObject as SalesOrderImportParam;
            if (importParam.GroupCompare == null)
            {
                importParam.GroupCompare = new Dtos.SalesOrder.GroupCompareDto();
            }
            importParam.GroupCompare.CompareItemStyles = compareData.Compare;
            importParam.GroupCompare.SalesOrders = compareData.SalesOrders;
            importParam.GroupCompare.FileName = compareData.FileName;
            importParam.GroupCompare.FilePath = compareData.FilePath;
        }

        private void CompareSalesOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var importParam = e.Action.SelectionContext.CurrentObject as SalesOrderImportParam;
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
                UserName = SecuritySystem.CurrentUserName
            };
            var compare = service.CompareSaleOrder(request).Result;

            if (compare != null)
            {
                if (compare.Result.Code == "000")
                {
                    //messageOptions = Message.GetMessageOptions(compare.Result.Message, "Success",
                    //    InformationType.Success, null, 5000);
                    if (compare.Data != null)
                    {
                        var objectSpace = this.ObjectSpace;
                        var model = new SalesOrderCompareParam()
                        {
                            Compare = compare.Data.CompareItemStyles,
                            SalesOrders = compare.Data.SalesOrders,
                            FileName = compare.Data.FileName,
                            FilePath = compare.Data.FilePath
                        };
                        var view = Application.CreateDetailView(objectSpace, model, false);
                        e.DialogController.SaveOnAccept = false;
                        e.View = view;
                    }
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

            //var objectSpace = this.ObjectSpace;
            //var model = new SalesOrderImportParam()
            //{
            //    Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            //};
            //var view = Application.CreateDetailView(objectSpace, model, false);
            //e.DialogController.SaveOnAccept = false;
            //e.View = view;
        }

        public virtual void BrowserImportSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

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
