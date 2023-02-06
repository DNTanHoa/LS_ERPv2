using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SubmitRequest_ViewController : ViewController
    {
        public SubmitRequest_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction submitPurchaseRequestAction = new PopupWindowShowAction(this, "SubmitPurchaseRequest",
                PredefinedCategory.Unspecified);
            submitPurchaseRequestAction.ImageName = "Actions_Send";
            submitPurchaseRequestAction.Caption = "Submit";
            submitPurchaseRequestAction.TargetObjectType = typeof(PurchaseRequest);
            submitPurchaseRequestAction.TargetViewType = ViewType.Any;
            submitPurchaseRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            submitPurchaseRequestAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            submitPurchaseRequestAction.CustomizePopupWindowParams += 
                SubmitPurchaseRequestAction_CustomizePopupWindowParams;
            submitPurchaseRequestAction.Execute += SubmitPurchaseRequestAction_Execute;
        }

        public virtual void SubmitPurchaseRequestAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            
        }

        private void SubmitPurchaseRequestAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SubmitPurchaseRequestParam()
            {
                
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
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
