using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportItemStyle_ViewController : ViewController
    {
        public ExportItemStyle_ViewController()
        {
            InitializeComponent();
            SimpleAction pullBomAction = new SimpleAction(this, "ExportItemStyle", PredefinedCategory.Unspecified);
            pullBomAction.ImageName = "Export";
            pullBomAction.Caption = "Export";
            pullBomAction.TargetObjectType = typeof(ItemStyle);
            pullBomAction.TargetViewType = ViewType.ListView;
            pullBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            pullBomAction.Execute += ExportItemStyle_Execute;
        }

        public virtual void ExportItemStyle_Execute(object sender, SimpleActionExecuteEventArgs e)
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
