using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MergeItemPartRevision_ViewController : ViewController
    {
        public MergeItemPartRevision_ViewController()
        {
            InitializeComponent();
            SimpleAction exportPartRevision = new SimpleAction(this, "MergeItemPartRevision", PredefinedCategory.Unspecified);
            exportPartRevision.ImageName = "MergeCross";
            exportPartRevision.Caption = "Merge Item (Ctrl + M)";
            exportPartRevision.TargetObjectType = typeof(PartRevision);
            exportPartRevision.TargetViewType = ViewType.Any;
            exportPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportPartRevision.Shortcut = "CtrlM";
            exportPartRevision.Execute += MergeItemPartRevision_Execute;
        }

        public virtual void MergeItemPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var currentPartRevision = View.SelectedObjects;
            var service = new ItemService();
            var messageOptions = new MessageOptions();

            var request = new MergeItemRequest()
            {
                Username = SecuritySystem.CurrentUserName
            };

            var mergeItemResponse = service.MergeItem(request).Result;

            if (mergeItemResponse != null)
            {
                if (mergeItemResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(mergeItemResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(mergeItemResponse.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            ObjectSpace.CommitChanges();
            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
