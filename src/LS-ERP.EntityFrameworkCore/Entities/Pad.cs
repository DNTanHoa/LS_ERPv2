using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Pad : Audit
    {
        public Pad()
        {
            Code = Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRST123456789", 12);
        }

        public string Code { get; set; }
        public string Description { get; set; }
    }
}
