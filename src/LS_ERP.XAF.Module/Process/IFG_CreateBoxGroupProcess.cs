using DevExpress.ExpressApp;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class IFG_CreateBoxGroupProcess
    {
        public static List<BoxGroup> CreateScanBarcode(ref PackingList packingList, 
            IObjectSpace objectSpaceBoxGroup, List<ItemModel> itemModels, List<ItemStyleBarCode> barcodes,
            string sheetName, ref string errorMessage)
        {
            var newBoxGroups = new List<BoxGroup>();

            /// Check barcode & UPC
            if(!itemModels.Any())
            {
                errorMessage = "Please import UPC before!!!";
                return null;
            }

            if (!barcodes.Where(b => !string.IsNullOrEmpty(b.BarCode)).Any())
            {
                errorMessage = "Please import barcode before!!!";
                return null;
            }

            /// Update barcode for packing lines
            foreach (var style in packingList?.ItemStyles)
            {
                var styleBarCodes = barcodes.Where(b => b.ItemStyleNumber == style.Number).ToList();
                foreach(var styleBarCode in styleBarCodes)
                {   
                    var lines = packingList?.PackingLines
                                .Where(l => l.LSStyle == style.LSStyle
                                       && l.Size == styleBarCode.Size).ToList();

                    lines.ForEach(x =>
                    {
                        x.BarCode = styleBarCode.BarCode;
                    });
                }
            }

            /// Create box group / box detail / box model
            foreach(var barCode in barcodes
                .Select(b => b.BarCode).Distinct())
            {
                var packingLines = packingList?.PackingLines
                        ?.Where(l => l.BarCode == barCode).ToList();

                var customerColorCode = barcodes.FirstOrDefault(b => b.BarCode == barCode).Color;

                var boxGroup = objectSpaceBoxGroup.CreateObject<BoxGroup>();
                boxGroup.PONum = packingList?.ItemStyles?.FirstOrDefault()?.PurchaseOrderNumber;
                boxGroup.BarcodeRange = barCode;
                boxGroup.Date = DateTime.Now;
                boxGroup.FileName = String.Format("Pulled from ERP ({0})", packingList.LSStyles);
                boxGroup.PackingListCode = packingList.PackingListCode;
                boxGroup.CustomerID = packingList.CustomerID;
                boxGroup.SheetName = sheetName;
                boxGroup.SetCreateAudit(SecuritySystem.CurrentUserName);

                var boxModel = new BoxModel();
                boxModel.Barcode = barCode;
                boxModel.SetCreateAudit(SecuritySystem.CurrentUserName);
                boxGroup.BoxModels.Add(boxModel);

                foreach(var line in packingLines)
                {
                    var style = packingList?.ItemStyles?.Where(i => i.LSStyle == line.LSStyle).FirstOrDefault();
                    

                    var itemModel = itemModels.Where(x => x.Style.Trim().ToUpper().Equals(style.CustomerStyle.Trim().ToUpper()) &&
                                                 x.Barcode.Trim().ToUpper().Equals(barCode.Trim().ToUpper()) &&
                                                 x.GarmentSize.Trim().ToUpper().Equals(line.Size.Trim().ToUpper())).FirstOrDefault();
                    if (itemModel != null)
                    {
                        var boxDetail = new BoxDetail();
                        boxDetail.Style = itemModel.Style;
                        boxDetail.Color = itemModel.CustomerColorCode;
                        boxDetail.ColorName = itemModel.GarmentColorName;
                        boxDetail.Description = style.Description;
                        boxDetail.Size = itemModel.GarmentSize;
                        boxDetail.GTIN = itemModel.UPC;
                        boxDetail.IsMerge = true;
                        boxDetail.Qty = (int)line.Quantity;
                        boxDetail.SetCreateAudit(SecuritySystem.CurrentUserName);
                        boxGroup.BoxDetails.Add(boxDetail);
                    }
                    else
                    {
                        errorMessage = String.Format("Style: {0} - Color: {1} - Size: {2} not found UPC, please re-check!!!",
                                                    style.CustomerStyle, customerColorCode, line.Size);
                        break;
                    }
                }

                newBoxGroups.Add(boxGroup);
            }

            if (!string.IsNullOrEmpty(errorMessage))
                return null;
            else
                return newBoxGroups;
        }
    }
}
