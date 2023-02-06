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
    public partial class GetForecastMaterialAction_ViewController : ViewController
    {
        public GetForecastMaterialAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchForecastOverallForPurchaseOrderAction = new SimpleAction(this,
                "SearchForecastOverallForPurchaseOrder", PredefinedCategory.Unspecified);
            searchForecastOverallForPurchaseOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchForecastOverallForPurchaseOrderAction.Caption = "Search (Ctrl + L)";
            searchForecastOverallForPurchaseOrderAction.TargetObjectType = typeof(PurchaseForecastMaterial);
            searchForecastOverallForPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            searchForecastOverallForPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchForecastOverallForPurchaseOrderAction.Shortcut = "CtrlL";

            searchForecastOverallForPurchaseOrderAction.Execute += 
                SearchForecastOverallForPurchaseOrderAction_Execute;

            SimpleAction loadForecastMaterialForPurchaseOrderAction = new SimpleAction(this, 
                "LoadForecastMaterialForPurchaseOrder", PredefinedCategory.Unspecified);
            loadForecastMaterialForPurchaseOrderAction.ImageName = "RotateCounterclockwise";
            loadForecastMaterialForPurchaseOrderAction.Caption = "Load Bom (Shift + B)";
            loadForecastMaterialForPurchaseOrderAction.TargetObjectType = typeof(PurchaseForecastMaterial);
            loadForecastMaterialForPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            loadForecastMaterialForPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadForecastMaterialForPurchaseOrderAction.Shortcut = "ShiftB";

            loadForecastMaterialForPurchaseOrderAction.Execute += 
                LoadForecastMaterialForPurchaseOrderAction_Execute;
        }

        private void LoadForecastMaterialForPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchaseForecastMaterial;
            var forecastMaterialIDs = viewObject.ReservationForecastEntries?.Select(x => x.ForecastMaterialID);
            if (viewObject != null)
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("ForecastOveralls") as ListPropertyEditor;
                if(listPropertyEditor != null)
                {
                    var selectedOveralls = listPropertyEditor.ListView
                        .SelectedObjects.Cast<ForecastOverall>().ToList();
                    string criteriaString = string.Empty;

                    if (forecastMaterialIDs != null && forecastMaterialIDs.Any())
                    {
                        criteriaString = "VendorID = ? AND RemainQuantity > 0 AND " +
                            "ForecastOverallID IN (" + string.Join(',', selectedOveralls?.Select(x => "'" + x.ID + "'")) + ") AND " +
                            "NOT (ID IN (" + string.Join(',', forecastMaterialIDs) + "))";
                    }
                    else
                    {
                        criteriaString = "VendorID = ? AND RemainQuantity > 0 AND " +
                            "ForecastOverallID IN (" + string.Join(',', selectedOveralls?.Select(x => "'" + x.ID + "'")) + ")";
                    }

                    var criteria = CriteriaOperator.Parse(criteriaString, viewObject.VendorID);
                    var forcastMaterials = ObjectSpace.GetObjects<ForecastMaterial>(criteria);
                    viewObject.ForecastMaterials = forcastMaterials.ToList();
                }
            }
            View.Refresh();
        }

        private void SearchForecastOverallForPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchaseForecastMaterial;
            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("[ForecastEntry.IsDeactive] <> true AND (Contains(CustomerStyle,?) OR ?)",
                    viewObject.Style, string.IsNullOrEmpty(viewObject.Style));
                var forcastOveralls = ObjectSpace.GetObjects<ForecastOverall>(criteria);
                viewObject.ForecastOveralls = forcastOveralls.ToList();
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
