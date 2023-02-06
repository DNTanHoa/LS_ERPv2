using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Ultils.Extensions
{
    public static class ObjectExtension
    {
        public static Dictionary<string, string>? ToDictionaryStringString(this object obj)
        {
            if (obj != null)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                PropertyInfo[] props = obj.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (!string.IsNullOrEmpty(prop.GetValue(obj)?.ToString()))
                    {
                        result.Add(prop.Name, prop.GetValue(obj)?.ToString()!);
                    }
                }
                return result;
            }
            return null;
        }
        public static SortedList<string, string> ToSortedList(this object obj, IComparer<string> comparer)
        {
            SortedList<string, string> result = new SortedList<string, string>(comparer);
            var dictionary = obj.ToDictionaryStringString();

            foreach (var key in dictionary!.Keys)
            {
                result.Add(key, dictionary[key]);
            }

            return result;
        }

        public static string ToQueryString(this object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            string queryString = String.Join("&", properties.ToArray());
            return queryString;
        }
    }
}

