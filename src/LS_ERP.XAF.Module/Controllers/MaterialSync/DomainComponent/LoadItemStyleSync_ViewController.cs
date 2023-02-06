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
    public partial class LoadItemStyleSync_ViewController : ViewController
    {
        public LoadItemStyleSync_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleSyncAction = new SimpleAction(this, "SearchItemStyleSync", PredefinedCategory.Unspecified);
            searchItemStyleSyncAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleSyncAction.Caption = "Search (Ctrl + L)";
            searchItemStyleSyncAction.TargetObjectType = typeof(ItemStyleSyncSearchParam);
            searchItemStyleSyncAction.TargetViewType = ViewType.DetailView;
            searchItemStyleSyncAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchItemStyleSyncAction.Execute += SearchItemStyleSyncAction_Execute;
        }

        private void SearchItemStyleSyncAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ItemStyleSyncSearchParam;
            
            if (viewObject != null)
            {
                var criteria = CriteriaOperator
                    .Parse("(CustomerID = ? OR ? )AND CreatedAt >= ? " +
                    "AND CreatedAt <= ? AND (Contains(LSStyle,?) OR ?)",
                    viewObject.Customer?.ID, string.IsNullOrEmpty(viewObject.Customer?.ID), 
                    viewObject.FromDate, viewObject.ToDate, 
                    viewObject.Style, string.IsNullOrEmpty(viewObject.Style));
                var itemStyleSycMasters = ObjectSpace.GetObjects<ItemStyleSyncMaster>(criteria);
                viewObject.ItemStyles = itemStyleSycMasters.ToList();
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
