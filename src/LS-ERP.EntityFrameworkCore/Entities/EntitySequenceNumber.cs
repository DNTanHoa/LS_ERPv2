using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class EntitySequenceNumber
    {
        public EntitySequenceNumber()
        {
            this.Code = string.Empty;       
        }

        public string Code { get; set; }
        public string Prefix { get; set; }
        public string Subfix { get; set; }
        public int? LastNumber { get; set; }
        public int? NumberLength { get; set; }
        public char? FillLengtCharacter { get; set; }
    }
}
