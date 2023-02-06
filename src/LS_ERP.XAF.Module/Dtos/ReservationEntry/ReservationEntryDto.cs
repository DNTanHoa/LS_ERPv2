namespace LS_ERP.XAF.Module.Dtos
{
    public class ReservationEntryDto
    {
        public long ID { get; set; }

        /// <summary>
        /// Reservation between order and production information
        /// </summary>
        public string JobHeadNumber { get; set; }
        public long? OrderDetailID { get; set; }

        /// <summary>
        /// Reservation between finish goods with stock or production bom with stock
        /// </summary>
        public long? StorageDetailID { get; set; }

        /// <summary>
        /// Reservation between purchase order and production bom
        /// </summary>
        public long? PurchaseOrderLineID { get; set; }
        public long? ProductionBOMID { get; set; }


        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
    }
}