using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ItemReportSearchParamAction_ViewController : ViewController
    {
        public ItemReportSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemReportAction = new SimpleAction(this, "SearchItemReport", PredefinedCategory.Unspecified);
            searchItemReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemReportAction.Caption = "Search (Ctrl + L)";
            searchItemReportAction.TargetObjectType = typeof(ItemReportSearchParam);
            searchItemReportAction.TargetViewType = ViewType.DetailView;
            searchItemReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchItemReportAction.Execute += SearchItemReportAction_Execute;
        }

        private void SearchItemReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ItemReportSearchParam;
            var service = new ReportService();
            var response = service.GetSummaryReport(viewObject.Style,viewObject.Customer?.ID,
                viewObject.FromDate, viewObject.ToDate).Result;

            if(response.IsSuccess)
            {
                viewObject.Items = response.Data.ToList();
                View.Refresh();
            }
            else
            {
                var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(message);
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
