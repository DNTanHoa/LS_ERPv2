using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportItemPricePopupAction_ViewController : ViewController
    {
        public ImportItemPricePopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportItemPrice = new SimpleAction(this, "BrowserItemPriceImportFile", PredefinedCategory.Unspecified);
            browserImportItemPrice.ImageName = "Open";
            browserImportItemPrice.Caption = "Browser";
            browserImportItemPrice.TargetObjectType = typeof(ItemPriceImportParam);
            browserImportItemPrice.TargetViewType = ViewType.DetailView;
            browserImportItemPrice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportItemPrice.Execute += BrowserImportItemPrice_Execute;

            SimpleAction importItemPrice = new SimpleAction(this, "ImportItemPriceAction", PredefinedCategory.Unspecified);
            importItemPrice.Caption = "Import";
            importItemPrice.ImageName = "Import";
            importItemPrice.TargetObjectType = typeof(ItemPriceImportParam);
            importItemPrice.TargetViewType = ViewType.DetailView;
            importItemPrice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importItemPrice.Execute += ImportItemPrice_Execute;

        }

        private void ImportItemPrice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var importParam = e.CurrentObject as ItemPriceImportParam;
            var service = new ItemPriceService();
            var request = new ImportItemPriceRequest()
            {
                FilePath = importParam.FilePath,
                CustomerID = importParam?.Customer.ID,
                UserName = SecuritySystem.CurrentUserName
            };
            var importResponse = service.ImportItemPrice(request).Result;
            var messageOptions = new MessageOptions();

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);

                    importParam.ItemPrices = importResponse.Data;
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        public virtual void BrowserImportItemPrice_Execute(object sender, SimpleActionExecuteEventArgs e)
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
