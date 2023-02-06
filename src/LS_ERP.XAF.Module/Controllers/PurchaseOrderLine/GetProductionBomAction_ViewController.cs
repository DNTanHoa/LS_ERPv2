using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetProductionBomAction_ViewController : ViewController
    {
        public GetProductionBomAction_ViewController()
        {
            InitializeComponent();
            SimpleAction searchItemStyleForPurchaseOrderAction = new SimpleAction(this, "SearchItemStyleForPurchaseOrder", PredefinedCategory.Unspecified);
            searchItemStyleForPurchaseOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleForPurchaseOrderAction.Caption = "Search (Ctrl + L)";
            searchItemStyleForPurchaseOrderAction.TargetObjectType = typeof(PurchaseProductionBOM);
            searchItemStyleForPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            searchItemStyleForPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemStyleForPurchaseOrderAction.Shortcut = "CtrlL";

            searchItemStyleForPurchaseOrderAction.Execute += SearchItemStyleForPurchaseOrderAction_Execute;

            SimpleAction loadProductionBomForPurchaseOrderAction = new SimpleAction(this, "LoadProductionBomForPurchaseOrder", PredefinedCategory.Unspecified);
            loadProductionBomForPurchaseOrderAction.ImageName = "RotateCounterclockwise";
            loadProductionBomForPurchaseOrderAction.Caption = "Load Bom (Shift + B)";
            loadProductionBomForPurchaseOrderAction.TargetObjectType = typeof(PurchaseProductionBOM);
            loadProductionBomForPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            loadProductionBomForPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadProductionBomForPurchaseOrderAction.Shortcut = "ShiftB";

            loadProductionBomForPurchaseOrderAction.Execute += LoadProductionBomForPurchaseOrderAction_Execute;
        }

        private void LoadProductionBomForPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as PurchaseProductionBOM;
            var objectSpace = Application.CreateObjectSpace(typeof(ProductionBOM));
            var productionBomIDs = viewObject.ReservationEntries?.Select(x => x.ProductionBOMID);

            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("ItemStyles") as ListPropertyEditor;
            if (listPropertyEditor != null)
            {
                var selectedStyles = listPropertyEditor.ListView.SelectedObjects.Cast<ItemStyle>().ToList();
                string criteriaString = string.Empty;

                if (productionBomIDs.Any())
                {
                    criteriaString = "VendorID = ? AND RemainQuantity > 0 AND " +
                        "ItemStyleNumber IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.Number + "'")) + ") AND " +
                        "NOT (ID IN (" + string.Join(',', productionBomIDs)  + "))";
                }
                else
                {
                    criteriaString = "VendorID = ? AND RemainQuantity > 0 AND " +
                        "ItemStyleNumber IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.Number + "'")) + ")";
                }

                var criteria = CriteriaOperator.Parse(criteriaString, viewObject.VendorID);
                var productionBOMs = ObjectSpace.GetObjects<ProductionBOM>(criteria);
                viewObject.ProductionBOMs = productionBOMs.ToList();
            }
            View.Refresh();
        }

        private void SearchItemStyleForPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as PurchaseProductionBOM;
            var criteria = CriteriaOperator.Parse("(Contains(LSStyle,?) AND ?) OR " +
                "(Contains(ContractNo,?) AND ?)", 
                viewObject.Style, !string.IsNullOrEmpty(viewObject.Style), 
                viewObject.ContractNumber, !string.IsNullOrEmpty(viewObject.ContractNumber));
            var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria);
            viewObject.ItemStyles = itemStyles.ToList();
            View.Refresh();
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
