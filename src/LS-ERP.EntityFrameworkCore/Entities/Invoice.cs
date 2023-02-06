using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Invoice : Audit
    {
        public Invoice()
        {
            this.Code = String.Empty;
            this.Date = DateTime.Now;
        }

        public long? ID { get; set; }

        [XafDisplayName("Invoice No")]
        public string Code { get; set; }
        [XafDisplayName("Invoice Date")]
        public DateTime? Date { get; set; }
        public long? PortOfLoadingID { get; set; }
        public long? PortOfDischargeID { get; set; }
        public long? ConsigneeID { get; set; }
        public string CompanyCode { get; set; }
        public string PaymentTermCode { get; set; }
        public string IncotermCode { get; set; }
        public string VesselVoyageNo { get; set; }
        public string ContainerNo { get; set; }
        public string SealNumber { get; set; }
        public string FlightNo { get; set; }
        public long? InvoiceTypeID { get; set; }
        public DateTime? OnBoardDate { get; set; }
        public string CustomerInvoiceNo { get; set; }

        [XafDisplayName("Applicant")]
        public string CustomerID { get; set; }
        public string ShipmentCode { get; set; }

        [XafDisplayName("ETA")]
        public DateTime? EstimatedTimeOfArrival { get; set; }

        [XafDisplayName("ETD")]
        public DateTime? EstimatedTimeOfDeparture { get; set; }

        public long? FinalDestinationID { get; set; }
        public long? ShipToID { get; set; }

        public string Description { get; set; }
        public bool? IsConfirmed { get; set; }

        public virtual Consignee Consignee { get; set; }
        public virtual Consignee ShipTo { get; set; }
        public virtual Port PortOfLoading { get; set; }
        public virtual Port PortOfDischarge { get; set; }
        public virtual Port FinalDestination { get; set; }
        public virtual Company Company { get; set; }
        public virtual PaymentTerm PaymentTerm { get; set; }
        public virtual IncoTerm IncoTerm { get; set; }
        public virtual InvoiceType InvoiceType { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ShippingMethod Shipment { get; set; }
        public virtual IList<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual IList<Consignee> NotifyParties { get; set; }
        public virtual IList<PackingList> PackingList { get; set; }
        public virtual IList<InvoiceDocument> InvoiceDocument { get; set; }
    }
}
