using Common.Model;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class CreateCuttingCardCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<CuttingCard>>
    {
        public string ID { get; set; }
        public int CuttingOutputID { get; set; }
        public int FabricContrastID { get; set; }
        public string FabricContrastName { get; set; }
        public string FabricContrastColor { get; set; }
        public string FabricContrastDescription { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string MergeLSStyle { get; set; }
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public string Set { get; set; }
        public string WorkCenterName { get; set; }
        public int TableNO { get; set; }
        public decimal Quantity { get; set; }
        public decimal AllocQuantity { get; set; }
        public decimal OnHandQuantity { get; set; }
        public int TotalPackage { get; set; }
        public DateTime ProduceDate { get; set; }
        public string Operation { get; set; }
        public string CurrentOperation { get; set; }
        public string QRCodeBase { get; set; }
        public string Location { get; set; }
        public int StorageBinEntryID { get; set; }
        public bool IsCompling { get; set; }
        public string CardType { get; set; }
        public string CardTypeID { get; set; }
        public string ParentID { get; set; }
        public string Lot { get; set; }
        public bool IsPrint { get; set; }
        public bool IsAllSize { get; set; }
        public string Season { get; set; }
        public string CustomerID { get; set; }
        public string Remark { get; set; }

    }
}
