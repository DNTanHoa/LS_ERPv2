using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadPurchaseOrderReportFromSearchParam_ViewController : ViewController
    {
        public LoadPurchaseOrderReportFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPurchaseOderReportAction = new SimpleAction(this, "SearchPurchaseOrderReport", PredefinedCategory.Unspecified);
            searchPurchaseOderReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPurchaseOderReportAction.Caption = "Search (Ctrl + L)";
            searchPurchaseOderReportAction.TargetObjectType = typeof(PurchaseOrderReportSearchParam);
            searchPurchaseOderReportAction.TargetViewType = ViewType.DetailView;
            searchPurchaseOderReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchPurchaseOderReportAction.Execute += SearchPurchaseOderReportAction_Execute;
        }

        private void SearchPurchaseOderReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as PurchaseOrderReportSearchParam;
            var purchaseOrderService = new PurchaseOrderService();
            if (searchParam != null)
            {

                var purchaseOrders = purchaseOrderService.GetPurchaseOrderReport(searchParam.Customer?.ID, searchParam.Vendor?.ID, (DateTime)searchParam.FromDate, (DateTime)searchParam.ToDate);
                searchParam.ReportDetails = purchaseOrders.Result.Data.ToList();   
                searchParam.ListCharts = purchaseOrders.Result.Data.ToList();
                searchParam.ListPivot = purchaseOrders.Result.Data.ToList();
                View.Refresh();
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
