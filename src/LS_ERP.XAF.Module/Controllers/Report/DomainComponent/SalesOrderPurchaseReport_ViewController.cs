using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.Report.DomainComponent
{
    public partial class SalesOrderPurchaseReport_ViewController : ViewController
    {
        public SalesOrderPurchaseReport_ViewController()
        {
            InitializeComponent();

            SimpleAction searchSalesOrderPurchaseReportAction = new SimpleAction(this, "SearchSalesOrderPurchaseReport", PredefinedCategory.Unspecified);
            searchSalesOrderPurchaseReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchSalesOrderPurchaseReportAction.Caption = "Search (Ctrl + L)";
            searchSalesOrderPurchaseReportAction.TargetObjectType = typeof(SalesOrderPurchaseReportParam);
            searchSalesOrderPurchaseReportAction.TargetViewType = ViewType.DetailView;
            searchSalesOrderPurchaseReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchSalesOrderPurchaseReportAction.Execute += 
                SearchSalesOrderPurchaseReportAction_Execute;

        }

        private void SearchSalesOrderPurchaseReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as SalesOrderPurchaseReportParam;
            
            if (viewObject != null)
            {
                var service = new SalesOrderService();
                var response = service.GetPurchaseOrderForSalesOrder(viewObject.SalesOrderNo,
                    viewObject.Customer.ID, viewObject.Style).Result;
                if(response != null)
                {
                    viewObject.OrderDetails = response.Data;
                    View.Refresh();
                }
            }
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
