using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DeleteForecastEntry_ViewController : ViewController
    {
        public DeleteForecastEntry_ViewController()
        {
            InitializeComponent();

            SimpleAction deleteForecastEntry = new SimpleAction(this, "DeleteForecastEntry", PredefinedCategory.Unspecified);
            deleteForecastEntry.ImageName = "Delete";
            deleteForecastEntry.Caption = "Delete (Shift + D)";
            deleteForecastEntry.TargetObjectType = typeof(ForecastEntry);
            deleteForecastEntry.TargetViewType = ViewType.ListView;
            deleteForecastEntry.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deleteForecastEntry.Shortcut = "ShiftD";
            deleteForecastEntry.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            deleteForecastEntry.Execute += DeleteForecastEntry_Execute;
        }

        private void DeleteForecastEntry_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var forecastEntry = View.CurrentObject as ForecastEntry;
            MessageOptions message = null;
            var service = new ForecastEntryService();

            var response = service.Delete(forecastEntry.ID).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                        null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                        null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

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
