using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportPartRevision_ViewController : ViewController
    {
        public ExportPartRevision_ViewController()
        {
            InitializeComponent();

            SimpleAction exportPartRevision = new SimpleAction(this, "ExportPartRevision", PredefinedCategory.Unspecified);
            exportPartRevision.ImageName = "ExportAs";
            exportPartRevision.Caption = "Export";
            exportPartRevision.TargetObjectType = typeof(PartRevision);
            exportPartRevision.TargetViewType = ViewType.Any;
            exportPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportPartRevision.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportPartRevision.Execute += ExportPartRevision_Execute;
        }

        public virtual void ExportPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
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
