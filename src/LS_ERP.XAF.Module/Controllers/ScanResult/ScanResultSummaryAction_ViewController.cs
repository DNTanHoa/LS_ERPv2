using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ScanResultSummaryAction_ViewController 
        : ObjectViewController<DetailView, ScanResultSummary>
    {
        public ScanResultSummaryAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadScanResultAction = new SimpleAction(this, "LoadScanResultAction", PredefinedCategory.Unspecified);
            loadScanResultAction.ImageName = "Action_Search";
            loadScanResultAction.Caption = "Search (Ctrl + L)";
            loadScanResultAction.TargetObjectType = typeof(ScanResultSummary);
            loadScanResultAction.TargetViewType = ViewType.DetailView;
            loadScanResultAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadScanResultAction.Shortcut = "CtrlL";

            loadScanResultAction.Execute += LoadScanResultAction_Execute;

            SimpleAction updateScanResutlPOAction = new SimpleAction(this, "UpdateScanResutlPOAction", PredefinedCategory.Unspecified);
            updateScanResutlPOAction.ImageName = "BO_StateMachine";
            updateScanResutlPOAction.Caption = "Update PO";
            updateScanResutlPOAction.TargetObjectType = typeof(ScanResultSummary);
            updateScanResutlPOAction.TargetViewType = ViewType.DetailView;
            updateScanResutlPOAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateScanResutlPOAction.Shortcut = "CtrlL";

            updateScanResutlPOAction.Execute += UpdateScanResutlPOAction_Execute;

            
        }

        

        private void UpdateScanResutlPOAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ScanResultSummary;

            if (viewObject != null)
            {
                var serivce = new ScanResultService();

                serivce.UpdatePurchaseOrderNumber();

                var response = serivce.GetResultSummary(new GetScanResultSummaryRequest()
                {
                    Company = viewObject.Company?.Code,
                    SummaryDate = viewObject.Date,
                }).Result;

                if (response != null)
                {
                    if (response.Data != null &&
                       response.Data.Any())
                    {
                        viewObject.Summaries = response.Data
                            .Select(x => new ScanResultSummaryDetail()
                            {
                                PurchaseOrderNumber = x.PONumber,
                                LSStyle = x.LSStyle,
                                OrderQuantity = x.OrderQuantity ?? 0,
                                EntryQuantity = x.TotalFound ?? 0,
                                Percent = (x.Percent ?? 0)
                            }).ToList();
                    }
                }

                View.Refresh();
            }
        }

        private void LoadScanResultAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ScanResultSummary;

            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("[ScanResult.StartDate] >= ? AND [ScanResult.StartDate]<= ?",
                    viewObject.Date.Date, viewObject.Date.Date.AddDays(1));
                var data = ObjectSpace.GetObjects<ScanResultDetail>(criteria);
                viewObject.Results = data.ToList();

                var serivce = new ScanResultService();

                var response = serivce.GetResultSummary(new GetScanResultSummaryRequest()
                {
                    Company = viewObject.Company?.Code,
                    SummaryDate = viewObject.Date,
                }).Result;

                if(response != null)
                {
                    if(response.Data != null &&
                       response.Data.Any())
                    {
                        viewObject.Summaries = response.Data
                            .Select(x => new ScanResultSummaryDetail()
                            {
                                PurchaseOrderNumber = x.PONumber,
                                LSStyle = x.LSStyle,
                                OrderQuantity = x.OrderQuantity ?? 0,
                                EntryQuantity = x.TotalFound ?? 0,
                                TotalBox = x.TotalBox ?? 0,
                                Percent = x.Percent ?? 0
                            }).ToList();
                    }
                    else
                    {
                        viewObject.Summaries = new List<ScanResultSummaryDetail>();
                    }
                }

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
