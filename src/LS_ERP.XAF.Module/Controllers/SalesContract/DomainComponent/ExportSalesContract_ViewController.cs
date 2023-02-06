using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ExportSalesContract_ViewController : ViewController
    {
        public ExportSalesContract_ViewController()
        {
            InitializeComponent();
            SimpleAction exportSalesContract = new SimpleAction(this, "ExportSalesContract", PredefinedCategory.Unspecified);
            exportSalesContract.ImageName = "Export";
            exportSalesContract.Caption = "Export (Ctrl + E)";
            exportSalesContract.TargetObjectType = typeof(SalesContract);
            exportSalesContract.TargetViewType = ViewType.ListView;
            exportSalesContract.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportSalesContract.Shortcut = "CtrlE";
            exportSalesContract.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportSalesContract.Execute += ExportSalesContract_Execute;
        }

        public virtual void ExportSalesContract_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
