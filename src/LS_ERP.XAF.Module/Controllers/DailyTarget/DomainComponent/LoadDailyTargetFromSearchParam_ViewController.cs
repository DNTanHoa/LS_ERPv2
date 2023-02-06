using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadDailyTargetFromSearchParam_ViewController : ViewController
    {
        public LoadDailyTargetFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchDailyTargetAction = new SimpleAction(this, "SearchDailyTarget", PredefinedCategory.Unspecified);
            searchDailyTargetAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchDailyTargetAction.Caption = "Search (Ctrl + L)";
            searchDailyTargetAction.TargetObjectType = typeof(DailyTargetSearchParam);
            searchDailyTargetAction.TargetViewType = ViewType.DetailView;
            searchDailyTargetAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchDailyTargetAction.Execute += SearchDailyTargetAction_Execute;

        }

        private void SearchDailyTargetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as DailyTargetSearchParam;

            if (searchParam != null)
            {
                var dailyTargetservice = new DailyTargetService();

                var dailyTarget = dailyTargetservice.GetDailyTargetReport(searchParam.CompanyID, searchParam.ProduceDate);
                searchParam.DailyTargets = dailyTarget.Result.Data.ToList();
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
