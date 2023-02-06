using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Helpers
{
    public class Message
    {
        public static MessageOptions GetMessageOptions(string message,
            string caption,
            InformationType type,
            Action OKDelegate,
            int duration = 2000)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = duration;
            options.Message = message;
            options.Type = type;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            options.OkDelegate = OKDelegate;
            options.CancelDelegate = null;
            return options;
        }
    }
}
