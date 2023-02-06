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
    public partial class LoadFabricRequestSearchParam_ViewController : ViewController
    {
        public LoadFabricRequestSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchFabricPurchaseOrderAction = new SimpleAction(this, "SearchFabricRequest", PredefinedCategory.Unspecified);
            searchFabricPurchaseOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchFabricPurchaseOrderAction.Caption = "Search (Ctrl + L)";
            searchFabricPurchaseOrderAction.TargetObjectType = typeof(FabricRequestSearchParam);
            searchFabricPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            searchFabricPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchFabricPurchaseOrderAction.Shortcut = "CtrlL";

            searchFabricPurchaseOrderAction.Execute += SearchFabricRequestAction_Execute;
        }

        private void SearchFabricRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as FabricRequestSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([Customer] = ? OR ? Is Null) AND ([CreatedAt] <= ?) AND ([CreatedAt] >= ?))",
                    searchParam.Customer, searchParam.Customer, searchParam.OrderToDate, searchParam.OrderFromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Customer));
                var fabricRequests = objectSpace.GetObjects<FabricRequest>(criteria);
                searchParam.FabricRequests = fabricRequests.ToList();

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
