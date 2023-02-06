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
    public partial class BalanceQuantity_ForecastMaterial_ViewController : ViewController
    {
        public BalanceQuantity_ForecastMaterial_ViewController()
        {
            InitializeComponent();

            SimpleAction balanceQuantityForecastOverall = new SimpleAction(this, "BalanceQuantityForecastOverall", PredefinedCategory.Unspecified);
            balanceQuantityForecastOverall.ImageName = "GlobalColors";
            balanceQuantityForecastOverall.Caption = "Balance Quantity";
            balanceQuantityForecastOverall.TargetObjectType = typeof(ForecastOverall);
            balanceQuantityForecastOverall.TargetViewType = ViewType.ListView;
            balanceQuantityForecastOverall.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            balanceQuantityForecastOverall.Execute += BalanceQuantityForecastOverall_Execute;
        }

        private void BalanceQuantityForecastOverall_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var forecastOveralls = e.SelectedObjects.Cast<ForecastOverall>();
            var forecastOverallIDs = forecastOveralls.Select(x => x.ID);
            var customerID = forecastOveralls.FirstOrDefault().ForecastEntry?.ForecastGroup?.CustomerID;
            MessageOptions messageOptions = null;

            var request = new BalanceQuantityForecastGroupRequest()
            {
                UserName = SecuritySystem.CurrentUserName,
                ForecastOverallIDs = forecastOverallIDs.ToList(),
                CustomerID = customerID
            };

            var service = new ForecastGroupService();

            var respone = service.BalanceQuantity(request).Result;

            if (respone != null)
            {
                if (respone.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Balance quantity successfully", "Success",
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
