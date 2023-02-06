using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MaterialRequest : Audit
    {
        public int Id { get; set; }
        public string CustomerID { get; set; }
        public string StorageCode { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DueDate { get; set; }
        public string RequestFor { get; set; }
        public string LSStyles { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public bool IsProcess { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Clone 
        /// </summary>
        public int? CloneRequestID { get; set; }    
        public virtual Storage Storage { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual List<MaterialRequestDetail> Details { get; set; }
            = new List<MaterialRequestDetail>();
        public virtual List<MaterialRequestOrderDetail> OrderDetails { get; set; }
            = new List<MaterialRequestOrderDetail>();
    }
}
