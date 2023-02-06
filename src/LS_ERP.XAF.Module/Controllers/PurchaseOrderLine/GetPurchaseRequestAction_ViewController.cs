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
    public partial class GetPurchaseRequestAction_ViewController : ViewController
    {
        public GetPurchaseRequestAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchRequestForPurchaseOrderAction = new SimpleAction(this, "SearchRequestForPurchaseOrder", PredefinedCategory.Unspecified);
            searchRequestForPurchaseOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchRequestForPurchaseOrderAction.Caption = "Search (Ctrl + L)";
            searchRequestForPurchaseOrderAction.TargetObjectType = typeof(PurchaseRequestMaterial);
            searchRequestForPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            searchRequestForPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchRequestForPurchaseOrderAction.Shortcut = "CtrlL";

            searchRequestForPurchaseOrderAction.Execute += SearchRequestForPurchaseOrderAction_Execute;

            SimpleAction loadRequestLineForPurchaseOrderAction = new SimpleAction(this, "LoadRequestForPurchaseOrder", PredefinedCategory.Unspecified);
            loadRequestLineForPurchaseOrderAction.ImageName = "RotateCounterclockwise";
            loadRequestLineForPurchaseOrderAction.Caption = "Load Request (Shift + R)";
            loadRequestLineForPurchaseOrderAction.TargetObjectType = typeof(PurchaseRequestMaterial);
            loadRequestLineForPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            loadRequestLineForPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadRequestLineForPurchaseOrderAction.Shortcut = "ShiftR";

            loadRequestLineForPurchaseOrderAction.Execute += LoadRequestLineForPurchaseOrderAction_Execute;
        }

        private void LoadRequestLineForPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as PurchaseRequestMaterial;
            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("Requests") as ListPropertyEditor;
            if (listPropertyEditor != null)
            {
                var selectedRequest = listPropertyEditor.ListView.SelectedObjects
                                        .Cast<PurchaseRequest>().ToList();
                var criteria = CriteriaOperator.Parse("VendorID = ? AND RemainQuantity > 0 " +
                    "AND PurchaseRequestID IN(" + string.Join(',', selectedRequest?.Select(x => "'" + x.ID + "'")) + ")",
                    viewObject.Vendor.ID);
                var requestMaterials = ObjectSpace.GetObjects<PurchaseRequestLine>(criteria);
                viewObject.RequestMaterials = requestMaterials.ToList();
            }
            View.Refresh();
        }

        private void SearchRequestForPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchaseRequestMaterial;
            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("[StatusID] = 'A' AND " +
                    "Contains(Number,?) AND [RequestDate] >= ? AND [RequestDate] <= ?",
                    viewObject.RequestNumber, viewObject.FromDate, viewObject.ToDate);
                var purchaseRequests = ObjectSpace.GetObjects<PurchaseRequest>(criteria);
                viewObject.Requests = purchaseRequests.ToList();
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
