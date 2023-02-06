using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportMultiShipPackingList_ViewController : ObjectViewController<DetailView, PackingListSearchParam>
    {
        public ExportMultiShipPackingList_ViewController()
        {
            InitializeComponent();

            SimpleAction exportMultiShipPackingList = new SimpleAction(this, "ExportMultiShipPackingList", PredefinedCategory.Unspecified);
            exportMultiShipPackingList.ImageName = "Export";
            exportMultiShipPackingList.Caption = "Export (Ctrl + E)";
            exportMultiShipPackingList.TargetObjectType = typeof(PackingListSearchParam);
            exportMultiShipPackingList.TargetViewType = ViewType.DetailView;
            exportMultiShipPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportMultiShipPackingList.Shortcut = "CtrlE";
            exportMultiShipPackingList.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportMultiShipPackingList.Execute += ExportMultiShipPackingList;
        }
        public virtual void ExportMultiShipPackingList(object sender, SimpleActionExecuteEventArgs e)
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
