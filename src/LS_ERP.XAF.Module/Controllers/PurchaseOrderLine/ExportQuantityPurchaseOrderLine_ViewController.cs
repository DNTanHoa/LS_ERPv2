using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ExportQuantityPurchaseOrderLine_ViewController : ViewController
    {
        public ExportQuantityPurchaseOrderLine_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction getPurchaseRequestAction = new PopupWindowShowAction(this, "ExportQuantityPurchaseOrderLine",
              PredefinedCategory.Unspecified);

            getPurchaseRequestAction.ImageName = "Export";
            getPurchaseRequestAction.Caption = "Export Quantity";
            getPurchaseRequestAction.TargetObjectType = typeof(PurchaseOrderLine);
            getPurchaseRequestAction.TargetViewType = ViewType.ListView;
            getPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getPurchaseRequestAction.Shortcut = "CtrlE";

            getPurchaseRequestAction.CustomizePopupWindowParams += ExportQuantityAction_CustomizePopupWindowParams;
            getPurchaseRequestAction.Execute += ExportQuantityAction_Execute;
        }

        void ExportQuantityAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PurchaseOrderLineExportQuantity();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }
        public virtual void ExportQuantityAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

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
