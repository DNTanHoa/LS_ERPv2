using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class IssuedType
    {
        public IssuedType()
        {
            Id = string.Empty;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
    }
}
