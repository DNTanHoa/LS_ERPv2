using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportStorageBinEntryPopupAction_ViewController : ObjectViewController<DetailView, StorageBinEntryCreateParam>
    {
        public ImportStorageBinEntryPopupAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportStorageBinEntry = new SimpleAction(this, "BrowserStorageBinEntryImportFile", PredefinedCategory.Unspecified);
            browserImportStorageBinEntry.ImageName = "Open";
            browserImportStorageBinEntry.Caption = "Browser";
            browserImportStorageBinEntry.TargetObjectType = typeof(StorageBinEntryCreateParam);
            browserImportStorageBinEntry.TargetViewType = ViewType.DetailView;
            browserImportStorageBinEntry.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportStorageBinEntry.Execute += BrowserImportStorageBinEntry_Execute;

            SimpleAction ImportStorageBinEntry = new SimpleAction(this, "ImportStorageBinEntryAction", PredefinedCategory.Unspecified);
            ImportStorageBinEntry.ImageName = "Import";
            ImportStorageBinEntry.Caption = "Import";
            ImportStorageBinEntry.TargetObjectType = typeof(StorageBinEntryCreateParam);
            ImportStorageBinEntry.TargetViewType = ViewType.DetailView;
            ImportStorageBinEntry.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            ImportStorageBinEntry.Execute += ImportStorageBinEntry_Execute;
        }
        public virtual void BrowserImportStorageBinEntry_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }
        private void ImportStorageBinEntry_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as StorageBinEntryCreateParam;
            MessageOptions message = null;

            if (viewObject != null)
            {
                var service = new StorageBinEntryService();

                var request = new ImportStorageBinEntryRequest()
                {
                    FilePath = viewObject.ImportFilePath,
                    CustomerID = viewObject.Customer?.ID,
                    UserName = SecuritySystem.CurrentUserName
                };

                var response = service.Import(request).Result;

                if (response != null)
                {
                    viewObject.Data = response.Data;
                    View.Refresh();
                }
            }
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
