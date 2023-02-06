using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateSaleOrderInformationAction_ViewController : ViewController
    {
        public UpdateSaleOrderInformationAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserSalesOrderUpdateInformationFile = new SimpleAction(this, 
                "BrowserSalesOrderUpdateInformationFile", PredefinedCategory.Unspecified);
            browserSalesOrderUpdateInformationFile.ImageName = "Open";
            browserSalesOrderUpdateInformationFile.Caption = "Browser";
            browserSalesOrderUpdateInformationFile.TargetObjectType = typeof(SalesOrderUpdateInformationParam);
            browserSalesOrderUpdateInformationFile.TargetViewType = ViewType.DetailView;
            browserSalesOrderUpdateInformationFile.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserSalesOrderUpdateInformationFile.Execute += BrowserSalesOrderUpdateInformationFile_Execute;

            SimpleAction importSalesOrderUpdateInformationFile = new SimpleAction(this, 
                "ImportSalesOrderUpdateInformationFile", PredefinedCategory.Unspecified);
            importSalesOrderUpdateInformationFile.ImageName = "Import";
            importSalesOrderUpdateInformationFile.Caption = "Import";
            importSalesOrderUpdateInformationFile.TargetObjectType = typeof(SalesOrderUpdateInformationParam);
            importSalesOrderUpdateInformationFile.TargetViewType = ViewType.DetailView;
            importSalesOrderUpdateInformationFile.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importSalesOrderUpdateInformationFile.Execute += ImportSalesOrderUpdateInformationFile_Execute;
        }

        private void ImportSalesOrderUpdateInformationFile_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as SalesOrderUpdateInformationParam;
            MessageOptions message = null;
            if(viewObject != null)
            {
                var service = new ItemStyleService();
                var request = new ImportUpdateInforRequest()
                {
                    UserName = SecuritySystem.CurrentUserName,
                    FilePath = viewObject.FilePath
                };

                var response = service.UpdateInfor(request).Result;

                if(response.Result.Code == "000")
                {
                    viewObject.Data = response.Data;
                    message = Message.GetMessageOptions(response.Result.Message, "Successfully",
                        InformationType.Success, null, 5000);
                    View.Refresh();
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error",
                    InformationType.Error, null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);
        }

        public virtual void BrowserSalesOrderUpdateInformationFile_Execute(object sender, 
            SimpleActionExecuteEventArgs e)
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
