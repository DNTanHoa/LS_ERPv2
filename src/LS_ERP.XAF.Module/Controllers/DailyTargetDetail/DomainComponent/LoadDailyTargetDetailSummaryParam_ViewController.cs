using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadDailyTargetDetailSummaryParam_ViewController : ViewController
    {
        public LoadDailyTargetDetailSummaryParam_ViewController()
        {
            InitializeComponent();
            SimpleAction searchDailyTargetAction = new SimpleAction(this, "SearchDailyTargetDetailSummary", PredefinedCategory.Unspecified);
            searchDailyTargetAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchDailyTargetAction.Caption = "Search (Ctrl + L)";
            searchDailyTargetAction.TargetObjectType = typeof(DailyTargetDetailSummary);
            searchDailyTargetAction.TargetViewType = ViewType.DetailView;
            searchDailyTargetAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchDailyTargetAction.Execute += SearchDailyTargetAction_Execute;
        }

        private void SearchDailyTargetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as DailyTargetDetailSummary;

            if (searchParam != null)
            {
                var dailyTargetDetailsService = new DailyTargetDetailService();

                var dailyTargetDetails = dailyTargetDetailsService.GetDailyTargetDetailReport(searchParam.Customer?.ID, searchParam.Style, searchParam.FromDate, searchParam.ToDate);
                searchParam.DailyTargetDetailReports = dailyTargetDetails.Result.Data.ToList();

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
