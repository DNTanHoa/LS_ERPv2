using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportPartRevision_ViewController : ViewController
    {
        public ImportPartRevision_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importPartRevision = new PopupWindowShowAction(this, "ImportPartRevision", PredefinedCategory.Unspecified);
            importPartRevision.ImageName = "Import";
            importPartRevision.Caption = "Import (Ctrl + I)";
            importPartRevision.TargetObjectType = typeof(PartRevision);
            importPartRevision.TargetViewType = ViewType.ListView;
            importPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importPartRevision.Shortcut = "CtrlI";

            importPartRevision.Execute += ImportPartRevision_Execute;
            importPartRevision.CustomizePopupWindowParams += ImportPartRevision_CustomizePopupWindowParams;
        }

        private void ImportPartRevision_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ImportPartRevisionParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
                EffectDate = DateTime.Today,
            };
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.View = view;
        }

        private void ImportPartRevision_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as ImportPartRevisionParam;
            MessageOptions messageOptions = null;

            if (viewObject != null && (viewObject.Customer != null && !viewObject.Customer.ID.Equals("PU")))
            {
                var service = new PartRevisionService();
                var request = new CreatePartRevisionRequest()
                {
                    RevisionNumber = viewObject.RevisionNumber,
                    PartNumber = viewObject.StyleNumber,
                    IsConfirmed = viewObject.IsConfirmed,
                    EffectDate = viewObject.EffectDate,
                    CustomerID = viewObject.Customer?.ID,
                    Season = viewObject.Season,
                    UserName = SecuritySystem.CurrentUserName,
                    PartMaterials = viewObject.PartMaterials,
                    Items = viewObject.Items,
                    FileNameServer = viewObject.FileNameServer,
                    FileName = viewObject.FileName
                };

                var response = service.CreatePartRevision(request).Result;

                if (response.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Confirm successfully", "Success", InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(response.Result?.Message, "Error", InformationType.Error, null, 5000);
                }
            }
            else
            {
                if (viewObject.Customer != null && viewObject.Customer.ID.Equals("PU"))
                {
                    messageOptions = Message.GetMessageOptions("Confirm successfully", "Success", InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions("Unknown error. Contact your admin for support", "Error", InformationType.Error, null, 5000);

                }

            }

            Application.ShowViewStrategy.ShowMessage(messageOptions);

            ObjectSpace.CommitChanges();
            View.Refresh();
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
