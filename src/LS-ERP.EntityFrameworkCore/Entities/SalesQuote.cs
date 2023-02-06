using DevExpress.Persistent.Base;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesQuote : Audit
    {
        public SalesQuote()
        {
            this.SalesQuoteLogs = new List<SalesQuoteLog>();
            this.SalesQuoteDetails = new List<SalesQuoteDetail>();
        }
        public long ID { get; set; }
        public string SalesOrderID { get; set; }
        public string CustomerID { get; set; }
        public string DivisionID { get; set; }
        public string CurrencyID { get; set; }
        public string CustomerStyle { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string PrepareBy { get; set; }
        public string ApprovedBy { get; set; }
        public string TargetFOBPrice { get; set; }
        public DateTime? CostingDate { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public string CurrencyExchangeTypeID { get; set; }
        public string PriceTermCode { get; set; }
        public string FactoryCode { get; set; }
        public string SalesQuoteStatusCode { get; set; }
        public decimal? ExchangeValue { get; set; }
        public decimal? Labour { get; set; }
        public decimal? Profit { get; set; }
        public decimal? TestingFee { get; set; }
        public decimal? CMTPrice { get; set; }
        public decimal? Discount { get; set; }
        public string SizeRun { get; set; }
        public string Season { get; set; }
        public string GenderID { get; set; }

        /// <summary>
        /// 0: public
        /// 1: local
        /// </summary>
        public int Level { get; set; } = 0;

        [NotMapped]
        [ImageEditor(ListViewImageEditorMode = ImageEditorMode.PictureEdit,
           DetailViewImageEditorMode = ImageEditorMode.PictureEdit,
           DetailViewImageEditorFixedHeight = 150)]
        public byte[] ImageData
        {
            get => SaveFileHelpers.Dowload(this.Image);
            set
            {
                if (value != null)
                {
                    this.Image = SaveFileHelpers.Upload(value,
                        Nanoid.Nanoid.Generate("ABCDEFGHIJKLMONPRSTUV0123456789", 8) + ".png",
                        AppGlobal.UploadFileUrl);
                }
            }
        }

        public virtual SalesOrder SalesOrder { get; set; }
        public virtual SalesQuoteStatus SalesQuoteStatus { get; set;}
        public virtual Customer Customer { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Division Division { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual CurrencyExchangeType CurrencyExchangeType { get; set; }
        public virtual PriceTerm PriceTerm { get; set; }
        public virtual IList<SalesQuoteDetail> SalesQuoteDetails { get; set; }
        public virtual IList<SalesQuoteLog> SalesQuoteLogs { get; set; }
    }
}
