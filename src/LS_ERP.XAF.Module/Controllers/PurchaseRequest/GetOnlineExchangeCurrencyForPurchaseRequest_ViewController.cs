using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetOnlineExchangeCurrencyForPurchaseRequest_ViewController : ViewController
    {
        public GetOnlineExchangeCurrencyForPurchaseRequest_ViewController()
        {
            InitializeComponent();

            SimpleAction getCurencyExchangeInPurchaseRequestAction = new SimpleAction(this, "GetCurencyExchangeInPurchaseRequest", PredefinedCategory.Unspecified);
            getCurencyExchangeInPurchaseRequestAction.ImageName = "ModelEditor_Localization_Localized";
            getCurencyExchangeInPurchaseRequestAction.Caption = "Get Exchange (Ctrl + E)";
            getCurencyExchangeInPurchaseRequestAction.TargetObjectType = typeof(PurchaseRequest);
            getCurencyExchangeInPurchaseRequestAction.TargetViewType = ViewType.DetailView;
            getCurencyExchangeInPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getCurencyExchangeInPurchaseRequestAction.Shortcut = "CtrlE";

            getCurencyExchangeInPurchaseRequestAction.Execute += GetCurencyExchangeInPurchaseRequestAction_Execute;
        }

        private void GetCurencyExchangeInPurchaseRequestAction_Execute(object sender, 
            SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchaseRequest;
            MessageOptions options = null;

            if (viewObject != null)
            {
                if (viewObject.CurrencyExchangeType != null)
                {
                    var service = new OnlineExchangeService();
                    var getExchangeResult = service.GetOnlineExchangeViaVietComBank(viewObject.CurrencyExchangeType.CurrencyID,
                        viewObject.CurrencyExchangeType.DestinationCurrencyID, out decimal exchangeValue, out string errorMessage);

                    if (getExchangeResult)
                    {
                        viewObject.ExchangeValue = exchangeValue;
                        options = Message.GetMessageOptions("Exchange successfully", "Succeess",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        options = Message.GetMessageOptions(errorMessage, "Error",
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    options = Message.GetMessageOptions("Please select exchange type.", "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                options = Message.GetMessageOptions("Unknow error, contact your admin.", "Error",
                    InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(options);
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
