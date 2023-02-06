using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ReportPackingList_ViewController : ViewController
    {
        public ReportPackingList_ViewController()
        {
            InitializeComponent();

            SimpleAction reportPacking = new SimpleAction(this,
                "ReportPacking", PredefinedCategory.Unspecified);
            reportPacking.ImageName = "Print";
            reportPacking.Caption = "Preview";
            reportPacking.TargetObjectType = typeof(PackingList);
            reportPacking.TargetViewType = ViewType.DetailView;
            reportPacking.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            reportPacking.Shortcut = "CtrlShiftP";

            reportPacking.Execute += ReportPacking_Execute;
        }

        private void ReportPacking_Execute(object sender, SimpleActionExecuteEventArgs e)
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
