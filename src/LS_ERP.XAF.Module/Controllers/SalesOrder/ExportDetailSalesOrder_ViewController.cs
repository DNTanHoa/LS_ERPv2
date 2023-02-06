using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportDetailSalesOrder_ViewController : ViewController
    {
        public ExportDetailSalesOrder_ViewController()
        {
            InitializeComponent();
            SimpleAction exportSalesOrder = new SimpleAction(this, "ExportDetailSalesOrder", PredefinedCategory.Unspecified);
            exportSalesOrder.ImageName = "Export";
            exportSalesOrder.Caption = "Export (Ctrl + E)";
            exportSalesOrder.TargetObjectType = typeof(SalesOrder);
            exportSalesOrder.TargetViewType = ViewType.DetailView;
            exportSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportSalesOrder.Shortcut = "CtrlE";
            exportSalesOrder.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportSalesOrder.Execute += ExportDetailSalesOrder_Execute;
        }

        public virtual void ExportDetailSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
