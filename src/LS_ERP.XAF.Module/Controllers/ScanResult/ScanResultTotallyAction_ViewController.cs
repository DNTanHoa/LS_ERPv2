using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ScanResultTotallyAction_ViewController : ObjectViewController<DetailView, ScanResultTotally>
    {
        public ScanResultTotallyAction_ViewController()
        {
            InitializeComponent();

            SimpleAction scanResultTotallyAction = new SimpleAction(this, "ScanResultTotallyAction", PredefinedCategory.Unspecified);
            scanResultTotallyAction.ImageName = "Action_Search";
            scanResultTotallyAction.Caption = "Search (Ctrl + L)";
            scanResultTotallyAction.TargetObjectType = typeof(ScanResultTotally);
            scanResultTotallyAction.TargetViewType = ViewType.DetailView;
            scanResultTotallyAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            scanResultTotallyAction.Shortcut = "CtrlL";

            scanResultTotallyAction.Execute += ScanResultTotallyAction_Execute;
        }
        private void ScanResultTotallyAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ScanResultTotally;
            var summaries = new List<ScanResultTotal>();
            if (viewObject != null)
            {
                var connectionString = Application.ConnectionString ?? string.Empty;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@CompanyID",viewObject?.Company?.Code ?? string.Empty),
                    new SqlParameter("@FromDate",viewObject?.FromDate ?? DateTime.Now),
                    new SqlParameter("@ToDate",viewObject?.ToDate ??  DateTime.Now),
                    new SqlParameter("@Search",viewObject?.Search ?? string.Empty)
                };

                DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_LoadSummaryScanResult", parameters);
                
                foreach(DataRow dr in table.Rows)
                {
                    var scanResultTotal = new ScanResultTotal()
                    {
                        PurchaseOrderNumber = dr["PONumber"].ToString(),
                        LSStyle = dr["LSStyle"].ToString(),
                        OrderQuantity = (decimal)dr["OrderQuantity"],
                        EntryQuantity = (int)dr["EntryQuantity"],
                        Status = dr["Status"].ToString()
                    };

                    summaries.Add(scanResultTotal);
                }
                viewObject.Summaries = new List<ScanResultTotal>(summaries);
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
