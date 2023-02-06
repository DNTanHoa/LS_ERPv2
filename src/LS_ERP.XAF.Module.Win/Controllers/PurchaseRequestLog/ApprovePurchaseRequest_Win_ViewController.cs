using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.BusinessObjects;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using MailMessage = LS_ERP.Service.Common.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ApprovePurchaseRequest_Win_ViewController
        : ApprovePurchaseRequest_ViewController
    {
        public override void ApprovePurchaseRequestSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ApprovePurchaseRequestSubmit_Execute(sender, e);

            var requestLog = View.CurrentObject as PurchaseRequestLog;
            var purchaseRequest = (((DevExpress.ExpressApp.ListView)View).CollectionSource as PropertyCollectionSource)
                             .MasterObject as PurchaseRequest;


            MessageOptions messageOptions = null;

            if (requestLog != null)
            {
                var contentTemplate = ObjectSpace
                    .GetObjectByKey<MessageTemplate>("ApprovePurchaseRequestMailTemplate");

                var messageContent = contentTemplate.Template;
                messageContent.Replace("[ApprovedBy]", SecuritySystem.CurrentUserName);
                messageContent.Replace("[Number]", purchaseRequest.Number);

                ///Create mail service
                var mailService = new MailService();

                ///Make message
                var destinations = new List<string>();

                var replyUser = ObjectSpace.GetObjects<ApplicationUser>(
                    CriteriaOperator.Parse("[UserName] = ?", requestLog.CreatedBy)).FirstOrDefault();
                destinations.Add(replyUser.Email);

                var message = new MailMessage()
                {
                    Subject = "Approve purchase request submit - " + requestLog.Code + " - " +
                        DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    Destinations = destinations,
                    Body = messageContent,
                };

                ///Send submit mail
                var sendMailResult = mailService.SenViaOutLook(message);

                if(sendMailResult)
                {
                    var approveLog = ObjectSpace.CreateObject<PurchaseRequestLog>();
                    approveLog.ReferenceLogCode = requestLog.Code;
                    approveLog.Code = DateTime.Now.Ticks.ToString();
                    approveLog.Description = "Approve submit " + requestLog.Code;
                    approveLog.PurchaseRequestID = requestLog.PurchaseRequestID;
                    approveLog.ActivityDate = DateTime.Now;

                    requestLog.PurchaseRequest.StatusID = "A";

                    //TODO: Handle send mail to notification
                    messageOptions = Message.GetMessageOptions("Approve successfully", "Success",
                        InformationType.Success, null, 5000);

                    ObjectSpace.CommitChanges();
                    View.Refresh(true);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions("Unknown error, contat your admin for support",
                        "Error", InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unknown error, contat your admin for support",
                    "Error", InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }
    }
}
