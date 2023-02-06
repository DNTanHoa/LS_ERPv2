using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DailyTargetDetailMailParam_ViewController : ViewController
    {
        public DailyTargetDetailMailParam_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction sendViaGmailAction = new PopupWindowShowAction(this, "SendMailDailyTargetDetailSummaryByDate", PredefinedCategory.Unspecified);
            sendViaGmailAction.ImageName = "Glyph_Mail";
            sendViaGmailAction.Caption = "Send Mail";
            sendViaGmailAction.TargetObjectType = typeof(DailyTargetDetailSummaryByDate);
            sendViaGmailAction.TargetViewType = ViewType.DetailView;
            sendViaGmailAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            sendViaGmailAction.Execute += SendViaGmailAction_Execute; ;
            sendViaGmailAction.CustomizePopupWindowParams += BrowserImportAttachFile_CustomizePopupWindowParams;
        }

        public virtual void SendViaGmailAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
           
        }

        private void BrowserImportAttachFile_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var searchParam = View.CurrentObject as DailyTargetDetailSummaryByDate;
            var date = searchParam.ProduceDate.ToString("MMMM dd yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var objectSpace = this.ObjectSpace;
            var model = new DailyTargetDetailMailParam();
            model.Subject = "Daily Production Output Report of " + date;
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
