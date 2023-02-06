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
    public partial class LoadItemStyleFromSearchParam_ViewController : ViewController
    {
        public LoadItemStyleFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleAction = new SimpleAction(this, "SearchItemStyle", PredefinedCategory.Unspecified);
            searchItemStyleAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleAction.Caption = "Search (Ctrl + L)";
            searchItemStyleAction.TargetObjectType = typeof(ItemStyleSearchParam);
            searchItemStyleAction.TargetViewType = ViewType.DetailView;
            searchItemStyleAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemStyleAction.Shortcut = "CtrlL";

            searchItemStyleAction.Execute += SearchItemStyleAction_Execute;
        }

        private void SearchItemStyleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ItemStyleSearchParam searchParam = View.CurrentObject as ItemStyleSearchParam;
            if(searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(Contains(LSStyle,?) OR ? ) AND" +
                    "(Contains(PurchaseOrderNumber,?) OR ?) AND ([SalesOrder.CustomerID] = ? OR ?) AND" +
                    "([SalesOrder.OrderDate] >= ? OR IsNull(?))",
                    searchParam.Number, string.IsNullOrEmpty(searchParam.Number),
                    searchParam.PurchaseOrderNumber, string.IsNullOrEmpty(searchParam.PurchaseOrderNumber),
                    searchParam.Customer?.ID, string.IsNullOrEmpty(searchParam.Customer?.ID),
                    searchParam.FromDate, searchParam.FromDate);
                var itemstyles = ObjectSpace.GetObjects<ItemStyle>(criteria);
                searchParam.Styles = itemstyles.ToList();
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
