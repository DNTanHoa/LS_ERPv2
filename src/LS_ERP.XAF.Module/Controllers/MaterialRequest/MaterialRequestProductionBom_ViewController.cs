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
    public partial class MaterialRequestProductionBom_ViewController : ObjectViewController<DetailView, MaterialRequestFromProductionBom>
    {
        public MaterialRequestProductionBom_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleForMaterialRequestAction = new SimpleAction(this, "SearchItemStyleForMaterialRequest", PredefinedCategory.Unspecified);
            searchItemStyleForMaterialRequestAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleForMaterialRequestAction.Caption = "Search (Ctrl + L)";
            searchItemStyleForMaterialRequestAction.TargetObjectType = typeof(MaterialRequestFromProductionBom);
            searchItemStyleForMaterialRequestAction.TargetViewType = ViewType.DetailView;
            searchItemStyleForMaterialRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemStyleForMaterialRequestAction.Shortcut = "CtrlL";

            searchItemStyleForMaterialRequestAction.Execute += SearchItemStyleForMaterialRequestAction_Execute;

            SimpleAction loadProductionBomForMaterialRequestAction = new SimpleAction(this, "LoadProductionBomForMaterialRequest", PredefinedCategory.Unspecified);
            loadProductionBomForMaterialRequestAction.ImageName = "RotateCounterclockwise";
            loadProductionBomForMaterialRequestAction.Caption = "Load Bom (Shift + B)";
            loadProductionBomForMaterialRequestAction.TargetObjectType = typeof(MaterialRequestFromProductionBom);
            loadProductionBomForMaterialRequestAction.TargetViewType = ViewType.DetailView;
            loadProductionBomForMaterialRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadProductionBomForMaterialRequestAction.Shortcut = "ShiftB";

            loadProductionBomForMaterialRequestAction.Execute += LoadProductionBomForMaterialRequestAction_Execute;
        }

        private void LoadProductionBomForMaterialRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as MaterialRequestFromProductionBom;
            
            if(viewObject != null)
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("ItemStyles") as ListPropertyEditor;

                if(listPropertyEditor != null)
                {
                    var selectedStyles = listPropertyEditor.ListView.SelectedObjects.Cast<ItemStyle>().ToList();
                    string criteriaString =
                        "ItemStyleNumber IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.Number + "'")) + ")";
                    var criteria = CriteriaOperator.Parse(criteriaString);

                    var proBomPreviews = ObjectSpace.GetObjects<ProductionBOM>(criteria)
                        .Select(x => new MaterialRequestDetailPreview()
                        {
                            ItemCode = x.ItemCode,
                            ItemID = x.ItemID,
                            ItemName = x.ItemName,
                            ItemColorCode = x.ItemColorCode,
                            ItemColorName = x.ItemColorName,
                            Specify = x.Specify,
                            CustomerStyle = x.ItemStyle.CustomerStyle,
                            LSStyle = x.ItemStyle.LSStyle,
                            GarmentColorCode = x.ItemStyle.ColorCode,
                            GarmentColorName = x.ItemStyle.ColorName,
                            GarmentSize = string.IsNullOrEmpty(x.GarmentSize) ? x.JobHead?.GarmentSize : x.GarmentSize, /// For edit quantity size
                            ConsumeQuantity = x.ConsumptionQuantity,
                            Position = x.Position,
                            RequiredQuantity = x.WareHouseQuantity - x.WastageQuantity, /// For update quantity after purchase
                            PerUnitID = x.PerUnitID,
                            PriceUnitID = x.PriceUnitID,
                            Season = x.ItemStyle.Season,
                            MaterialTypeCode = x.MaterialTypeCode,
                            GroupSize = x.GroupSize ?? false,
                            GroupItemColor = x.GroupItemColor ?? false,
                            QuantityPerUnit = x.QuantityPerUnit,
                            OtherName = x.OtherName,
                        });

                    viewObject.MaterialRequestDetailPreviews = proBomPreviews.ToList();
                    View.Refresh();
                }
            }
        }

        private void SearchItemStyleForMaterialRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as MaterialRequestFromProductionBom;
           
            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("(Contains(LSStyle, ?) OR ?) AND (Contains(PurchaseOrderNumber, ?) OR ?)  AND TotalQuantity > 0",
                    viewObject.Style, string.IsNullOrEmpty(viewObject.Style), 
                    viewObject.PurchaseOrderNumber, string.IsNullOrEmpty(viewObject.PurchaseOrderNumber));
                var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria);
                viewObject.ItemStyles = itemStyles.ToList();
                
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
