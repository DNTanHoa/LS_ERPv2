using LS_ERP.Ultilities.Global;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Helpers
{
    public class LogHelper
    {
        private static Logger _instance;

        protected LogHelper()
        {

        }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Telegram(AppGlobal.TelegramBotToken, AppGlobal.TelegramChatId).CreateLogger();
                }
                return _instance;
            }
        }
    }
}
