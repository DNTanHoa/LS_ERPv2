using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Models
{
    public class TreeList<T>
    {
        public T Node { get; set; }
        public IEnumerable<TreeList<T>> Childrens { get; set; }
    }
}
