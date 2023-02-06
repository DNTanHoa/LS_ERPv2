using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Log
    {
        public long ID { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Exception { get; set; }
        public string LogEvent { get; set; }
        public string Properties { get; set; }
        [NotMapped]
        public XElement PropertiesXml => XElement.Parse(Properties);
    }
}
