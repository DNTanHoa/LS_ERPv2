using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportPart_ViewController : ObjectViewController<ListView, Part>
    {
        public ImportPart_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importPart = new PopupWindowShowAction(this, "ImportPart", PredefinedCategory.Unspecified);
            importPart.Caption = "Import";
            importPart.ImageName = "Import";
            importPart.TargetObjectType = typeof(Part);
            importPart.TargetViewType = ViewType.ListView;
            importPart.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importPart.CustomizePopupWindowParams += ImportPart_CustomizePopupWindowParams;
            importPart.Execute += ImportPart_Execute;

        }

        void ImportPart_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as ImportPartParam;
            var service = new PartService();
            var messageOptions = new MessageOptions();

            var request = new ImportPartRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.FilePath,
                UserName = SecuritySystem.CurrentUserName,
                Update = importParam.Update,
                SalesOrderIDs = importParam.SalesOrderIDs
            };

            var importResponse = service.Import(request).Result;

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

        private void ImportPart_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ImportPartParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
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
