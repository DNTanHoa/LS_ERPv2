using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetPurchaseOrderLIneForPurchaseRequest_ViewController : ViewController
    {
        public GetPurchaseOrderLIneForPurchaseRequest_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction getOldPurchaseAction = new PopupWindowShowAction(this, "GetOldPurchaseOrderForPurchaseRequestLine",
                PredefinedCategory.Unspecified);

            getOldPurchaseAction.ImageName = "Header";
            getOldPurchaseAction.Caption = "Old Purchase";
            getOldPurchaseAction.TargetObjectType = typeof(PurchaseRequestLine);
            getOldPurchaseAction.TargetViewType = ViewType.ListView;
            getOldPurchaseAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getOldPurchaseAction.Shortcut = "CtrlShiftP";

            getOldPurchaseAction.CustomizePopupWindowParams += GetOldPurchaseAction_CustomizePopupWindowParams;
            getOldPurchaseAction.Execute += GetOldPurchaseAction_Execute; ;
        }

        private void GetOldPurchaseAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GetOldPurchaseAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new RequestOldPurchaseOrder();
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = Application.CreateDetailView(objectSpace, model, false);
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
