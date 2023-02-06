using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportPartRevisionAction_ViewController : ViewController
    {
        public ImportPartRevisionAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportPartRevision = new SimpleAction(this, "BrowserPartRevisionImportFile", PredefinedCategory.Unspecified);
            browserImportPartRevision.ImageName = "Open";
            browserImportPartRevision.Caption = "Browser";
            browserImportPartRevision.TargetObjectType = typeof(ImportPartRevisionParam);
            browserImportPartRevision.TargetViewType = ViewType.DetailView;
            browserImportPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportPartRevision.Execute += BrowserImportPartRevision_Execute;

            SimpleAction importPartRevision = new SimpleAction(this, "ImportPartRevisionAction", PredefinedCategory.Unspecified);
            importPartRevision.Caption = "Import";
            importPartRevision.ImageName = "Import";
            importPartRevision.TargetObjectType = typeof(ImportPartRevisionParam);
            importPartRevision.TargetViewType = ViewType.DetailView;
            importPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importPartRevision.Execute += ImportPartRevision_Execute; ;

        }

        private void ImportPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var importParam = e.CurrentObject as ImportPartRevisionParam;
            var service = new PartRevisionService();
            var request = new ImportPartRevisionRequest()
            {
                FilePath = importParam.FilePath,
                UserName = SecuritySystem.CurrentUserName,
                CustomerID = importParam.Customer?.ID,
                StyleNumber = importParam.StyleNumber,
                IsConfirmed = importParam.IsConfirmed,
                EffectDate = importParam.EffectDate,
                RevisionNumber = importParam.RevisionNumber,
                Season = importParam.Season,
                FileNameServer = importParam.FileNameServer,
                FileName = importParam.FileName
            };
            var importResponse = service.ImportPartRevision(request).Result;
            var messageOptions = new MessageOptions();

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "001")
                {
                    messageOptions = Message
                        .GetMessageOptions(importResponse.Result.Message, "Success",
                            InformationType.Success, null, 5000);

                    importParam.PartMaterials = importResponse.Data?.PartRevision?.PartMaterials?.ToList();
                    importParam.Items = importResponse.Data?.Items;
                    importParam.FileNameServer = importResponse.Data?.PartRevision.FileNameServer;
                    importParam.FileName = importResponse.Data?.PartRevision.FileName;
                }
                else
                {
                    messageOptions = Message
                        .GetMessageOptions(importResponse.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message
                    .GetMessageOptions("Unexpected error", "Error",
                        InformationType.Error, null, 5000);
            }

            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        public virtual void BrowserImportPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
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
