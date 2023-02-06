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
    public partial class IssuedProductionBomAction_ViewController : ViewController
    {
        public IssuedProductionBomAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleForIssuedAction = new SimpleAction(this, "SearchItemStyleForIssued", PredefinedCategory.Unspecified);
            searchItemStyleForIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleForIssuedAction.Caption = "Search (Ctrl + L)";
            searchItemStyleForIssuedAction.TargetObjectType = typeof(IssuedCreateParam);
            searchItemStyleForIssuedAction.TargetViewType = ViewType.DetailView;
            searchItemStyleForIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemStyleForIssuedAction.Shortcut = "CtrlL";

            searchItemStyleForIssuedAction.Execute += SearchItemStyleForIssuedAction_Execute;

            SimpleAction loadProductionBomForIssuedAction = new SimpleAction(this, "LoadProductionBomForIssued", PredefinedCategory.Unspecified);
            loadProductionBomForIssuedAction.ImageName = "RotateCounterclockwise";
            loadProductionBomForIssuedAction.Caption = "Load Bom (Shift + B)";
            loadProductionBomForIssuedAction.TargetObjectType = typeof(IssuedCreateParam);
            loadProductionBomForIssuedAction.TargetViewType = ViewType.DetailView;
            loadProductionBomForIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadProductionBomForIssuedAction.Shortcut = "ShiftB";

            loadProductionBomForIssuedAction.Execute += LoadProductionBomForIssuedAction_Execute;
        }

        private void LoadProductionBomForIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as IssuedCreateParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("Styless") as ListPropertyEditor;
            if (listPropertyEditor != null)
            {
                var selectedStyles = listPropertyEditor.ListView.SelectedObjects.Cast<ItemStyle>().ToList();
                var criteria = CriteriaOperator.Parse("ItemStyleNumber IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.Number + "'")) + ")");
                var productionBOMs = ObjectSpace.GetObjects<ProductionBOM>(criteria);
                viewObject.ProductionBOMs = productionBOMs.ToList();
            }
            View.Refresh();
        }

        private void SearchItemStyleForIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as IssuedCreateParam;

            var criteria = CriteriaOperator.Parse("(Contains(LSStyle,?) AND ?) OR " +
                "(Contains(ContractNo,?) AND ?) AND" +
                "(Contains([SalesOrder.CustomerID],?) OR ?)",
                viewObject.StyleNumher, !string.IsNullOrEmpty(viewObject.StyleNumher),
                viewObject.ContractNo, !string.IsNullOrEmpty(viewObject.ContractNo),
                viewObject.CustomerID, string.IsNullOrEmpty(viewObject.CustomerID));
            
            var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria);
            viewObject.Styless = itemStyles.ToList();
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
