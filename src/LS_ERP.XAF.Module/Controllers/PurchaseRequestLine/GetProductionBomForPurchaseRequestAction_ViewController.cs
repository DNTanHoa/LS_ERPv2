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
    public partial class GetProductionBomForPurchaseRequestAction_ViewController : ViewController
    {
        public GetProductionBomForPurchaseRequestAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleForPurchaseRequestAction = new SimpleAction(this, "SearchItemStyleForPurchaseRequest", PredefinedCategory.Unspecified);
            searchItemStyleForPurchaseRequestAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleForPurchaseRequestAction.Caption = "Search (Ctrl + L)";
            searchItemStyleForPurchaseRequestAction.TargetObjectType = typeof(RequestProductionBOM);
            searchItemStyleForPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            searchItemStyleForPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemStyleForPurchaseRequestAction.Shortcut = "CtrlL";

            searchItemStyleForPurchaseRequestAction.Execute += SearchItemStyleForPurchaseRequestAction_Execute;

            SimpleAction loadProductionBomForPurchaseRequestAction = new SimpleAction(this, "LoadProductionBomForPurchaseRequest", PredefinedCategory.Unspecified);
            loadProductionBomForPurchaseRequestAction.ImageName = "RotateCounterclockwise";
            loadProductionBomForPurchaseRequestAction.Caption = "Load Bom (Shift + B)";
            loadProductionBomForPurchaseRequestAction.TargetObjectType = typeof(RequestProductionBOM);
            loadProductionBomForPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            loadProductionBomForPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadProductionBomForPurchaseRequestAction.Shortcut = "ShiftB";

            loadProductionBomForPurchaseRequestAction.Execute += LoadProductionBomForPurchaseRequestAction_Execute;
        }

        private void LoadProductionBomForPurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as RequestProductionBOM;
            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("ItemStyles") as ListPropertyEditor;
            if (listPropertyEditor != null)
            {
                var selectedStyles = listPropertyEditor.ListView.SelectedObjects.Cast<ItemStyle>().ToList();
                var criteria = CriteriaOperator.Parse("ItemStyleNumber IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.Number + "'")) + ")");
                var productionBOMs = ObjectSpace.GetObjects<ProductionBOM>(criteria);
                viewObject.ProductionBOMs = productionBOMs.ToList();
            }
            View.Refresh();
        }

        private void SearchItemStyleForPurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as RequestProductionBOM;
            var criteria = CriteriaOperator.Parse("Contains(LSStyle,?)", viewObject.ItemStyle);
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
