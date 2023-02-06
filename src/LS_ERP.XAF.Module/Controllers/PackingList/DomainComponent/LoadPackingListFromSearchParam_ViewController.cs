using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadPackingListFromSearchParam_ViewController 
        : ObjectViewController<DetailView, PackingListSearchParam>
    {
        public LoadPackingListFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPackingListAction = new SimpleAction(this, "SearchPackingList", PredefinedCategory.Unspecified);
            searchPackingListAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchPackingListAction.Caption = "Search (Ctrl + L)";
            searchPackingListAction.TargetObjectType = typeof(PackingListSearchParam);
            searchPackingListAction.TargetViewType = ViewType.DetailView;
            searchPackingListAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchPackingListAction.Shortcut = "CtrlL";

            searchPackingListAction.Execute += SearchPackingListAction_Execute;

            PopupWindowShowAction searchMultiPackingListAction =
                new PopupWindowShowAction(this, "SearchMultiPackingList", PredefinedCategory.Unspecified);
            searchMultiPackingListAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchMultiPackingListAction.Caption = "Search Multi (Ctrl + L)";
            searchMultiPackingListAction.TargetObjectType = typeof(PackingListSearchParam);
            searchMultiPackingListAction.TargetViewType = ViewType.DetailView;
            searchMultiPackingListAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchMultiPackingListAction.Shortcut = "CtrlL";

            searchMultiPackingListAction.CustomizePopupWindowParams += 
                SearchMultiPackingListAction_CustomizePopupWindowParams;
            searchMultiPackingListAction.Execute += SearchMultiPackingListAction_Execute;

        }

        private void SearchMultiPackingListAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var searchParam = e.PopupWindowViewCurrentObject as PackingListSearchMultiParam;
            var currentObject = View.CurrentObject as PackingListSearchParam;
            
            var lsStyles = searchParam.Searchs.Split("\r\n");
            var criteria = string.Join(";", lsStyles);

            var connectionString = Application.ConnectionString;

            using (var db = new QueryFactory(
                   new SqlConnection(connectionString), new SqlServerCompiler()))
            {
                var packingLists = db.Select<PackingList>("EXEC sp_SelectPackingListByMultiLSStyle @LSStyles",
                        new { LSStyles = criteria }).ToList();
                currentObject.PackingLists = packingLists;
                View.Refresh();
            }
        }

        private void SearchMultiPackingListAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PackingListSearchMultiParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        private void SearchPackingListAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PackingListSearchParam;

            if (viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("([CustomerID] = ? OR ?) AND " +
                    "PackingListDate >= ? AND PackingListDate <= ? AND " +
                    "([ParentPackingListID] IS  NULL OR [ParentPackingListID] = 0)",
                    viewObject.Customer?.ID, string.IsNullOrEmpty(viewObject.Customer?.ID),
                    viewObject.PackingFromDate, viewObject.PackingToDate);
                var packingLists = ObjectSpace.GetObjects<PackingList>(criteria);
                viewObject.PackingLists = packingLists.ToList();
            }

            View.Refresh();
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
