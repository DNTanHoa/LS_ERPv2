using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponet
{
    public partial class LoadPurchaseRequestFromSearchParam_ViewController : ViewController
    {
        public LoadPurchaseRequestFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPurchaseRequestAction = new SimpleAction(this, "SearchPurchaseRequest", PredefinedCategory.Unspecified);
            searchPurchaseRequestAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPurchaseRequestAction.Caption = "Search (Ctrl + L)";
            searchPurchaseRequestAction.TargetObjectType = typeof(PurchaseRequestSearchParam);
            searchPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            searchPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchPurchaseRequestAction.Shortcut = "CtrlL";

            SimpleAction newPurchaseRequestAction = new SimpleAction(this, "NewPurchaseRequest", PredefinedCategory.Unspecified);
            newPurchaseRequestAction.ImageName = "Action_New";
            newPurchaseRequestAction.Caption = "New Request";
            newPurchaseRequestAction.TargetObjectType = typeof(PurchaseRequestSearchParam);
            newPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            newPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchPurchaseRequestAction.Execute += SearchPurchaseRequestAction_Execute; ;
            newPurchaseRequestAction.Execute += NewPurchaseRequestAction_Execute;
        }

        private void SearchPurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            var searchParam = View.CurrentObject as PurchaseRequestSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([RequestDate] >= ? AND [RequestDate] <= ?))",
                    searchParam.FromDate, searchParam.ToDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Customer));
                var purchaseRequests = objectSpace.GetObjects<PurchaseRequest>(criteria);
                searchParam.PurchaseRequests = purchaseRequests.ToList();

                View.Refresh();
            }
            
        }

        private void NewPurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(PurchaseRequest));
            var model = objectSpace.CreateObject<PurchaseRequest>();
            model.RequestDate = DateTime.Today;
            model.Division = ObjectSpace.GetObjects<Division>().FirstOrDefault();
            model.Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault();
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, model, true);
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
