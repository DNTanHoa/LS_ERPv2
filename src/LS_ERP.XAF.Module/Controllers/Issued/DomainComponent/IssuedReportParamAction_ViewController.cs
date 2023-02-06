using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class IssuedReportParamAction_ViewController : ViewController
    {
        public IssuedReportParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction reportIssuedAction = new SimpleAction(this, "ReportIssued", PredefinedCategory.Unspecified);
            reportIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            reportIssuedAction.Caption = "Search (Ctrl + L)";
            reportIssuedAction.TargetObjectType = typeof(IssuedReportParam);
            reportIssuedAction.TargetViewType = ViewType.DetailView;
            reportIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            reportIssuedAction.Shortcut = "CtrlL";

            reportIssuedAction.Execute += SearchIssuedAction_Execute;
        }

        private void SearchIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedReportParam;

            if (viewObject != null)
            {
                var service = new IssuedService();
                var response = service.GetReport(viewObject.Customer?.ID, viewObject.Storage?.Code,
                    viewObject.FromDate, viewObject.ToDate).Result;
                if (response != null)
                {
                    viewObject.Details = response.Data;

                    View.Refresh();
                }
                else
                {
                    var message = Message.GetMessageOptions("Not found data", "Error",
                        InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
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
