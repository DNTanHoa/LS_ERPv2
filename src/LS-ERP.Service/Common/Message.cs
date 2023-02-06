using System;
using System.Collections.Generic;
using System.Text;

namespace LS_ERP.Service.Common
{
    public class Message
    {
        public string Subject { get; set; }
        public string Destination { get; set; }
        public string From { get; set; }
        public string Body { get; set; }

        public List<string> Destinations { get; set; } = new List<string>();


        /// <summary>
        /// For send mail
        /// </summary>
        public List<string> CCs { get; set; } = new List<string>();
        public List<string> BCCs { get; set; } = new List<string>();
        public string AttachFile { get; set; }
    }
}
