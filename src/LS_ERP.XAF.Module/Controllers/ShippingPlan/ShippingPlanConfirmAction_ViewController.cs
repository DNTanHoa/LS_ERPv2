using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShippingPlanConfirmAction_ViewController : ObjectViewController<ListView, ShippingPlanDetail>
    {
        public ShippingPlanConfirmAction_ViewController()
        {
            InitializeComponent();

            SimpleAction confirmShippingPlan = new SimpleAction(this, "ConfirmShippingPlan", PredefinedCategory.Unspecified);
            confirmShippingPlan.ImageName = "ApplyChanges";
            confirmShippingPlan.Caption = "Confirm";
            confirmShippingPlan.TargetObjectType = typeof(ShippingPlanDetail);
            confirmShippingPlan.TargetViewType = ViewType.ListView;
            confirmShippingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            confirmShippingPlan.Execute += ConfirmShippingPlan_Execute;
        }
        private void ConfirmShippingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var shippingPlanDetails = ((ListView)View).SelectedObjects.Cast<ShippingPlanDetail>().ToList();

            if (shippingPlanDetails.Any())
            {
                try
                {
                    shippingPlanDetails.ForEach(x =>
                    {
                        x.IsConfirm = true;
                    });

                    objectSpace.CommitChanges();
                    
                    var message  = Message.GetMessageOptions("Confirm sucessful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
                catch (Exception ex)
                {
                    var message = Message.GetMessageOptions("Confirm failed", "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }

            }

            View.Refresh(true);
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
