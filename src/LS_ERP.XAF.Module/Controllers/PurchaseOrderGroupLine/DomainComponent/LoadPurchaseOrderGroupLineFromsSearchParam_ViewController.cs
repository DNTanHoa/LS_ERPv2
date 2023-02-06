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
    public partial class LoadPurchaseOrderGroupLineFromsSearchParam_ViewController : ViewController
    {
        public LoadPurchaseOrderGroupLineFromsSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPurchaseOrderGroupLineAction = new SimpleAction(this, "SearchPurchaseOrderGroupLine", 
                PredefinedCategory.Unspecified);
            searchPurchaseOrderGroupLineAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPurchaseOrderGroupLineAction.Caption = "Search (Ctrl + L)";
            searchPurchaseOrderGroupLineAction.TargetObjectType = typeof(PurchaseOrderGroupLineSearchParam);
            searchPurchaseOrderGroupLineAction.TargetViewType = ViewType.DetailView;
            searchPurchaseOrderGroupLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchPurchaseOrderGroupLineAction.Shortcut = "CtrlL";

            searchPurchaseOrderGroupLineAction.Execute += SearchPurchaseOrderGroupLineAction_Execute;
        }

        private void SearchPurchaseOrderGroupLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as PurchaseOrderGroupLineSearchParam;
            if(searchParam != null)
            {
                 var criteria = CriteriaOperator.Parse("([PurchaseOrder.CustomerID] = ? OR ?)" +
                    "AND ([PurchaseOrder.OrderDate] >= ?)" +
                    "AND ([PurchaseOrder.OrderDate] <= ?)" +
                    "AND (Contains([PurchaseOrder.Number],?) OR ?)" +
                    "AND ([PurchaseOrder.CompanyCode] = ? OR ?)",
                    searchParam.Customer?.ID, string.IsNullOrEmpty(searchParam.Customer?.ID), 
                    searchParam.FromDate, searchParam.ToDate,
                    searchParam.PurchaseNumber, string.IsNullOrEmpty(searchParam.PurchaseNumber),
                    searchParam.Company?.Code, string.IsNullOrEmpty(searchParam.Company?.Code));
                
                var purchaseOrderGroupLines = ObjectSpace.GetObjects<PurchaseOrderGroupLine>(criteria);
                searchParam.PurchaseOrderGroupLines = purchaseOrderGroupLines.ToList();

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
