using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DeleteReceipt_ViewController : ObjectViewController<ListView, Receipt>
    {
        public DeleteReceipt_ViewController()
        {
            InitializeComponent();

            SimpleAction deleteReceiptAction = new SimpleAction(this, "DeleteReceiptAction", PredefinedCategory.Unspecified);
            deleteReceiptAction.ImageName = "Action_Delete";
            deleteReceiptAction.Caption = "Delete";
            deleteReceiptAction.TargetObjectType = typeof(Receipt);
            deleteReceiptAction.TargetViewType = ViewType.ListView;
            deleteReceiptAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deleteReceiptAction.Shortcut = "ShiftD";
            deleteReceiptAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            
            deleteReceiptAction.Execute += DeleteReceiptAction_Execute;
        }

        private void DeleteReceiptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as Receipt;
            MessageOptions message = null;

            if (viewObject != null)
            {
                var service = new ReceiptService();
                var response = service.DeleteReceipt(viewObject.Number).Result;

                if(response != null)
                {
                    if(response.Result.Code == "000")
                    {
                        Message.GetMessageOptions(response.Result.Message, "Success", 
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        Message.GetMessageOptions(response.Result.Message, "Error", 
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    Message.GetMessageOptions("Server error", "Error", 
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);
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
