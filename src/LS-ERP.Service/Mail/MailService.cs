using LS_ERP.Service.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Diagnostics;
using OutLook = Microsoft.Office.Interop.Outlook;

namespace LS_ERP.Service.Mail
{
    public interface IMailService
    {
        public bool SenViaOutLook(Message message);
        public bool SendViaGmail(Message message, ConfigSMTP configSMTP, AlternateView alternateView);
    }

    public class MailService : IMailService
    {
        public bool SendViaGmail(Message message, ConfigSMTP configSMTP, AlternateView alternateView)
        {
            var smtp = new SmtpClient
            {
                Host = configSMTP.Host,
                Port = configSMTP.Port,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configSMTP.FromAddress, configSMTP.Password),
                Timeout = 30000
            };
            var sendMessage = new MailMessage(configSMTP.FromAddress, message.Destination);
            sendMessage.Subject = message.Subject;
            sendMessage.Body = message.Body;
            if (message.CCs != null)
            {
                foreach (var cc in message.CCs)
                {
                    sendMessage.CC.Add(cc);
                }
            }
            if (message.BCCs != null)
            {
                foreach (var bcc in message.BCCs)
                {
                    sendMessage.Bcc.Add(bcc);
                }
            }
            if (!string.IsNullOrEmpty(message.AttachFile))
            {
                sendMessage.Attachments.Add(new Attachment(message.AttachFile));
            }
            if (alternateView != null)
            {
                sendMessage.AlternateViews.Add(alternateView);
            }
            sendMessage.IsBodyHtml = true;
            smtp.Send(sendMessage);
            return true;
        }

        public bool SenViaOutLook(Message message)
        {
            int outlookInstanceCount = System.Diagnostics.Process.GetProcessesByName("OUTLOOK").Length;

            /// check outlook is open
            if (outlookInstanceCount > 0)
            {
                var outLookApplication = new OutLook.Application();
                var outLookMessage = (OutLook.MailItem)outLookApplication.CreateItem(OutLook.OlItemType.olMailItem);

                OutLook.Recipients outLookRecipients = outLookMessage.Recipients;
                foreach (string destination in message.Destinations)
                {
                    OutLook.Recipient oTORecipt = outLookRecipients.Add(destination);
                    oTORecipt.Type = (int)OutLook.OlMailRecipientType.olTo;
                    oTORecipt.Resolve();
                }

                if (message.CCs != null && message.CCs.Count > 0)
                {
                    foreach (var cc in message.CCs)
                    {
                        OutLook.Recipient oTORecipt = outLookRecipients.Add(cc);
                        oTORecipt.Type = (int)OutLook.OlMailRecipientType.olCC;
                        oTORecipt.Resolve();
                    }
                }

                outLookMessage.Subject = message.Subject;
                outLookMessage.HTMLBody = message.Body;

                try
                {
                    outLookMessage.Save();
                    outLookMessage.Send();

                    outLookApplication = null;
                    outLookRecipients = null;
                    outLookMessage = null;

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
