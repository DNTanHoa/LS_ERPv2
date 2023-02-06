using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class IssuedContractParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string IssuedBy { get; set; }
        public string ReceivedBy { get; set; }
        public Storage Storage { get; set; }
        public Customer Customer { get; set; }
        public string MaterialTypeCode { get; set; } = string.Empty;
        public string ContractSearch { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<IssuedContractDetail> Details
            = new List<IssuedContractDetail>();

        public List<IssuedContractStyle> Contracts
            = new List<IssuedContractStyle>();
    }

    [DomainComponent]
    public class IssuedContractStyle
    {
        public string ContractNo { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
    }

    [DomainComponent]
    public class IssuedContractDetail
    {
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }
        public string UnitID { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string CustomStyle { get; set; }
        public string LSStyle { get; set; }
        public string Season { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }

        public decimal RequiredQuantity { get; set; }
        public decimal Roll { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainQuantity { get; set; }
        public string Remark { get; set; }
    }
}
