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
    public partial class LoadFabricPurchaseSearchParam_ViewController : ViewController //ObjectViewController<DetailView, FabricPurchaseOrderSearchParam>
    {
        public LoadFabricPurchaseSearchParam_ViewController()
        {
            InitializeComponent();
            SimpleAction searchFabricPurchaseOrderAction = new SimpleAction(this, "SearchFabricPurchaseOrder", PredefinedCategory.Unspecified);
            searchFabricPurchaseOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchFabricPurchaseOrderAction.Caption = "Search (Ctrl + L)";
            searchFabricPurchaseOrderAction.TargetObjectType = typeof(FabricPurchaseOrderSearchParam);
            searchFabricPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            searchFabricPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchFabricPurchaseOrderAction.Shortcut = "CtrlL";

            searchFabricPurchaseOrderAction.Execute += SearchFabricPurchaseOrderActionAction_Execute;
        }

        private void SearchFabricPurchaseOrderActionAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as FabricPurchaseOrderSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([Customer] = ? OR ? Is Null) AND ([CreatedAt] <= ?) AND ([CreatedAt] >= ?))",
                    searchParam.Customer, searchParam.Customer, searchParam.OrderToDate, searchParam.OrderFromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Customer));
                var fabricPurchaseOrders = objectSpace.GetObjects<FabricPurchaseOrder>(criteria);
                searchParam.FabricPurchaseOrders = fabricPurchaseOrders.ToList();

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
