using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class LoadIssuedFromSearchParam_ViewController : ViewController
    {
        public LoadIssuedFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchIssuedAction = new SimpleAction(this, "SearchIssued", PredefinedCategory.Unspecified);
            searchIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchIssuedAction.Caption = "Search (Ctrl + L)";
            searchIssuedAction.TargetObjectType = typeof(IssuedSearchParam);
            searchIssuedAction.TargetViewType = ViewType.DetailView;
            searchIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchIssuedAction.Shortcut = "CtrlL";

            searchIssuedAction.Execute += SearchIssuedAction_Execute;
        }

        private void SearchIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as IssuedSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([IssuedDate] <= ?) AND ([IssuedDate] >= ?))",
                    searchParam.ToDate, searchParam.FromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Issued));
                var issueds = objectSpace.GetObjects<Issued>(criteria);
                searchParam.Issueds = issueds.ToList();

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
