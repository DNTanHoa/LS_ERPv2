using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ApproveSubmit_ViewController : ViewController
    {
        public ApproveSubmit_ViewController()
        {
            InitializeComponent();

            SimpleAction approveBomSubmit = new SimpleAction(this, "ApproveBomSubmit", PredefinedCategory.Unspecified);
            approveBomSubmit.ImageName = "ApplyChanges";
            approveBomSubmit.Caption = "Approve";
            approveBomSubmit.TargetObjectType = typeof(PartRevisionLog);
            approveBomSubmit.TargetViewType = ViewType.ListView;
            approveBomSubmit.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            approveBomSubmit.TargetObjectsCriteria = "IsNullOrEmpty(PartRevisionLogReferenceCode)";

            approveBomSubmit.Execute += ApproveBomSubmit_Execute;
        }

        public virtual void ApproveBomSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
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
