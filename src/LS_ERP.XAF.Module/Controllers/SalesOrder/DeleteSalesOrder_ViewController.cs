using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DeleteSalesOrder_ViewController : ViewController
    {
        public DeleteSalesOrder_ViewController()
        {
            InitializeComponent();
            SimpleAction deleteSalesOrder = new SimpleAction(this, "DeleteSalesOrder", PredefinedCategory.Unspecified);
            deleteSalesOrder.ImageName = "Delete";
            deleteSalesOrder.Caption = "Delete (Shift + D)";
            deleteSalesOrder.TargetObjectType = typeof(SalesOrder);
            deleteSalesOrder.TargetViewType = ViewType.ListView;
            deleteSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            deleteSalesOrder.Shortcut = "ShiftD";
            deleteSalesOrder.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            deleteSalesOrder.Execute += DeleteSalesOrder_Execute;
        }

        private void DeleteSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var salesOrder = View.CurrentObject as SalesOrder;
            MessageOptions message = null;
            var service = new SalesOrderService();

            var response = service.Delete(salesOrder.ID).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
                        null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                        null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

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
