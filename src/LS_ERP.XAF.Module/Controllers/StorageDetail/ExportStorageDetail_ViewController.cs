using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportStorageDetail_ViewController : ObjectViewController<DetailView, StorageDetailReportParam>
    {
        public ExportStorageDetail_ViewController()
        {
            InitializeComponent();

            SimpleAction exportStorageDetail = new SimpleAction(this, "ExportStorageDetail", PredefinedCategory.Unspecified);
            exportStorageDetail.ImageName = "Export";
            exportStorageDetail.Caption = "Export";
            
            exportStorageDetail.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            
            exportStorageDetail.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportStorageDetail.Execute += ExportStorageDetail_Execute;


        }
        public virtual void ExportStorageDetail_Execute(object sender, SimpleActionExecuteEventArgs e)
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
