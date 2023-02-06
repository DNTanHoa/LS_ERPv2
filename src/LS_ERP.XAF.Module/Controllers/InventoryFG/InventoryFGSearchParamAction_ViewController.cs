using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class InventoryFGSearchParamAction_ViewController : ViewController
    {
        public InventoryFGSearchParamAction_ViewController()
        {
            SimpleAction loadInventoryFGAction = new SimpleAction(this, "LoadInventoryFGAction", PredefinedCategory.Unspecified);
            loadInventoryFGAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadInventoryFGAction.Caption = "Search (Ctrl + L)";
            loadInventoryFGAction.TargetObjectType = typeof(InventoryFGSearchParam);
            loadInventoryFGAction.TargetViewType = ViewType.DetailView;
            loadInventoryFGAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadInventoryFGAction.Shortcut = "CtrlL";

            loadInventoryFGAction.Execute += LoadInventoryFGAction_Execute;

            SimpleAction exportInventoryFGAction = new SimpleAction(this, "ExportInventoryFGAction", PredefinedCategory.Unspecified);
            exportInventoryFGAction.ImageName = "";
            exportInventoryFGAction.Caption = "Export (Ctrl + E)";
            exportInventoryFGAction.TargetObjectType = typeof(InventoryFGSearchParam);
            exportInventoryFGAction.TargetViewType = ViewType.DetailView;
            exportInventoryFGAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportInventoryFGAction.Shortcut = "CtrlL";

            exportInventoryFGAction.Execute += ExportInventoryFGAction_Execute;

            InitializeComponent();
        }

        public virtual void ExportInventoryFGAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            
        }

        private void LoadInventoryFGAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as InventoryFGSearchParam;

            if(viewObject != null)
            {
                var inventorySummaries = new List<InventorySummaryFG>();
                var criteria = CriteriaOperator.Parse("Contains(CustomerStyle, ?) OR ?",
                    viewObject.Search, string.IsNullOrEmpty(viewObject.Search));
                var inventories = ObjectSpace.GetObjects<InventoryFG>(criteria);
                foreach(var data in inventories)
                {
                    var summary = new InventorySummaryFG();

                    criteria = CriteriaOperator.Parse("[InventoryFGID] = ?", data.ID);
                    var details = ObjectSpace.GetObjects<FinishGoodTransaction>(criteria);

                    summary.Inventory = data;
                    summary.ReceiptQuantity = details.Where(x => x.ScanResultDetailID > 0).Sum(x => x.Quantity);
                    summary.IssueQuantity = Math.Abs((decimal)details.Where(x => x.PackingListID > 0).Sum(x => x.Quantity));

                    inventorySummaries.Add(summary);
                }

                viewObject.Inventory = inventorySummaries.ToList();

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
