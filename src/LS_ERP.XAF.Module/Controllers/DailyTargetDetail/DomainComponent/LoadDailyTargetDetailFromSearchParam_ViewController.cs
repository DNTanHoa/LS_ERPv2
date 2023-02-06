using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadDailyTargetDetailFromSearchParam_ViewController : ViewController
    {
        public LoadDailyTargetDetailFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchDailyTargetAction = new SimpleAction(this, "SearchDailyTargetDetail", PredefinedCategory.Unspecified);
            searchDailyTargetAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchDailyTargetAction.Caption = "Search (Ctrl + L)";
            searchDailyTargetAction.TargetObjectType = typeof(DailyTargetDetailSearchParam);
            searchDailyTargetAction.TargetViewType = ViewType.DetailView;
            searchDailyTargetAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchDailyTargetAction.Execute += SearchDailyTargetAction_Execute;
        }

        private void SearchDailyTargetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as DailyTargetDetailSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([CustomerID] = ? OR ?) AND ([Operation] = ? OR ?) AND ([ProduceDate] >= ?) AND ([ProduceDate] <= ?))",
                    searchParam.Customer?.ID, string.IsNullOrEmpty(searchParam.Customer?.ID), searchParam.Operation, string.IsNullOrEmpty(searchParam.Operation?.Name), searchParam.FromDate, searchParam.ToDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Customer));
                var dailyTargetDetails = objectSpace.GetObjects<DailyTargetDetail> (criteria);
                searchParam.DailyTargetDetails = dailyTargetDetails.ToList();

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
