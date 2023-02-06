using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.API.Configuration
{
    public class MessageConfiguration
    {
        public static List<Message> Messages { get; set; }
    }

    public class Message
    {
        public string Code { get; set; }
        public string MessageContent { get; set; }
    }
}
