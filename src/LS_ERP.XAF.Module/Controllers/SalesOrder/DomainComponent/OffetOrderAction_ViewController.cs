using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class OffetOrderAction_ViewController : ViewController
    {
        public OffetOrderAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserOffsetSalesOrder = new SimpleAction(this, "BrowserOffsetSalesOrder", PredefinedCategory.Unspecified);
            browserOffsetSalesOrder.ImageName = "Open";
            browserOffsetSalesOrder.Caption = "Browser";
            browserOffsetSalesOrder.TargetObjectType = typeof(SalesOrderOffsetParam);
            browserOffsetSalesOrder.TargetViewType = ViewType.DetailView;
            browserOffsetSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserOffsetSalesOrder.Execute += BrowserOffsetSalesOrder_Execute;

            SimpleAction importOffsetSalesOrder = new SimpleAction(this, "ImportOffsetSalesOrder", PredefinedCategory.Unspecified);
            importOffsetSalesOrder.ImageName = "Import";
            importOffsetSalesOrder.Caption = "Import";
            importOffsetSalesOrder.TargetObjectType = typeof(SalesOrderOffsetParam);
            importOffsetSalesOrder.TargetViewType = ViewType.DetailView;
            importOffsetSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importOffsetSalesOrder.Execute += ImportOffsetSalesOrder_Execute;
        }

        private void ImportOffsetSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var param = View.CurrentObject as SalesOrderOffsetParam;
            var service = new SalesOrderService();
            var request = new ImportSalesOrderOffsetRequest()
            {
                FilePath = param.FilePath,
                CustomerID = param.Customer?.ID
            };

            var response = service.ImportSalesOrderOffset(request).Result;
            var messageOptions = new MessageOptions();

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(response.Result.Message, "Success",
                        InformationType.Success, null, 5000);

                    param.OffsetDetails = response.Data;
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(response.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message
                    .GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            View.Refresh();
            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        public virtual void BrowserOffsetSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

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
