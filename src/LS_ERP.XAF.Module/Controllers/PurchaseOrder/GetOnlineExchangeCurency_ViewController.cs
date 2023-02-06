using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetOnlineExchangeCurency_ViewController : ObjectViewController<DetailView, PurchaseOrder>
    {
        public GetOnlineExchangeCurency_ViewController()
        {
            InitializeComponent();

            SimpleAction getCurencyExchangeInPurchaseOrderAction = new SimpleAction(this, "GetCurencyExchangeInPurchaseOrder", PredefinedCategory.Unspecified);
            getCurencyExchangeInPurchaseOrderAction.ImageName = "ModelEditor_Localization_Localized";
            getCurencyExchangeInPurchaseOrderAction.Caption = "Get Exchange (Ctrl + E)";
            getCurencyExchangeInPurchaseOrderAction.TargetObjectType = typeof(PurchaseOrder);
            getCurencyExchangeInPurchaseOrderAction.TargetViewType = ViewType.DetailView;
            getCurencyExchangeInPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getCurencyExchangeInPurchaseOrderAction.Shortcut = "CtrlE";

            getCurencyExchangeInPurchaseOrderAction.Execute += GetCurencyExchangeInPurchaseOrderAction_Execute;
        }

        private void GetCurencyExchangeInPurchaseOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchaseOrder;
            MessageOptions options = null;

            if(viewObject != null)
            {
                if(viewObject.CurrencyExchangeType != null)
                {
                    var service = new OnlineExchangeService();
                    var getExchangeResult = service.GetOnlineExchangeViaVietComBank(viewObject.CurrencyExchangeType.CurrencyID,
                        viewObject.CurrencyExchangeType.DestinationCurrencyID, out decimal exchangeValue, out string errorMessage);

                    if (getExchangeResult)
                    {
                        viewObject.CurrencyExchangeValue = exchangeValue;
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
