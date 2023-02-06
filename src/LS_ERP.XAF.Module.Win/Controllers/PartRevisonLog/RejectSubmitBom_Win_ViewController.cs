using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.BusinessObjects;
using LS_ERP.XAF.Module.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailMessage = LS_ERP.Service.Common.Message;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class RejectSubmitBom_Win_ViewController : RejectSubmit_ViewController
    {
        public override void RejectBomSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.RejectBomSubmit_Execute(sender, e);

            var partRevisionLog = View.CurrentObject as PartRevisionLog;
            MessageOptions options = null;

            if (partRevisionLog.PartRevision != null)
            {
                partRevisionLog.PartRevision.IsConfirmed = true;

                /// Set part material to approved status
                foreach (var partRevisionLogDetail in partRevisionLog.PartRevisionLogDetails)
                {
                    partRevisionLogDetail.PartMaterial.PartMaterialStatusCode = "2";
                }

                /// Create log
                var approvePartRevisionLog = ObjectSpace.CreateObject<PartRevisionLog>();
                approvePartRevisionLog.Code = DateTime.Now.ToString("yyyyMMddHHmmssfff"); ;
                approvePartRevisionLog.PartRevisionLogReferenceCode = partRevisionLog.Code;
                approvePartRevisionLog.ActivityDate = DateTime.Now;
                approvePartRevisionLog.PartRevisionID = partRevisionLog.PartRevisionID;
                approvePartRevisionLog.Description = "Reject submit " + partRevisionLog.Code;
                approvePartRevisionLog.SetCreateAudit(SecuritySystem.CurrentUserName);

                /// Send mail to submit persion
                ///Create mail service
                var mailService = new MailService();

                ///Make message
                var contentTemplate = ObjectSpace.GetObjectByKey<MessageTemplate>("RejectBOMTemplate");
                var content = contentTemplate.Template;

                var rejectUser = ObjectSpace.GetObjects<ApplicationUser>()
                    .FirstOrDefault(x => x.UserName == SecuritySystem.CurrentUserName);

                content = content.Replace("[FullName]", rejectUser.FullName);
                content = content.Replace("[PartNum]", partRevisionLog.PartRevision?.PartNumber);
                content = content.Replace("[RevNum]", partRevisionLog.PartRevision?.RevisionNumber);
                content = content.Replace("[Season]", partRevisionLog.PartRevision?.Season);
                content = content.Replace("[EffectDate]", partRevisionLog.PartRevision?
                    .EffectDate?.ToString("yyyy-MM-dd"));

                var destinations = new List<string>();
                var user = ObjectSpace.GetObjects<ApplicationUser>()
                    .FirstOrDefault(x => x.UserName == partRevisionLog.CreatedBy);
                destinations.Add(user.Email);
                var message = new MailMessage()
                {
                    Subject = "Reject submit for master BOM - " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    Destinations = destinations,
                    Body = content,
                };

                ///Send approved mail
                var sendMailResult = mailService.SenViaOutLook(message);

                if (sendMailResult)
                {
                    options = Message.GetMessageOptions("Reject successfully.", "Success",
                       InformationType.Success, null, 5000);
                }
                else
                {
                    options = Message.GetMessageOptions("Send mail error check your outlook mail", "Error",
                        InformationType.Error, null, 5000);
                }

                ObjectSpace.CommitChanges();
                View.Refresh();
            }
        }
    }
}
