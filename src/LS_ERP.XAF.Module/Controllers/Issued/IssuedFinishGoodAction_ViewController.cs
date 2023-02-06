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
    public partial class IssuedFinishGoodAction_ViewController : ViewController
    {
        public IssuedFinishGoodAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadStyleToIssuedAction = new SimpleAction(this, "LoadStyleToIssued", PredefinedCategory.Unspecified);
            loadStyleToIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadStyleToIssuedAction.Caption = "Search (Ctrl + L)";
            loadStyleToIssuedAction.TargetObjectType = typeof(IssuedFinishGoodParam);
            loadStyleToIssuedAction.TargetViewType = ViewType.DetailView;
            loadStyleToIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadStyleToIssuedAction.Shortcut = "CtrlShiftL";

            loadStyleToIssuedAction.Execute += LoadStyleToIssuedAction_Execute;

            SimpleAction loadStyleDetailToIssuedAction = new SimpleAction(this, "LoadStyleDetailToIssued", PredefinedCategory.Unspecified);
            loadStyleDetailToIssuedAction.ImageName = "RotateCounterclockwise";
            loadStyleDetailToIssuedAction.Caption = "Load (Ctrl + B)";
            loadStyleDetailToIssuedAction.TargetObjectType = typeof(IssuedFinishGoodParam);
            loadStyleDetailToIssuedAction.TargetViewType = ViewType.DetailView;
            loadStyleDetailToIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadStyleDetailToIssuedAction.Shortcut = "CtrlL";

            loadStyleDetailToIssuedAction.Execute += LoadStyleDetailToIssuedAction_Execute;
        }

        private void LoadStyleDetailToIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedFinishGoodParam;

            if (viewObject != null)
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("Details")
                    as ListPropertyEditor;

                if (listPropertyEditor != null)
                {
                    var selectedStyles = listPropertyEditor.ListView.SelectedObjects.Cast<IssuedFinishStyle>().ToList();
                    var criteriaString = "StorageCode = 'FG' AND OnHandQuantity > 0 AND " +
                        "LSStyle IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.LSStyle + "'")) + ")";

                    var storageDetails = ObjectSpace.GetObjects<StorageDetail>(CriteriaOperator.Parse(criteriaString));
                    viewObject.Details = storageDetails
                        .Select(x => new IssuedFinishGoodDetail()
                        {
                            CustomerStyle = x.CustomerStyle,
                            LSStyle = x.LSStyle,
                            GarmentColorCode = x.GarmentColorCode,
                            GarmentColorName = x.GarmentColorName,
                            GarmentSize = x.GarmentSize,
                            ProductionDescription = x.PurchaseOrderNumber,
                            InStockQuantity = x.OnHandQuantity ?? 0,
                        }).ToList();

                    View.Refresh();
                }
            }
        }

        private void LoadStyleToIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedFinishGoodParam;

            if (viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("[SalesOrder.CustomerID] = ? AND (Contains(LSStyle, ?) OR ?))",
                    viewObject.Customer?.ID, viewObject.SearchStyle, string.IsNullOrEmpty(viewObject.SearchStyle));
                var styles = ObjectSpace.GetObjects<ItemStyle>(criteria);
                

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
