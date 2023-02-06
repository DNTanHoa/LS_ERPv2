using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class LoadSalesContractFromSearchParam_ViewController : ViewController
    {
        public LoadSalesContractFromSearchParam_ViewController()
        {
            InitializeComponent();
            SimpleAction searchPurchaseOderAction = new SimpleAction(this, "SearchSalesContract", PredefinedCategory.Unspecified);
            searchPurchaseOderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPurchaseOderAction.Caption = "Search (Ctrl + L)";
            searchPurchaseOderAction.TargetObjectType = typeof(SalesContractSearchParam);
            searchPurchaseOderAction.TargetViewType = ViewType.DetailView;
            searchPurchaseOderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchPurchaseOderAction.Shortcut = "CtrlL";

            searchPurchaseOderAction.Execute += SearchPurchaseOderAction_Execute;

        }

        private void SearchPurchaseOderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as SalesContractSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([CreatedAt] <= ?) AND ([CreatedAt] >= ?))",
                     searchParam.ToDate, searchParam.FromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(SalesContract));
                var salesContracts = objectSpace.GetObjects<SalesContract>(criteria);
                searchParam.SalesContract = salesContracts.ToList();

                View.Refresh();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
