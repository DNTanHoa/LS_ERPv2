using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Helpers.JsonHelpers
{
    public static class AuditLogSerializer
    {
        public static readonly JsonSerializerSettings DefaultAuditJsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.Indented
        };

        public static readonly JsonSerializerSettings BaseAuditEventJsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.Indented
        };

        static AuditLogSerializer()
        {
            DefaultAuditJsonSettings.Converters.Add(new StringEnumConverter());
            BaseAuditEventJsonSettings.Converters.Add(new StringEnumConverter());
            BaseAuditEventJsonSettings.ContractResolver = new AuditLoggerContractResolver();
        }

        /// <summary>
        /// Serializes the audit event object.
        /// </summary>
        /// <param name="logObject">The object.</param>
        /// <returns></returns>
        public static string Serialize(object logObject)
        {
            return JsonConvert.SerializeObject(logObject, DefaultAuditJsonSettings);
        }

        /// <summary>
        /// Serializes the audit event object.
        /// </summary>
        /// <param name="logObject">The object.</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string Serialize(object logObject, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(logObject, settings);
        }
    }
}
