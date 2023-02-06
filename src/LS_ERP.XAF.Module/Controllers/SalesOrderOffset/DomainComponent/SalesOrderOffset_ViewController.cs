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
    public partial class SalesOrderOffset_ViewController : ViewController
    {
        public SalesOrderOffset_ViewController()
        {
            InitializeComponent();

            SimpleAction searchSalesOrderOffsetAction = new SimpleAction(this, "SearchSalesOrderOffset", PredefinedCategory.Unspecified);
            searchSalesOrderOffsetAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchSalesOrderOffsetAction.Caption = "Search (Ctrl + L)";
            searchSalesOrderOffsetAction.TargetObjectType = typeof(SalesOrderOffsetSearchParam);
            searchSalesOrderOffsetAction.TargetViewType = ViewType.DetailView;
            searchSalesOrderOffsetAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchSalesOrderOffsetAction.Execute += SearchSalesOrderOffsetAction_Execute;
        }

        private void SearchSalesOrderOffsetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var param = View.CurrentObject as SalesOrderOffsetSearchParam;
            
            if(param != null)
            {
                var criteria = CriteriaOperator.Parse("(CustomerStyle = ? OR ?) AND CreatedAt >= ? AND CreatedAt <= ?",
                param.Style, string.IsNullOrEmpty(param.Style), param.FromDate, param.ToDate);
                param.SalesOrderOffsets = ObjectSpace.GetObjects<SalesOrderOffset>(criteria).ToList();
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
