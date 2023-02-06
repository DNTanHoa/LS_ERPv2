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
    public partial class LoadPurchaseOrderLineFromSearchParam_ViewController : ViewController
    {
        public LoadPurchaseOrderLineFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPurchaseOrderLineAction = new SimpleAction(this, "SearchPurchaseOrderLine",
                PredefinedCategory.Unspecified);
            searchPurchaseOrderLineAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPurchaseOrderLineAction.Caption = "Search (Ctrl + L)";
            searchPurchaseOrderLineAction.TargetObjectType = typeof(PurchaseOrderLineSearchParam);
            searchPurchaseOrderLineAction.TargetViewType = ViewType.DetailView;
            searchPurchaseOrderLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchPurchaseOrderLineAction.Shortcut = "CtrlL";

            searchPurchaseOrderLineAction.Execute += SearchPurchaseOrderLineAction_Execute;
        }

        private void SearchPurchaseOrderLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as PurchaseOrderLineSearchParam;
            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("([PurchaseOrder.CustomerID] = ? OR ?)" +
                   "AND ([PurchaseOrder.OrderDate] >= ?)" +
                   "AND ([PurchaseOrder.OrderDate] <= ?)" +
                   "AND (Contains([PurchaseOrder.Number],?) OR ?)",
                   searchParam.Customer?.ID, string.IsNullOrEmpty(searchParam.Customer?.ID), 
                   searchParam.FromDate, searchParam.ToDate,
                   searchParam.PurchaseNumber, string.IsNullOrEmpty(searchParam.PurchaseNumber));

                var purchaseOrderGroupLines = ObjectSpace.GetObjects<PurchaseOrderLine>(criteria);
                searchParam.PurchaseOrderLines = purchaseOrderGroupLines.ToList();

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
