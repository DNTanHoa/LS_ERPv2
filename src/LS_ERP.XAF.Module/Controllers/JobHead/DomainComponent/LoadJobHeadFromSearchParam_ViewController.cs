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
    public partial class LoadJobHeadFromSearchParam_ViewController : ViewController
    {
        public LoadJobHeadFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchJobHeadAction = new SimpleAction(this, "SearchJobHead", PredefinedCategory.Unspecified);
            searchJobHeadAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchJobHeadAction.Caption = "Search (Ctrl + L)";
            searchJobHeadAction.TargetObjectType = typeof(JobHeadSearchParam);
            searchJobHeadAction.TargetViewType = ViewType.DetailView;
            searchJobHeadAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchJobHeadAction.Shortcut = "CtrlL";

            searchJobHeadAction.Execute += SearchJobHeadAction_Execute;
        }

        private void SearchJobHeadAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as JobHeadSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("((Contains([LSStyle],?) OR ? Is Null) AND ([RequestDueDate] <= ?) AND ([RequestDueDate] >= ?))",
                    searchParam.Style, searchParam.Style, searchParam.ToDate, searchParam.FromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(JobHead));
                var jobHeads = objectSpace.GetObjects<JobHead>(criteria);
                searchParam.JobHeads = jobHeads.ToList();

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
