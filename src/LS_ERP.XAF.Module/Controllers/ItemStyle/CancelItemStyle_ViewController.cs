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
    public partial class CancelItemStyle_ViewController : ViewController
    {
        public CancelItemStyle_ViewController()
        {
            InitializeComponent();

            SimpleAction cancelItemStyle = new SimpleAction(this, "CancelItemStyle", PredefinedCategory.Unspecified);
            cancelItemStyle.ImageName = "RemoveHeader";
            cancelItemStyle.Caption = "Cancel";
            cancelItemStyle.TargetObjectType = typeof(ItemStyle);
            cancelItemStyle.TargetViewType = ViewType.ListView;
            cancelItemStyle.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            cancelItemStyle.Execute += CancelItemStyle_Execute;
        }
        private void CancelItemStyle_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            MessageOptions options = null;

            var itemStyles = View.SelectedObjects.Cast<ItemStyle>();
            var styleNumbers = itemStyles.Select(x => x.Number);
            var bulkCancelRequest = new BulkCancelItemStyleRequest()
            {
                StyleNumbers = styleNumbers.ToList(),
                UserName = SecuritySystem.CurrentUserName
            };

            var service = new ItemStyleService();
            var response = service.BulkCancel(bulkCancelRequest).Result;

            if(response != null)
            {
                if (response.Result.Code == "000")
                {
                    options = Message.GetMessageOptions("Pull successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    options = Message.GetMessageOptions(response.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                options = Message.GetMessageOptions("Unexpected error. Contact your admin",
                    "Error", InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(options);
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
