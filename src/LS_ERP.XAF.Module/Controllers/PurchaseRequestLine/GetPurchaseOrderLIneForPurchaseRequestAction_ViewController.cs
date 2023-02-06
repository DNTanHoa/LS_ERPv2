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
    public partial class GetPurchaseOrderLIneForPurchaseRequestAction_ViewController : ViewController
    {
        public GetPurchaseOrderLIneForPurchaseRequestAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchOldPurchaseOrderForPurchaseRequestAction = new SimpleAction(this, "SearchOldPurchaseOrderForPurchaseRequest", PredefinedCategory.Unspecified);
            searchOldPurchaseOrderForPurchaseRequestAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchOldPurchaseOrderForPurchaseRequestAction.Caption = "Search (Ctrl + L)";
            searchOldPurchaseOrderForPurchaseRequestAction.TargetObjectType = typeof(RequestOldPurchaseOrder);
            searchOldPurchaseOrderForPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            searchOldPurchaseOrderForPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchOldPurchaseOrderForPurchaseRequestAction.Shortcut = "CtrlL";

            searchOldPurchaseOrderForPurchaseRequestAction.Execute += SearchOldPurchaseOrderForPurchaseRequestAction_Execute;

            SimpleAction loadOldPurchaseOrderLineForPurchaseRequestAction = new SimpleAction(this, "LoadOldPurchaseOrderLineForPurchaseRequest", PredefinedCategory.Unspecified);
            loadOldPurchaseOrderLineForPurchaseRequestAction.ImageName = "RotateCounterclockwise";
            loadOldPurchaseOrderLineForPurchaseRequestAction.Caption = "Load Purchase (Shift + B)";
            loadOldPurchaseOrderLineForPurchaseRequestAction.TargetObjectType = typeof(RequestOldPurchaseOrder);
            loadOldPurchaseOrderLineForPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            loadOldPurchaseOrderLineForPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadOldPurchaseOrderLineForPurchaseRequestAction.Shortcut = "ShiftB";

            loadOldPurchaseOrderLineForPurchaseRequestAction.Execute += LoadOldPurchaseOrderLineForPurchaseRequestAction_Execute; ;

        }

        private void LoadOldPurchaseOrderLineForPurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as RequestOldPurchaseOrder;
            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("PurchaseOrders") as ListPropertyEditor;
            if (listPropertyEditor != null)
            {
                var selectedPurchaseOrders = listPropertyEditor.ListView.SelectedObjects.Cast<PurchaseOrder>().ToList();
                var criteria = CriteriaOperator.Parse("PurchaseOrderID IN(" + 
                    string.Join(',', selectedPurchaseOrders?.Select(x => "'" + x.ID + "'")) + ")");
                var purchaseOrderLines = ObjectSpace.GetObjects<PurchaseOrderLine>(criteria);
                viewObject.PurchaseOrderLines = purchaseOrderLines.ToList();
            }
            View.Refresh();
        }

        private void SearchOldPurchaseOrderForPurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as RequestOldPurchaseOrder;

            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("Contains(Number,?)", viewObject.PurchaseOrderNumber);
                var purchaseOrders = ObjectSpace.GetObjects<PurchaseOrder>(criteria);
                viewObject.PurchaseOrders = purchaseOrders.ToList();

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
