using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class GA_PackValidationProcess
    {
        public static List<BoxGroup> GeneratePackValidation(List<PackingLine> packingLines, 
            List<ItemModel> itemModels, List<ItemStyle> itemStyles, bool checkAssortedSize, 
            string packingListCode ,ref string errorMessage)
        {
            List<BoxGroup> boxGroups = new List<BoxGroup>();
            var boxGroup = new BoxGroup();  
            var PONumber = itemStyles.FirstOrDefault()?.PurchaseOrderNumber ?? string.Empty;
            var firstRow = true;
            var barcodes = "";

            foreach(var style in itemStyles)
            {
                foreach (var packingLine in packingLines
                    .Where(x => x.LSStyle == style.LSStyle).OrderBy(x => x.SequenceNo).ToList())
                {
                    // Create Box Group
                    if (firstRow)
                    {
                        boxGroup.PONum = PONumber;
                        boxGroup.Date = DateTime.Now;
                        boxGroup.FileName = String.Format("Pulled from ERP ({0})", packingListCode);
                        firstRow = false;

                        /// Insert barcode
                        barcodes = packingLine.BarCode.Trim();
                        if (!string.IsNullOrEmpty(barcodes))
                        {
                            if (!InsertBarcodes(barcodes, ref boxGroup, ref errorMessage))
                            {
                                return null;
                            }
                        }
                    }
                    else if (!checkAssortedSize)
                    {
                        boxGroup = new BoxGroup();
                        boxGroup.PONum = PONumber;
                        boxGroup.Date = DateTime.Now;
                        boxGroup.FileName = String.Format("Pulled from ERP ({0})", packingListCode);

                        /// Insert barcode
                        barcodes = packingLine.BarCode.Trim();
                        if (!string.IsNullOrEmpty(barcodes))
                        {
                            if (!InsertBarcodes(barcodes, ref boxGroup, ref errorMessage))
                            {
                                return null;
                            }
                        }
                    }

                    // Insert Box Detail
                    var itemModel = itemModels.Where(x => x.Style.Trim().ToUpper().Equals(style.CustomerStyle.Trim().ToUpper()) &&
                                                     x.GarmentColorCode.Trim().ToUpper().Equals(style.ColorCode.Trim().ToUpper()) &&
                                                     x.GarmentSize.Trim().ToUpper().Equals(packingLine.Size.Trim().ToUpper())).FirstOrDefault();
                    if (itemModel != null)
                    {
                        if (!InsertBoxDetail(packingLine, itemModel, style, ref boxGroup, ref errorMessage))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        errorMessage = String.Format("Style: {0} - Color: {1} - Size: {2} not found UPC, please re-check!!!",
                                                    style.CustomerStyle, style.ColorCode, packingLine.Size);
                        return null;
                    }

                    if(boxGroups.Find(x => x.Oid == boxGroup.Oid) == null)
                    {
                        boxGroups.Add(boxGroup);
                    }
                }
            }

            return boxGroups;
        }

        private static bool InsertBarcodes(string barcodes, ref BoxGroup boxGroup, ref string errorMessage)
        {
            if (!String.IsNullOrEmpty(barcodes))
            {
                barcodes = barcodes.Trim();

                //barcodes = barcodes.Replace(">", "").Replace("》", "").Replace(",", "").Replace(";", "\n");
                string fakebarcodes = barcodes.Replace(">", "").Replace("》", "").Replace(",", "\n")
                                              .Replace(";", "\n").Replace("\n\n", "\n").Replace("  ", " ")
                                              .Replace("   ", " ").Replace(" ", "\n").Replace("\"","");

                string[] arrBarcodes = fakebarcodes.Split(
                                                        new[] { "\r\n", "\r", "\n", " " },
                                                        StringSplitOptions.None
                                                    );
                foreach (var item in arrBarcodes)
                {
                    boxGroup.BarcodeRange += item + "\n";
                }

                for (int ii = 0; ii < arrBarcodes.Count(); ii++)
                {
                    string barcode = arrBarcodes[ii].Trim();
                    if (!String.IsNullOrEmpty(barcode))
                    {
                        long startBC = 0;
                        long endBC = 0;
                        if (barcode.IndexOf("-") > 0)
                        {
                            string[] rangeBarcode = barcode.Split('-');
                            startBC = long.Parse(rangeBarcode[0]);
                            endBC = long.Parse(rangeBarcode[1]);
                        }
                        else
                        {
                            startBC = long.Parse(barcode);
                            endBC = long.Parse(barcode);
                        }

                        if (startBC > endBC)
                        {
                            long temp = 0;
                            temp = startBC;
                            startBC = endBC;
                            endBC = temp;

                            if (startBC.ToString().Length < endBC.ToString().Length)
                            {
                                errorMessage = String.Format("Barcode {0} incorrect, please re-check!!!", barcodes);
                                return false;
                            }
                        }

                        // generate barcode
                        bool isLoop = true;
                        do
                        {
                            var boxModel = new BoxModel();
                            boxModel.Barcode = startBC.ToString();
                            boxGroup.BoxModels.Add(boxModel);

                            if (startBC == endBC)
                            {
                                isLoop = false;
                            }

                            startBC++;

                        } while (isLoop);
                    }
                }

            }
            return true;
        }

        private static bool InsertBoxDetail(PackingLine packingLine,ItemModel itemModel, 
            ItemStyle style, ref BoxGroup boxGroup, ref string errorMessage)
        {
            var boxDetail =new BoxDetail();
            boxDetail.Style = itemModel.Style;
            boxDetail.Color = itemModel.GarmentColorCode;
            boxDetail.ColorName = itemModel.GarmentColorName;
            boxDetail.Description = style.Description;
            boxDetail.Size = itemModel.GarmentSize;
            boxDetail.GTIN = itemModel.UPC;
            boxDetail.IsMerge = true;
            boxDetail.Qty = (int)packingLine.Quantity;
            boxGroup.BoxDetails.Add(boxDetail);
                
            return true;
        }
    }
}
