using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ItemStyleDto
    {
        /// <summary>
        /// Sale order infor
        /// </summary>
        public string SalesOrderID { get; set; }
        public string Number { get; set; }
        public string Season { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string ItemStyleStatusCode { get; set; }
        public DateTime? ShipDate { get; set; }

        /// <summary>
        /// Style infor
        /// </summary>
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string Description { get; set; }
        public string ProductionDescription { get; set; }
        public string Brand { get; set; }
        public string ContractNo { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Division { get; set; }
        public string LabelCode { get; set; }
    }
}
