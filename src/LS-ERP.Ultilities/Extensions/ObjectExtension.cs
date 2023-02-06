using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Extensions
{
    public static class ObjectExtensions
    {
        public static void MapFromDictionary(this object obj, Dictionary<string, string> dict)
        {
            if (obj != null &&
               dict != null)
            {
                var props = obj.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (dict.ContainsKey(prop.Name) &&
                        prop.CanWrite)
                    {
                        prop.SetValue(obj: obj,
                            value: Convert.ChangeType(dict[prop.Name], prop.PropertyType));
                    }
                }
            }
        }

        public static Dictionary<string, object> ToDictionaryStringString(this object obj)
        {
            if (obj != null)
            {
                return obj.GetType().GetProperties().ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(obj, null)
                );
            }
            return null;
        }

        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeType(this object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
    }
}
