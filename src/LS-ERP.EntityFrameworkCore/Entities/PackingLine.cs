using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PackingLine : Audit
    {
        public int ID { get; set; }
        public string SequenceNo { get; set; }
        public string LSStyle { get; set; }
        public decimal? QuantitySize { get; set; } // for style canada
        public decimal? QuantityPerPackage { get; set; }
        public decimal? PackagesPerBox { get; set; }
        public decimal? QuantityPerCarton { get; set; } // QtyPerCTNS
        public decimal? TotalQuantity { get; set; } // QtyPCS
        public decimal? NetWeight { get; set; }
        public decimal? GrossWeight { get; set; }
        public string Color { get; set; }
        public string PrePack { get; set; } // = field size of old version
        public string Size { get; set; }
        public decimal? Quantity { get; set; }
        public string PackingListCode { get; set; }
        public string BoxDimensionCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string InnerBoxDimensionCode { get; set; }
        public decimal? InnerLength { get; set; }
        public decimal? InnerWidth { get; set; }
        public decimal? InnerHeight { get; set; }
        public int? FromNo { get; set; }
        public int? ToNo { get; set; }
        public int? TotalCarton { get; set; }
        public string DeliveryPlace { get; set; }

        public string PadCode { get; set; }

        /// using GA scan barcode
        public string BarCode { get; set; } 
        public int? TotalBarCode { get; set; }

        private BoxDimension boxDimension;
        public virtual BoxDimension BoxDimension
        {
            get => boxDimension;
            set
            {
                this.boxDimension = value;
                if (value != null)
                {
                    Width = value.Width;
                    Height = value.Height;
                    Length = value.Length;
                    this.GrossWeight = this.NetWeight + this.BoxDimension?.Weight * this.TotalCarton
                        + (this.InnerBoxDimension?.Weight * this.TotalCarton * this.PackagesPerBox ?? 0);
                }
            }
        }

        private BoxDimension innerBoxDimension;
        public virtual BoxDimension InnerBoxDimension
        {
            get => innerBoxDimension;
            set
            {
                this.innerBoxDimension = value;
                if (value != null)
                {
                    InnerWidth = value.Width;
                    InnerHeight = value.Height;
                    InnerLength = value.Length;
                    this.GrossWeight = this.NetWeight + this.BoxDimension?.Weight * this.TotalCarton
                        + (this.InnerBoxDimension?.Weight * this.TotalCarton * this.PackagesPerBox ?? 0);
                }
            }
        }
        public virtual PackingList PackingList { get; set; }
        public virtual Pad Pad { get; set; }

        public string HangerCode { get; set; }
        public string UnitID { get; set; }
        public virtual Hanger Hanger { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
