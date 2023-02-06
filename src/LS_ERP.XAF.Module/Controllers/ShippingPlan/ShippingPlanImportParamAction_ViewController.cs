using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShippingPlanImportParamAction_ViewController 
        : ObjectViewController<DetailView, ImportShippingPlanParam>
    {
        public ShippingPlanImportParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction BrowserShippingPlanImportFile = new SimpleAction(this, "BrowserShippingPlanImportFile", PredefinedCategory.Unspecified);
            BrowserShippingPlanImportFile.ImageName = "Open";
            BrowserShippingPlanImportFile.Caption = "Browser";
            BrowserShippingPlanImportFile.TargetObjectType = typeof(ImportShippingPlanParam);
            BrowserShippingPlanImportFile.TargetViewType = ViewType.DetailView;
            BrowserShippingPlanImportFile.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            BrowserShippingPlanImportFile.Execute += BrowserShippingPlanImportFile_Execute;

            SimpleAction importShippingPlanAction = new SimpleAction(this, "ImportShippingPlanAction", PredefinedCategory.Unspecified);
            importShippingPlanAction.Caption = "Import";
            importShippingPlanAction.ImageName = "Import";
            importShippingPlanAction.TargetObjectType = typeof(ImportShippingPlanParam);
            importShippingPlanAction.TargetViewType = ViewType.DetailView;
            importShippingPlanAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importShippingPlanAction.Execute += ImportShippingPlanAction_Execute;
        }

        private void ImportShippingPlanAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ImportShippingPlanParam;
            MessageOptions message = null;

            if(viewObject != null)
            {
                var service = new ShippingPlanService();
                var request = new ImportShippingPlanRequest()
                {
                    FilePath = viewObject.FilePath,
                    CustomerID = viewObject.Customer?.ID,
                    UserName = SecuritySystem.CurrentUserName,
                    CompanyID = viewObject.Company?.Code
                };
                var response = service.ImportShippingPlan(request).Result;

                if (response != null)
                {
                    if(response.Success)
                    {
                        //message = Message.GetMessageOptions("Action successfully", "Success",
                        //    InformationType.Success, null, 2000);
                        viewObject.Details = response.Data;
                        View.Refresh();
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Message, "Error",
                        InformationType.Error, null, 2000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unexpected error. Contact your admin", "Error",
                        InformationType.Error, null, 2000);
                }

                if(message != null)
                    Application.ShowViewStrategy.ShowMessage(message);

            }
        }

        public virtual void BrowserShippingPlanImportFile_Execute(object sender, SimpleActionExecuteEventArgs e)
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
