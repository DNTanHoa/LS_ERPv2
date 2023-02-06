using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PopupFabricInvoice_ViewController : ObjectViewController<DetailView, ReceiptSearchParam>
    {
        public PopupFabricInvoice_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupFabricInvoiceReceipt = new PopupWindowShowAction(this, "PopupFabricInvoiceReceipt", PredefinedCategory.Unspecified);
            popupFabricInvoiceReceipt.ImageName = "Header";
            popupFabricInvoiceReceipt.Caption = "Fabric (Ctrl + Shift + F)";
            popupFabricInvoiceReceipt.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupFabricInvoiceReceipt.Shortcut = "CtrlShiftF";

            popupFabricInvoiceReceipt.CustomizePopupWindowParams += PopupFabricInvoiceReceipt_CustomizePopupWindowParams;
            popupFabricInvoiceReceipt.Execute += PopupFabricInvoiceReceipt_Execute;

        }

        private void PopupFabricInvoiceReceipt_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new FabricInvoicePopupModel()
            {
                ArrivedDate = DateTime.Today
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = false;
            e.View = view;
        }

        private void PopupFabricInvoiceReceipt_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
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
