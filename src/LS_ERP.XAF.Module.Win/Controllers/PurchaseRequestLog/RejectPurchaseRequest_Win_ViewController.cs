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
using System.Text;
using System.Threading.Tasks;
using MailMessage = LS_ERP.Service.Common.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class RejectPurchaseRequest_Win_ViewController
        : RejectPurchaseRequest_ViewController
    {
        public override void RejectPurchaseRequestSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.RejectPurchaseRequestSubmit_Execute(sender, e);

            var viewObject = View.CurrentObject as PurchaseRequestLog;
            var purchaseRequest = (((DevExpress.ExpressApp.ListView)View).CollectionSource as PropertyCollectionSource)
                             .MasterObject as PurchaseRequest;

            MessageOptions messageOptions = null;

            if (viewObject != null)
            {
                var contentTemplate = ObjectSpace
                    .GetObjectByKey<MessageTemplate>("RejectPurchaseRequestMailTemplate");

                var messageContent = contentTemplate.Template;
                messageContent.Replace("[RejectedBy]", SecuritySystem.CurrentUserName);
                messageContent.Replace("[Number]", purchaseRequest.Number);

                ///Create mail service
                var mailService = new MailService();

                ///Make message
                var destinations = new List<string>();

                var replyUser = ObjectSpace.GetObjects<ApplicationUser>(
                    CriteriaOperator.Parse("[UserName] = ?", viewObject.CreatedBy)).FirstOrDefault();
                destinations.Add(replyUser.Email);

                var message = new MailMessage()
                {
                    Subject = "Reject purchase request submit - " + viewObject.Code + " - " +
                        DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    Destinations = destinations,
                    Body = messageContent,
                };

                ///Send submit mail
                var sendMailResult = mailService.SenViaOutLook(message);

                if (sendMailResult)
                {
                    ///Create log
                    var purchaseRequestLog = ObjectSpace.CreateObject<PurchaseRequestLog>();
                    purchaseRequestLog.ActivityDate = DateTime.Now;
                    purchaseRequestLog.Code = DateTime.Now.Ticks.ToString();
                    purchaseRequestLog.Description = "Reject request";
                    purchaseRequestLog.Remarks = viewObject.Remarks;
                    purchaseRequestLog.PurchaseRequestID = viewObject.PurchaseRequestID;
                    purchaseRequestLog.ReferenceLogCode = viewObject.Code;

                    purchaseRequest.StatusID = "R";

                    ObjectSpace.CommitChanges();
                }
                else
                {
                    messageOptions = Message.GetMessageOptions("Reject error, contact your admin", "Error",
                        InformationType.Error, null, 2000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unknown error. Contact your admin for support",
                    "Error", InformationType.Error, null, 2000);
            }

            if (messageOptions != null)
                Application.ShowViewStrategy.ShowMessage(messageOptions);
        }
    }
}
