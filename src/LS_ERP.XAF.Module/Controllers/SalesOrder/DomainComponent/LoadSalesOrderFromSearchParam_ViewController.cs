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
    public partial class LoadSalesOrderFromSearchParam_ViewController : ViewController
    {
        public LoadSalesOrderFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchSalesOrderAction = new SimpleAction(this, "SearchSalesOrder", PredefinedCategory.Unspecified);
            searchSalesOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchSalesOrderAction.Caption = "Search (Ctrl + L)";
            searchSalesOrderAction.TargetObjectType = typeof(SalesOrderSearchParam);
            searchSalesOrderAction.TargetViewType = ViewType.DetailView;
            searchSalesOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchSalesOrderAction.Shortcut = "CtrlL";

            SimpleAction newSalesOderAction = new SimpleAction(this, "NewSalesOrder", PredefinedCategory.Unspecified);
            newSalesOderAction.ImageName = "Action_New";
            newSalesOderAction.Caption = "New Sales Order";
            newSalesOderAction.TargetObjectType = typeof(SalesOrderSearchParam);
            newSalesOderAction.TargetViewType = ViewType.DetailView;
            newSalesOderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchSalesOrderAction.Execute += SearchSalesOrderAction_Execute;
            newSalesOderAction.Execute += NewSalesOderAction_Execute;
        }

        private void NewSalesOderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(SalesOrder));
            var model = new SalesOrder();
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, model, true);
        }

        private void SearchSalesOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as SalesOrderSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([Customer] = ? OR ? Is Null) AND ([OrderDate] <= ?) AND ([OrderDate] >= ?))",
                    searchParam.Customer, searchParam.Customer, searchParam.OrderToDate, searchParam.OrderFromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Customer));
                var salesOrders = objectSpace.GetObjects<SalesOrder>(criteria);
                searchParam.SalesOrders = salesOrders.ToList();

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
