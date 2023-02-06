using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PackingListSeparateParam
    {
        public List<SeparatePackingLine> Details { get; set; } = new List<SeparatePackingLine>();
    }

    [DomainComponent]
    public class SeparatePackingLine
    {
        public string SequenceNo { get; set; }
        public int? FromNo { get; set; }
        public int? ToNo { get; set; }
        public string LSStyle { get; set; }
        public decimal? QuantitySize { get; set; } // for style canada
        public decimal? QuantityPerPackage { get; set; }
        public decimal? PackagesPerBox { get; set; }
        public decimal? QuantityPerCarton { get; set; } // QtyPerCTNS
        public decimal? TotalQuantity { get; set; } // QtyPCS
        public string Size { get; set; }
        public decimal? Quantity { get; set; }
        public string PrePack { get; set; } // = field size of old version
        public string BoxDemensionCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? MeasM3 { get; set; }
        public decimal? TotalMeasM3 { get; set; }

        private int totalCarton;
        public int TotalCarton
        {
            get => this.totalCarton;
            set
            {
                this.totalCarton = value;
                if (value > 0)
                {
                    this.MeasM3 = Math.Round((decimal)(this.totalCarton * 
                                            (this.Length * (decimal)2.54) *
                                            (this.Width * (decimal)2.54) * 
                                            (this.Height * (decimal)2.54)/
                                            1000000 ?? 0), 3);

                    //this.TotalMeasM3 = Math.Round((decimal)(this.Length *
                    //               this.Width * this.Height * (decimal)2.54 *
                    //               this.totalCarton / 1000000 ?? 0), 3);
                }
                else
                {
                    this.MeasM3 = 0;
                    //.TotalMeasM3 = 0;
                }
            }
        }
    }
}
