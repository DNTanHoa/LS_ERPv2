using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PartPriceImportAction_ViewController : ObjectViewController<DetailView, ImportPartPriceParam>
    {
        public PartPriceImportAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browsePartPriceImportFile = new SimpleAction(this, "BrowsePartPriceImportFile", PredefinedCategory.Unspecified);
            browsePartPriceImportFile.ImageName = "Open";
            browsePartPriceImportFile.Caption = "Browser";
            browsePartPriceImportFile.TargetObjectType = typeof(ImportPartPriceParam);
            browsePartPriceImportFile.TargetViewType = ViewType.DetailView;
            browsePartPriceImportFile.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browsePartPriceImportFile.Execute += BrowsePartPriceImportFile_Execute;
        }
        public virtual void BrowsePartPriceImportFile_Execute(object sender, SimpleActionExecuteEventArgs e)
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
