using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ImportDailyTarget_ViewController : ViewController
    {
        public ImportDailyTarget_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importDailyTarget = new PopupWindowShowAction(this, "ImportDailyTarget", PredefinedCategory.Unspecified);
            importDailyTarget.ImageName = "Import";
            importDailyTarget.TargetObjectType = typeof(DailyTargetSearchParam);
            importDailyTarget.TargetViewType = ViewType.DetailView;
            importDailyTarget.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importDailyTarget.CustomizePopupWindowParams += ImportDailyTarget_CustomizePopupWindowParams;
            importDailyTarget.Execute += ImportDailyTarget_Execute;
        }

        private void ImportDailyTarget_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as DailyTargetImportParam;
            var service = new DailyTargetService();
            var messageOptions = new MessageOptions();

            var request = new BulkDailyTargetRequest()
            {
                Data = importParam.Data,
                UserName = SecuritySystem.CurrentUserName
            };

                var importResponse = service.Bulk(request).Result;

                if (importResponse != null)
                {
                    if (importResponse.Success)
                    {
                        messageOptions = Message.GetMessageOptions(importResponse.Message, "Success",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        messageOptions = Message.GetMessageOptions(importResponse.Message, "Error",
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

        private void ImportDailyTarget_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new DailyTargetImportParam();
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
