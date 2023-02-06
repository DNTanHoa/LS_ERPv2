using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class FabricPurchaseOrder : Audit
    {
        public FabricPurchaseOrder()
        {
            Number = string.Empty;
        }

        public long? ID { get; set; }
        public string Number { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }

        [XafDisplayName("EDD Week")]
        public string ExpectedDeliveryDateWeek { get; set; }
        public string Line { get; set; }
        public string FabricSupplier { get; set; }
        public string CustomerID { get; set; }
        [XafDisplayName("ETD")]
        public DateTime? EstimatedTimeOfDeparture { get; set; } // original ETD

        [XafDisplayName("Updated ETD")]
        public DateTime? UpdatedEstimatedTimeOfDeparture { get; set; }

        [XafDisplayName("EDD")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [XafDisplayName("CDD")]
        public DateTime? ContractualDeliveryDate { get; set; }
        public DateTime? OrderCreationDate { get; set; }
        public DateTime? ProductionStartDate { get; set; }
        public string Note { get; set; }
        public string UnitID { get; set; }

        public decimal? OrderedQuantity { get; set; }
        public decimal? ShippedQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? OnHandQuantity { get; set; }
        public decimal? DeliveredQuantity { get; set; }

        public string SupplierContactName { get; set; }
        public string CustomerStyles { get; set; }
        public string GarmentColorCodes { get; set; }
        public string Seasons { get; set; }
        public string FileName { get; set; }
        public string ServerFileName { get; set; }
        public string FilePath { get; set; }
        public string UserFollow { get; set; }
        public bool? Offset { get; set; }
        public string ProductionMethodCode { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual PriceTerm ProductionMethod { get; set; }
        
    }
}
