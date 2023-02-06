using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ExportPackingList_ViewController : ViewController
    {
        public ExportPackingList_ViewController()
        {
            InitializeComponent();
            SimpleAction exportSalesContract = new SimpleAction(this, "ExportPackingList", PredefinedCategory.Unspecified);
            exportSalesContract.ImageName = "Export";
            exportSalesContract.Caption = "Export (Ctrl + E)";
            exportSalesContract.TargetObjectType = typeof(PackingList);
            exportSalesContract.TargetViewType = ViewType.DetailView;
            exportSalesContract.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportSalesContract.Shortcut = "CtrlE";
            exportSalesContract.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportSalesContract.Execute += ExportPackingList_Execute;
        }

        public virtual void ExportPackingList_Execute(object sender, SimpleActionExecuteEventArgs e)
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
