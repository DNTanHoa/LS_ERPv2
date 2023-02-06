using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportPackingListPopupAction_ViewController : ViewController
    {
        public ImportPackingListPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportPackingList = new SimpleAction(this, "BrowserPackingListImportFile", PredefinedCategory.Unspecified);
            browserImportPackingList.ImageName = "Open";
            browserImportPackingList.Caption = "Browser";
            browserImportPackingList.TargetObjectType = typeof(PackingListImportParam);
            browserImportPackingList.TargetViewType = ViewType.DetailView;
            browserImportPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportPackingList.Execute += BrowserImportPackingList_Execute;

            //SimpleAction importPackingList = new SimpleAction(this, "ImportPackingListAction", PredefinedCategory.Unspecified);
            //importPackingList.ImageName = "Import";
            //importPackingList.Caption = "Import";
            //importPackingList.TargetObjectType = typeof(PackingListImportParam);
            //importPackingList.TargetViewType = ViewType.DetailView;
            //importPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            //importPackingList.Execute += ImportPackingListAction_Execute;
        }

        public virtual void BrowserImportPackingList_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }

        //private void ImportPackingListAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    var viewObject = View.CurrentObject as PackingListImportParam;
        //    MessageOptions message = null;

        //    if (viewObject != null)
        //    {
        //        var service = new PackingListService();
        //        var request = new ImportPackingListRequest()
        //        {
        //            FilePath = viewObject.ImportFilePath,
        //            UserName = SecuritySystem.CurrentUserName
        //        };
        //        var response = service.ImportPackingList(request).Result;
        //        if (response != null)
        //        {
        //            View.Refresh();
        //        }
        //    }
        //}
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
