using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Models
{
    public class PagedList<T>
    {
        public int PageIndex { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
