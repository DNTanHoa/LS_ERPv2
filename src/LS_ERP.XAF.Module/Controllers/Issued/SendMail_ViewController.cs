using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SendMail_ViewController : ObjectViewController<ListView, Issued>
    {
        public SendMail_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction sendMailAction = new PopupWindowShowAction(this, "SendMailIssued", PredefinedCategory.Unspecified);
            sendMailAction.ImageName = "Actions_Send";
            sendMailAction.Caption = "Send mail";
            sendMailAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //sendMailAction.TargetObjectType = typeof(FabricRequest);
            //sendMailAction.TargetViewType = ViewType.ListView;
            sendMailAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            sendMailAction.CustomizePopupWindowParams += SendMailIssuedAction_CustomizePopupWindowParams;
            sendMailAction.Execute += SendMailIssued_Execute;
        }

        public virtual void SendMailIssued_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void SendMailIssuedAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var currentObject = View.CurrentObject as Issued;

            var model = new IssuedSendMailParam();

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = false;
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
