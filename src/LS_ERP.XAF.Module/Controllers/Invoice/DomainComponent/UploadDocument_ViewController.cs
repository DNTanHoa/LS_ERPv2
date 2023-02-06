using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service.Invoice;
using LS_ERP.XAF.Module.Service.Request;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UploadDocument_ViewController : ObjectViewController<DetailView, Invoice>
    {
        public UploadDocument_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupInvoiceDocument = new PopupWindowShowAction(this,
                "PopupInvoiceDocument", PredefinedCategory.Unspecified);
            popupInvoiceDocument.ImageName = "ActionGroup_EasyTestRecorder";
            popupInvoiceDocument.Caption = "Upload Document";
            //popupInvoiceDocument.TargetObjectType = typeof(Invoice);
            //popupInvoiceDocument.TargetViewType = ViewType.DetailView;
            popupInvoiceDocument.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //popupInvoice.Shortcut = "CtrlShiftO";

            popupInvoiceDocument.CustomizePopupWindowParams += PopupInvoiceDocument_CustomizePopupWindowParams;
            popupInvoiceDocument.Execute += PopupInvoiceDocument_Execute;

        }

        private void PopupInvoiceDocument_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as InvoiceDocumentParam;
            var service = new InvoiceService();
            var invoice = e.CurrentObject as Invoice;
            var messageOptions = new MessageOptions();

            var request = new UploadDocumentRequest()
            {
                InvoiceDocumentTypeID = importParam.InvoiceDocumentType.ID,
                FilePath = importParam.ImportFilePath,
                UserName = SecuritySystem.CurrentUserName
            };

            var importResponse = service.UploadDocument(request).Result;

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);

                    InvoiceDocument invoiceDocument = new InvoiceDocument();
                    invoiceDocument.FileName = importResponse.Data?.FileName;
                    invoiceDocument.FileNameServer = importResponse.Data?.FileNameServer;
                    invoiceDocument.FilePath = importResponse.Data?.FilePath;
                    invoiceDocument.InvoiceDocumentTypeID = importResponse.Data.InvoiceDocumentTypeID;
                    invoiceDocument.SetCreateAudit(SecuritySystem.CurrentUserName);

                    if (invoice.InvoiceDocument == null)
                    {
                        invoice.InvoiceDocument = new List<InvoiceDocument>();
                    }

                    invoice.InvoiceDocument.Add(invoiceDocument);
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

        private void PopupInvoiceDocument_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            //var viewObject = View.CurrentObject as Invoice;

            var model = new InvoiceDocumentParam();
            model.InvoiceDocumentType = objectSpace.GetObjects<InvoiceDocumentType>().FirstOrDefault();

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            //e.Maximized = true;
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
