using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class CuttingOutputStatusDtos
    {
        public CuttingOutputStatusDtos()
        {
            this.Status = "NO";
        }
        public DateTime PSDD { get; set; }
        public string PSDDString { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string MergeLSStyle { get; set; }
        public string LSStyle { get; set; }
        public string CustomerName { get; set; }
        public string Set { get; set; }
        public string Size { get; set; }
        public decimal TargetQuantity { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal TotalAllocQuantity { get; set; }
        public decimal TotalBalanceQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal FabricBalanceQuantity { get; set; }
        public decimal FabricQuantity { get; set; }
        public bool IsFull { get; set; }
        public string GarmentColor { get; set; }
        public string ContrastColor { get; set; }
        public string FabricContrastName { get; set; }
        public decimal AllocQuantity { get; set; }
        public int CuttingOutputID { get; set; }
        public string WorkcenterName { get; set; }
        public DateTime ProduceDate { get; set; }
        public string Lot { get; set; }
        public decimal LotAllocQuantity { get; set; }
        public string Status { get; set; }
        public bool  IsCanceled { get; set; }
        public string  Remark { get; set; }
        public decimal PercentPrint { get; set; }
        public decimal PercentQuantity { get; set; }
        public decimal Sample { get; set; }

    }
}
