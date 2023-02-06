using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Helpers.Common
{
    public static class TypeHelpers
    {
        public static string GetNameWithoutGenericParams(this Type type)
        {
            return type.IsGenericType ? type.Name.Remove(type.Name.IndexOf('`')) : type.Name;
        }
    }
}
