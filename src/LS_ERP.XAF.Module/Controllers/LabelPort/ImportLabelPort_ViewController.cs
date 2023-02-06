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
    public partial class ImportLabelPort_ViewController : ObjectViewController<ListView, LabelPort>
    {
        public ImportLabelPort_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction importLabelPort = new PopupWindowShowAction(this, "ImportLabelPort", PredefinedCategory.Unspecified);
            importLabelPort.Caption = "Import";
            importLabelPort.ImageName = "Import";
            importLabelPort.TargetObjectType = typeof(LabelPort);
            importLabelPort.TargetViewType = ViewType.ListView;
            importLabelPort.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importLabelPort.CustomizePopupWindowParams += ImportLabelPort_CustomizePopupWindowParams;
            importLabelPort.Execute += ImportLabelPort_Execute;

        }

        void ImportLabelPort_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as ImportLabelPortParam;
            var service = new LabelPortService();
            var messageOptions = new MessageOptions();

            var request = new ImportLabelPortRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.FilePath,
                UserName = SecuritySystem.CurrentUserName
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

        private void ImportLabelPort_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ImportLabelPortParam()
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
