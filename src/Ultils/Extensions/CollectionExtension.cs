using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Models;

namespace Ultils.Extensions
{
    public static class CollectionExtension
    {
        public static PagedList<T> ToPagedList<T>(this List<T> data, int pageSize, int pageIndex)
        {
            var result = new PagedList<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = data.Skip((pageIndex - 1) * pageSize).Take(pageSize),
                TotalCount = data.Count()
            };

            return result;
        }

        public static IEnumerable<TreeList<T>> ToTreeList<T, K>(this ICollection<T> data,
            Func<T, K> id,
            Func<T, K> parentId,
            K root = default(K)!)
        {
            foreach (var c in data.Where(c => EqualityComparer<K>.Default.Equals(parentId(c), root)))
            {
                yield return new TreeList<T>
                {
                    Node = c,
                    Childrens = data.ToTreeList(id, parentId, id(c))
                };
            }
        }

        public static DataTable ToDataTable<T>(this ICollection<T> data)
        {
            if (data != null)
            {
                if (data.Any())
                {
                    var result = new DataTable();
                    var properties = data.First()!.GetType().GetProperties();

                    properties.ToList().ForEach(x =>
                    {
                        result.Columns.Add(x.Name, Nullable.GetUnderlyingType(x.PropertyType) ?? x.PropertyType);
                    });

                    data.ToList().ForEach(x =>
                    {
                        DataRow row = result.NewRow();
                        properties.ToList().ForEach(p =>
                        {
                            row[p.Name] = p.GetValue(x);
                        });

                        result.Rows.Add(row);
                    });
                }
            }

            return null;
        }
    }
}
