using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class RejectSubmit_ViewController : ViewController
    {
        public RejectSubmit_ViewController()
        {
            InitializeComponent();

            SimpleAction rejectBomSubmit = new SimpleAction(this, "RejectBomSubmit", PredefinedCategory.Unspecified);
            rejectBomSubmit.ImageName = "RemoveFooter";
            rejectBomSubmit.Caption = "Reject";
            rejectBomSubmit.TargetObjectType = typeof(PartRevisionLog);
            rejectBomSubmit.TargetViewType = ViewType.ListView;
            rejectBomSubmit.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            rejectBomSubmit.TargetObjectsCriteria = "IsNullOrEmpty(PartRevisionLogReferenceCode)";

            rejectBomSubmit.Execute += RejectBomSubmit_Execute;
        }

        public virtual void RejectBomSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
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
