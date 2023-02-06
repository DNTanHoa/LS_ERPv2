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
    public partial class DeletePurchaseOrderLine_ViewController : ViewController
    {
        public DeletePurchaseOrderLine_ViewController()
        {
            InitializeComponent();

            SimpleAction deletePurchaseOrderLineAction = new SimpleAction(this, "DeletePurchaseOrderLine", PredefinedCategory.Unspecified);
            deletePurchaseOrderLineAction.ImageName = "Delete";
            deletePurchaseOrderLineAction.Caption = "Delete";
            deletePurchaseOrderLineAction.TargetObjectType = typeof(PurchaseOrderLine);
            deletePurchaseOrderLineAction.TargetViewType = ViewType.Any;
            deletePurchaseOrderLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deletePurchaseOrderLineAction.Shortcut = "CtrlE";

            deletePurchaseOrderLineAction.Execute += DeletePurchaseOrderLineAction_Execute;
        }

        private void DeletePurchaseOrderLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObjects = View.SelectedObjects.Cast<PurchaseOrderLine>();
            MessageOptions message = null;
            
            if(viewObjects.Any())
            {
                var service = new PurchaseOrderLineService();
                var request = new DeletePurchaseOrderLineRequest()
                {
                    UserName = SecuritySystem.CurrentUserName,
                    PurchaseOrderLineIDs = viewObjects.Select(x => x.ID).ToList()
                };

                var response = service.DeletePurchaseOrderLine(request).Result;

                if(response != null)
                {
                    if(response.Result.Code == "000")
                    {
                        message = Message.GetMessageOptions("Action successfully", "Success",
                            InformationType.Success,
                            null, 5000);

                        ObjectSpace.Refresh();
                        View.Refresh();
                    }
                    else
                    {
                        message = Message.GetMessageOptions("Unexpeced error", "Error", InformationType.Error,
                            null, 5000);
                    }
                }
                else 
                {
                    message = Message.GetMessageOptions("Unexpeced error", "Error", InformationType.Error,
                        null, 5000);
                }

                if(message != null)
                {
                    Application.ShowViewStrategy.ShowMessage(message);
                }
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
