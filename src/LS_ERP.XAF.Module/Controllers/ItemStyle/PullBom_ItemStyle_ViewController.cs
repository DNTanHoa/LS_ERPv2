using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PullBom_ItemStyle_ViewController : ViewController
    {
        public PullBom_ItemStyle_ViewController()
        {
            InitializeComponent();

            SimpleAction pullBomAction = new SimpleAction(this, "PullBomItemStyle", PredefinedCategory.Unspecified);
            pullBomAction.ImageName = "BringToFront";
            pullBomAction.Caption = "Pull Bom";
            pullBomAction.TargetObjectType = typeof(ItemStyle);
            pullBomAction.TargetViewType = ViewType.ListView;
            pullBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            pullBomAction.Execute += PullBomAction_Execute;
        }

        private void PullBomAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var itemStyleBomPulled = e.SelectedObjects.Cast<ItemStyle>().FirstOrDefault(x => x.IsBomPulled == true);

            if (itemStyleBomPulled != null)
            {
                var messageOptions = Message.GetMessageOptions("Your select has style which bom is pulled. Do you want to continue?",
                    "Warning", InformationType.Warning, new Action(() => PullBom(e)), 500);
                Application.ShowViewStrategy.ShowMessage(messageOptions);
            }
            else
            {
                PullBom(e);
            }
        }

        private void PullBom(SimpleActionExecuteEventArgs e)
        {
            var itemStyles = e.SelectedObjects.Cast<ItemStyle>();
            var itemStyleNumbers = itemStyles.Select(x => x.Number);
            var customerID = itemStyles.FirstOrDefault().SalesOrder?.CustomerID;
            MessageOptions messageOptions = null;

            var request = new PullBomItemStyleRequest()
            {
                CustomerID = customerID,
                UserName = SecuritySystem.CurrentUserName,
                ItemStyleNumbers = itemStyleNumbers.ToList(),
            };

            var service = new ItemStyleService();

            var respone = service.PullBom(request).Result;

            if (respone != null)
            {
                if (respone.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Pull successfully", "Success",
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
