using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class CalculateRequiredQuantity_ForecastOverall_ViewController : ViewController
    {
        public CalculateRequiredQuantity_ForecastOverall_ViewController()
        {
            InitializeComponent();

            SimpleAction calculateRequiredQuantity = new SimpleAction(this, "CalculateRequiredQuantityForecastOverall", PredefinedCategory.Unspecified);
            calculateRequiredQuantity.ImageName = "CalcDefault";
            calculateRequiredQuantity.Caption = "Cal.Required Quantity";
            calculateRequiredQuantity.TargetObjectType = typeof(ForecastOverall);
            calculateRequiredQuantity.TargetViewType = ViewType.ListView;
            calculateRequiredQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            calculateRequiredQuantity.Execute += CalculateRequiredQuantity_Execute;
        }

        private void CalculateRequiredQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var forecastOveralls = e.SelectedObjects.Cast<ForecastOverall>();
            var forecastOverallIDs = forecastOveralls.Select(x => x.ID);
            var customerID = forecastOveralls.FirstOrDefault().ForecastEntry?.ForecastGroup?.CustomerID;
            MessageOptions messageOptions = null;

            var request = new CalculateRequiredQuantityForecastOverallRequest()
            {
                UserName = SecuritySystem.CurrentUserName,
                ForecastOverallIDs = forecastOverallIDs.ToList(),
                CustomerID = customerID
            };

            var service = new ForecastGroupService();

            var respone = service.CalculateRequiredQuantity(request).Result;

            if (respone != null)
            {
                if (respone.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Calculate required quantity successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(respone.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error",
                    InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(messageOptions);

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
