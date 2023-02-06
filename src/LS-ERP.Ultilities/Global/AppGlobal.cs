using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Global
{
    public class AppGlobal
    {
        public static string UploadFileDirectory
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                return builder.Build().GetSection("AppSettings").GetSection("UploadFileDirectory").Value;
            }
        }

        public static string UploadFileUrl
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                return builder.Build().GetSection("AppSettings").GetSection("UploadFileUrl").Value;
            }
        }

        public static string Host
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                return builder.Build().GetSection("AppSettings").GetSection("Host").Value;
            }
        }

        public static string TelegramBotToken
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                return builder.Build().GetSection("AppSettings").GetSection("TelegramBotToken").Value;
            }
        }
        
        public static string TelegramChatId
        {
            get
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                return builder.Build().GetSection("AppSettings").GetSection("TelegramChatId").Value;
            }
        }
    }
}
