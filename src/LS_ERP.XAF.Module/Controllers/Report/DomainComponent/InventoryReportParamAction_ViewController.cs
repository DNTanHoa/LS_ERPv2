using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class InventoryReportParamAction_ViewController 
        : ObjectViewController<DetailView, InventoryReportParam>
    {
        public InventoryReportParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchIventoryReportAction = new SimpleAction(this, "SearchIventoryReportAction", PredefinedCategory.Unspecified);
            searchIventoryReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchIventoryReportAction.Caption = "Search (Ctrl + L)";
            searchIventoryReportAction.TargetObjectType = typeof(InventoryReportParam);
            searchIventoryReportAction.TargetViewType = ViewType.DetailView;
            searchIventoryReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchIventoryReportAction.Execute += SearchIventoryReportAction_Execute;
        }

        private void SearchIventoryReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as InventoryReportParam;

            if(viewObject != null)
            {
                var service = new ReportService();
                var response = service.GetInventoryReportResponse(viewObject.Customer?.ID, viewObject.Storage?.Code,
                    viewObject.PurchaseOrder, viewObject.Search).Result;

                if (response != null)
                {
                    viewObject.Details = response.Data;
                    View.Refresh();
                }
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
