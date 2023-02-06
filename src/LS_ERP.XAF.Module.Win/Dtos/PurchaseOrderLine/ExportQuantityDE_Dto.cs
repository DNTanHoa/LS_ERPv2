using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Win.Dtos
{
    public class ExportQuantityDE_Dto
    {
        public string LSStyle { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderID { get; set; }
        public string ProductionLine { get; set; }
        public DateTime? ContactSupplierHandover { get; set; }
        public string OrderType { get; set; }

        public string Model { get; set; }
        public string Designation { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
        public string PCB { get; set; }
        public string UE { get; set; }
        public string Packing { get; set; }
        public string IMan { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string DeliveryPlace { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? ContractualDeliveryDate { get; set; }
        public DateTime? EstimatedSupplierHandover { get; set; }

    }
}
