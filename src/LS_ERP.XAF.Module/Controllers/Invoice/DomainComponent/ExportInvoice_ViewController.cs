using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ExportInvoice_ViewController : ObjectViewController<DetailView, Invoice>
    {
        public ExportInvoice_ViewController()
        {
            InitializeComponent();

            SimpleAction exportInvoice = new SimpleAction(this, "ExportInvoice", PredefinedCategory.Unspecified);
            exportInvoice.ImageName = "Export";
            exportInvoice.Caption = "Export";
            //exportInvoice.TargetObjectType = typeof(Invoice);
            //exportInvoice.TargetViewType = ViewType.DetailView;
            exportInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //exportSalesContract.Shortcut = "CtrlShiftP";
            exportInvoice.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportInvoice.Execute += ExportInvoice_Execute;
        }

        public virtual void ExportInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
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
