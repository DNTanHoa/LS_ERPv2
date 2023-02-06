using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadPurchaseOrderFromSearchParam_ViewController : ViewController
    {
        public LoadPurchaseOrderFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPurchaseOderAction = new SimpleAction(this, "SearchPurchaseOrder", PredefinedCategory.Unspecified);
            searchPurchaseOderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPurchaseOderAction.Caption = "Search (Ctrl + L)";
            searchPurchaseOderAction.TargetObjectType = typeof(PurchaseOrderSearchParam);
            searchPurchaseOderAction.TargetViewType = ViewType.DetailView;
            searchPurchaseOderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            SimpleAction newPurchaseOderAction = new SimpleAction(this, "NewPurchaseOrder", PredefinedCategory.Unspecified);
            newPurchaseOderAction.ImageName = "Action_New";
            newPurchaseOderAction.Caption = "New Purchase";
            newPurchaseOderAction.TargetObjectType = typeof(PurchaseOrderSearchParam);
            newPurchaseOderAction.TargetViewType = ViewType.DetailView;
            newPurchaseOderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchPurchaseOderAction.Execute += SearchPurchaseOderAction_Execute;
            newPurchaseOderAction.Execute += NewPurchaseOderAction_Execute;
        }

        private void NewPurchaseOderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateNestedObjectSpace(ObjectSpace);
            var model = objectSpace.CreateObject<PurchaseOrder>();
            model.Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault();
            model.OrderDate = DateTime.Today;
            model.Company = ObjectSpace.GetObjects<Company>().FirstOrDefault(x => x.Code == "LS");
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, model, false);
        }

        private void SearchPurchaseOderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as PurchaseOrderSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([Customer] = ? OR ? Is Null) AND ([Vendor] = ? OR ? Is Null) AND ([OrderDate] <= ?) AND ([OrderDate] >= ?))",
                    searchParam.Customer, searchParam.Customer, searchParam.Vendor, searchParam.Vendor,
                    searchParam.OrderToDate, searchParam.OrderFromDate);

                var objectSpace = this.ObjectSpace;
                var purchaseOrders = objectSpace.GetObjects<PurchaseOrder>(criteria);
                searchParam.PurchaseOrders = purchaseOrders.ToList();

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
