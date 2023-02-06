using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.Report.DomainComponent
{
    public partial class SeasonReportAction_ViewController : ViewController
    {
        public SeasonReportAction_ViewController()
        {
            InitializeComponent();

            SimpleAction seasonReportAction = new SimpleAction(this, "SeasonReportAction", PredefinedCategory.Unspecified);
            seasonReportAction.ImageName = "Action_Search_Object_FindObjectByID";
            seasonReportAction.Caption = "Search (Ctrl + L)";
            seasonReportAction.TargetObjectType = typeof(SeasonReportParam);
            seasonReportAction.TargetViewType = ViewType.DetailView;
            seasonReportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            seasonReportAction.Execute += SeasonReportAction_Execute;
        }

        private void SeasonReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as SeasonReportParam;

            if (viewObject != null)
            {
                var service = new ReportService();
                var response = service.GetSeasonReport(viewObject.Season,
                    viewObject.Customer.ID, viewObject.Style, viewObject.Key).Result;
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
