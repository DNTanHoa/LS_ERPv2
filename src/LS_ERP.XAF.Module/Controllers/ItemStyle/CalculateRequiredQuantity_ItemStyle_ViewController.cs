using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class CalculateRequiredQuantity_ItemStyle_ViewController : ViewController
    {
        public CalculateRequiredQuantity_ItemStyle_ViewController()
        {
            InitializeComponent();
            SimpleAction calculateRequiredQuantity = new SimpleAction(this, "CalculateRequiredQuantityItemStyle", PredefinedCategory.Unspecified);
            calculateRequiredQuantity.ImageName = "CalcDefault";
            calculateRequiredQuantity.Caption = "Cal.Required Quantity";
            calculateRequiredQuantity.TargetObjectType = typeof(ItemStyle);
            calculateRequiredQuantity.TargetViewType = ViewType.ListView;
            calculateRequiredQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            calculateRequiredQuantity.Execute += CalculateRequiredQuantity_Execute;
        }

        private void CalculateRequiredQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var itemStyles = e.SelectedObjects.Cast<ItemStyle>();
            var itemStyleNumbers = itemStyles.Select(x => x.Number);
            var customerID = itemStyles.FirstOrDefault().SalesOrder?.CustomerID;
            MessageOptions messageOptions = null;

            var request = new CalculateRequiredQuantityItemStyleRequest()
            {
                UserName = SecuritySystem.CurrentUserName,
                ItemStyleNumbers = itemStyleNumbers.ToList(),
            };

            var service = new ItemStyleService();

            var respone = service.CalculateRequiredQuantity(request).Result;

            if (respone != null)
            {
                if (respone.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Calculate successfully", "Success",
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
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
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
