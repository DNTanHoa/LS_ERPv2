using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailMessage = LS_ERP.Service.Common.Message;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class SubmitRequest_Win_ViewController
        : SubmitRequest_ViewController
    {
        public override void SubmitPurchaseRequestAction_Execute(object sender, 
            PopupWindowShowActionExecuteEventArgs e)
        {
            base.SubmitPurchaseRequestAction_Execute(sender, e);

            var viewObject = e.PopupWindowViewCurrentObject as SubmitPurchaseRequestParam;
            var purchaseRequest = View.CurrentObject as PurchaseRequest;
            MessageOptions messageOptions = null;

            ///Send mail to manager
            var masterTemplate = ObjectSpace
                .GetObjectByKey<MessageTemplate>("SubmitPurchaseRequestMailTemplate");
            var contentTemplate = ObjectSpace
                .GetObjectByKey<MessageTemplate>("SubmitPurchaseRequestContentMailTemplate");

            var mailContent = masterTemplate.Template?.Replace("[CreatedBy]", 
                SecuritySystem.CurrentUserName);
            mailContent = mailContent.Replace("[Number]",
                purchaseRequest.Number);
            mailContent = mailContent.Replace("[RequestDate]",
                purchaseRequest.RequestDate?.ToString("MM/dd/yyyy HH:mm:ss"));
            mailContent = mailContent.Replace("[CustomerID]",
                purchaseRequest.CustomerID);

            var tableContent = string.Empty;

            foreach (var item in purchaseRequest.PurchaseRequestGroupLines)
            {
                var rowContent = contentTemplate.Template;

                rowContent = rowContent.Replace("[ItemID]", item.ItemID);
                rowContent = rowContent.Replace("[ItemName]", item.ItemName);
                rowContent = rowContent.Replace("[ItemColorCode]", item.ItemColorCode);
                rowContent = rowContent.Replace("[ItemColorName]", item.ItemColorName);
                rowContent = rowContent.Replace("[Style]", item.CustomerStyle);
                rowContent = rowContent.Replace("[Specify]", item.Specify);
                rowContent = rowContent.Replace("[Position]", item.Position);
                rowContent = rowContent.Replace("[Season]", item.Season);
                rowContent = rowContent.Replace("[GarmentColorCode]", item.GarmentColorCode);
                rowContent = rowContent.Replace("[GarmentSize]", item.GarmentSize);
                rowContent = rowContent.Replace("[Unit]", item.UnitID);
                rowContent = rowContent.Replace("[Vendor]", item.VendorID);
                rowContent = rowContent.Replace("[Price]", item.Price?.ToString("G29"));
                rowContent = rowContent.Replace("[Quantity]", item.Quantity?.ToString("G29"));
                rowContent = rowContent.Replace("[Remark]", item.Remarks);

                tableContent += rowContent;
            }

            mailContent = mailContent.Replace("[TableContent]", tableContent);

            ///Create mail service
            var mailService = new MailService();

            ///Make message
            var destinations = new List<string>();
            destinations.Add(viewObject.Email);

            var message = new MailMessage()
            {
                Subject = "Request confirmination for extra purchase - " +
                DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                Destinations = destinations,
                Body = mailContent,
            };

            ///Send submit mail
            var sendMailResult = mailService.SenViaOutLook(message);

            if (sendMailResult)
            {
                var objectSpace = Application.CreateObjectSpace(typeof(PurchaseRequest));
                ///Save purchase request to submitted status
                purchaseRequest.Status = objectSpace.GetObjectByKey<PurchaseRequestStatus>("S");
                purchaseRequest.StatusID = "S";

                ///Create log
                var purchaseRequestLog = objectSpace.CreateObject<PurchaseRequestLog>();
                purchaseRequestLog.ActivityDate = DateTime.Now;
                purchaseRequestLog.Code = DateTime.Now.Ticks.ToString();
                purchaseRequestLog.Description = "Submit request";
                purchaseRequestLog.Remarks = viewObject.Remarks;
                purchaseRequestLog.PurchaseRequestID = purchaseRequest.ID;
                purchaseRequestLog.SetCreateAudit(SecuritySystem.CurrentUserName);

                objectSpace.CommitChanges();

                messageOptions = Message.GetMessageOptions("Submit successfully", "Success",
                    InformationType.Success, null, 2000);
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Submit error, contact your admin", "Error",
                    InformationType.Error, null, 2000);
            }

            if (messageOptions != null)
                Application.ShowViewStrategy.ShowMessage(messageOptions);
        }
    }
}
