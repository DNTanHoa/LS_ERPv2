using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.SalesContract;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportSalesContract_ViewController : ViewController
    {
        public ImportSalesContract_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importSalesOrder = new PopupWindowShowAction(this, "ImportSalesContract", PredefinedCategory.Unspecified);
            importSalesOrder.ImageName = "Import";
            importSalesOrder.TargetObjectType = typeof(SalesContractSearchParam);
            importSalesOrder.TargetViewType = ViewType.DetailView;
            importSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importSalesOrder.CustomizePopupWindowParams += ImportSalesContract_CustomizePopupWindowParams;
            importSalesOrder.Execute += ImportSalesContract_Execute;
        }

        private void ImportSalesContract_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as SalesContractImportParam;
            var service = new SalesContractService();
            var request = new ImportSalesContractRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.ImportFilePath,
                UserName = SecuritySystem.CurrentUserName
            };
            var importResponse = service.ImportSalesContract(request).Result;
            var messageOptions = new MessageOptions();

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            ObjectSpace.CommitChanges();
            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        private void ImportSalesContract_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesContractImportParam();
            model.Customer = objectSpace.GetObjects<Customer>().FirstOrDefault();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
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
