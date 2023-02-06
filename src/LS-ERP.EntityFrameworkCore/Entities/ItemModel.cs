using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemModel : Audit
    {
        public long ID { get; set; }
        public string Style { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string UPC { get; set; }
        public string CustomerID { get; set; }
        public string FileName { get; set; }
        public string SaveFilePath { get; set; }

        /// <summary>
        /// For GARAN
        /// </summary>
        public string ContractNo { get; set; }
        public string Mfg { get; set; }
        public string SuppPlt { get; set; }
        public string Season { get; set; }
        public string ReplCode { get; set; }
        public string DeptSubFineline { get; set; }
        public string FixtureCode { get; set; }
        public string TagSticker { get; set; }

        /// <summary>
        /// For HADDAD
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// For IFG
        /// </summary>
        public decimal? MSRP { get; set; }
        public int MasterBox { get; set; }
        public string LSStyle { get; set; }
        public string Barcode { get; set; }
        public string Label { get; set; }
        public string LabelCode { get; set; }
        public string StyleDescription { get; set; }
        public string CustomerColorCode { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
