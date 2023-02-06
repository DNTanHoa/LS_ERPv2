using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ScanResultDetailSummaryAction_ViewController : ObjectViewController<DetailView, ScanResultDetailSummary>
    {
        public ScanResultDetailSummaryAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadScanResultDetailAction = new SimpleAction(this, "LoadScanResultDetailAction", PredefinedCategory.Unspecified);
            loadScanResultDetailAction.ImageName = "Action_Search";
            loadScanResultDetailAction.Caption = "Search (Ctrl + L)";
            loadScanResultDetailAction.TargetObjectType = typeof(ScanResultDetailSummary);
            loadScanResultDetailAction.TargetViewType = ViewType.DetailView;
            loadScanResultDetailAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadScanResultDetailAction.Shortcut = "CtrlL";

            loadScanResultDetailAction.Execute += LoadScanResultDetailAction_Execute;
        }
        private void LoadScanResultDetailAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ScanResultDetailSummary;

            if (viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("[ScanResult.StartDate] >= ? AND [ScanResult.StartDate]< ?",
                    viewObject.FromDate.Date, viewObject.ToDate.Date.AddDays(1));
                var data = ObjectSpace.GetObjects<ScanResultDetail>(criteria);
                viewObject.Results = data.ToList();

                var summaries = new List<SummaryScanResultDetail>();
                var connectionString = Application.ConnectionString ?? string.Empty;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Company",viewObject?.Company?.Code ?? string.Empty),
                    new SqlParameter("@FromDate",viewObject?.FromDate.Date),
                    new SqlParameter("@ToDate",viewObject?.ToDate.Date)
                };

                DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_SelectSummaryScanResultDetail", parameters);
                foreach (DataRow dr in table.Rows)
                {
                    var summary = new SummaryScanResultDetail()
                    {
                        PurchaseOrderNumber = dr["PONumber"].ToString(),
                        LSStyle = dr["LSStyle"].ToString(),
                        ScanDate = (DateTime)dr["StartDate"],
                        OrderQuantity = !string.IsNullOrEmpty(dr["OrderQuantity"].ToString()) ? (decimal)dr["OrderQuantity"] : 0,
                        EntryQuantity = !string.IsNullOrEmpty(dr["TotalFound"].ToString()) ? (int)dr["TotalFound"] : 0,
                        TotalBox = !string.IsNullOrEmpty(dr["TotalBox"].ToString()) ? (int)dr["TotalBox"] : 0,
                        Percent = !string.IsNullOrEmpty(dr["Percent"].ToString()) ? (decimal)dr["Percent"] : 0,
                    };
                    
                    summaries.Add(summary);
                }
                viewObject.Summaries = new List<SummaryScanResultDetail>(summaries);
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
