using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadDailyTargetDetailSummaryByDate_ViewController : ViewController
    {
        public LoadDailyTargetDetailSummaryByDate_ViewController()
        {
            InitializeComponent();
            SimpleAction searchDailyTargetDetailAction = new SimpleAction(this, "SearchDailyTargetDetailSummaryByDate", PredefinedCategory.Unspecified);
            searchDailyTargetDetailAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchDailyTargetDetailAction.Caption = "Search (Ctrl + L)";
            searchDailyTargetDetailAction.TargetObjectType = typeof(DailyTargetDetailSummaryByDate);
            searchDailyTargetDetailAction.TargetViewType = ViewType.DetailView;
            searchDailyTargetDetailAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchDailyTargetDetailAction.Execute += SearchDailyTargetDetailAction_Execute;
  
        }

        private void SearchDailyTargetDetailAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as DailyTargetDetailSummaryByDate;

            if (searchParam != null)
            {
                var dailyTargetDetailsService = new DailyTargetDetailService();

                var dailyTargetDetails = dailyTargetDetailsService.GetDailyTargetDetailSummaryByDate(searchParam.Customer?.ID, searchParam.Operation?.ID, searchParam.ProduceDate);
                searchParam.ListDailyTargetDetailSummaryByDate = dailyTargetDetails.Result.Data.ToList();
                searchParam.ListDetail = dailyTargetDetails.Result.Data.ToList();

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
