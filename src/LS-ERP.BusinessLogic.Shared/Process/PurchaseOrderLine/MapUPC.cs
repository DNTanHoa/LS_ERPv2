using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class MapUPC
    {
        public static bool Map(List<PurchaseOrderLine> purchaseOrderLines, List<ItemModel> itemModels, out string errorMessaage,
            out List<PurchaseOrderLine> mappedPurchaseOrderLines)
        {
            errorMessaage = string.Empty;
            mappedPurchaseOrderLines = new List<PurchaseOrderLine>();

            foreach(var purchaseOrderLine in purchaseOrderLines)
            {
                var itemModel = itemModels
                    .FirstOrDefault(x => x.Style == purchaseOrderLine.CustomerStyle &&
                                         x.GarmentColorCode == purchaseOrderLine.GarmentColorCode &&
                                         x.GarmentSize == purchaseOrderLine.GarmentSize);
                if(itemModel != null)
                {
                    purchaseOrderLine.UPC = itemModel.UPC;
                    purchaseOrderLine.ContractNo = itemModel.ContractNo;
                    purchaseOrderLine.Mfg = itemModel.Mfg;
                    purchaseOrderLine.SuppPlt = itemModel.SuppPlt;
                    purchaseOrderLine.Season = itemModel.Season;
                    purchaseOrderLine.ReplCode = itemModel.ReplCode;
                    purchaseOrderLine.DeptSubFineline = itemModel.DeptSubFineline;
                    purchaseOrderLine.FixtureCode = itemModel.FixtureCode;
                    purchaseOrderLine.TagSticker = itemModel.TagSticker;
                    purchaseOrderLine.ModelName = itemModel.ModelName;
                    purchaseOrderLine.MSRP = itemModel.MSRP;
                }

                mappedPurchaseOrderLines.Add(purchaseOrderLine);
            }

            return true;
        }
    }
}
