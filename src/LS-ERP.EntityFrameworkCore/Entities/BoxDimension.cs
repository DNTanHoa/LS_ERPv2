using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class BoxDimension : Audit
    {
        public BoxDimension()
        {
            this.Code = string.Empty;
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
    }
}
