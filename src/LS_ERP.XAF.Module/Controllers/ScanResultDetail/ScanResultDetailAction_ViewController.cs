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
    public partial class ScanResultDetailAction_ViewController 
        : ObjectViewController<ListView, ScanResultDetail>
    {
        public ScanResultDetailAction_ViewController()
        {
            InitializeComponent();

            SimpleAction deleteScanResultAction = new SimpleAction(this, "DeleteScanResultAction", PredefinedCategory.Unspecified);
            deleteScanResultAction.ImageName = "DeleteDataSource";
            deleteScanResultAction.Caption = "Delete";
            deleteScanResultAction.TargetObjectType = typeof(ScanResultDetail);
            deleteScanResultAction.TargetViewType = ViewType.ListView;
            deleteScanResultAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deleteScanResultAction.Shortcut = "ShiftD";

            deleteScanResultAction.Execute += DeleteScanResultAction_Execute;

            SimpleAction confirmScanResultAction = new SimpleAction(this, "ConfirmScanResultAction", PredefinedCategory.Unspecified);
            confirmScanResultAction.ImageName = "ItemTypeChecked";
            confirmScanResultAction.Caption = "Confirm";
            confirmScanResultAction.TargetObjectType = typeof(ScanResultDetail);
            confirmScanResultAction.TargetViewType = ViewType.ListView;
            confirmScanResultAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            confirmScanResultAction.Shortcut = "ShiftC";

            confirmScanResultAction.Execute += ConfirmScanResultAction_Execute;
        }

        private void ConfirmScanResultAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObjects = View.SelectedObjects.Cast<ScanResultDetail>();
            MessageOptions message = null;

            if(viewObjects != null &&
               viewObjects.Any())
            {
                var service = new ScanResultService();
                var response = service
                    .ConfirmScanResults(viewObjects.Select(x => x.ScanResultID).ToList())
                    .Result;

                if (response != null)
                {
                    var informationType = response.Success == true ?
                        InformationType.Success : InformationType.Error;

                    message = Message.GetMessageOptions(response.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions("Unexpected error", "Error",
                        InformationType.Error, null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);
            }
        }

        private void DeleteScanResultAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObjects = View.SelectedObjects.Cast<ScanResultDetail>();
            MessageOptions message = null;
            
            if(viewObjects != null &&
               viewObjects.Any())
            {
                var service = new ScanResultService();
                var response = service.DeleteScanResults(viewObjects.Select(x => x.ID).ToList(),
                    SecuritySystem.CurrentUserName).Result;

                if(response != null)
                {
                    var informationType = response.Success == true ?
                        InformationType.Success : InformationType.Error;
                    
                    message = Message.GetMessageOptions(response.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions("Unexpected error", "Error", 
                        InformationType.Error, null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);
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
