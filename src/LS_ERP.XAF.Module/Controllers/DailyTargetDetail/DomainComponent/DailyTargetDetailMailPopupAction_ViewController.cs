using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DailyTargetDetailMailPopupAction_ViewController : ViewController
    {
        public DailyTargetDetailMailPopupAction_ViewController()
        {
            InitializeComponent();
            SimpleAction browserDailyTargetDetailAttachFile = new SimpleAction(this, "BrowserDailyTargetDetailAttachFile", PredefinedCategory.Unspecified);
            browserDailyTargetDetailAttachFile.ImageName = "Open";
            browserDailyTargetDetailAttachFile.Caption = "Browser";
            browserDailyTargetDetailAttachFile.TargetObjectType = typeof(DailyTargetDetailMailParam);
            browserDailyTargetDetailAttachFile.TargetViewType = ViewType.DetailView;
            browserDailyTargetDetailAttachFile.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            browserDailyTargetDetailAttachFile.Execute += BrowserDailyTargetDetailAttachFile_Execute;
        }

        public virtual void BrowserDailyTargetDetailAttachFile_Execute(object sender, SimpleActionExecuteEventArgs e)
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
