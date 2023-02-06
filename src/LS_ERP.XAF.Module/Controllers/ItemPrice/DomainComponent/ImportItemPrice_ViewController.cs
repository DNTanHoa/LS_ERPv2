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
    public partial class ImportItemPrice_ViewController : ViewController
    {
        public ImportItemPrice_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importItemPrice = new PopupWindowShowAction(this, "ImportItemPrice", PredefinedCategory.Unspecified);
            importItemPrice.ImageName = "Import";
            importItemPrice.Caption = "Import (Ctrl + I)";
            importItemPrice.TargetObjectType = typeof(ItemPriceSearchParam);
            importItemPrice.TargetViewType = ViewType.DetailView;
            importItemPrice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importItemPrice.Shortcut = "CtrlI";

            importItemPrice.CustomizePopupWindowParams += ImportItemPrice_CustomizePopupWindowParams;
            importItemPrice.Execute += ImportItemPrice_Execute;
        }

        private void ImportItemPrice_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as ItemPriceImportParam;
            MessageOptions messageOptions = null;

            if(viewObject != null)
            {
                var itemPriceService = new ItemPriceService();
                var request = new CreateMultiItemPriceRequest()
                {
                    ItemPrices = viewObject.ItemPrices,
                };

                var response = itemPriceService.CreateMultItemPrice(request).Result;

                if (response.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Confirm successfully", "Success", InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(response.Result?.Message, "Error", InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unknown error. Contact your admin for support", "Error", InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(messageOptions);

            ObjectSpace.CommitChanges();
            View.Refresh();

        }

        private void ImportItemPrice_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ItemPriceImportParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
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
