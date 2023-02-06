using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Dtos;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class ExportPurchaseOrderProcessor
    {
        private static Dictionary<int, string> _dicAlphabel; // 1 = A, 2 = B
        private static Dictionary<string, int> _dicPositionColumnPackingLine; // Units / carton = 10, No. of Carton = 11
        private static int _maxColumn = 0;
        private static int _maxRow = 1;

        public static Stream CreateExcelFile(List<PurchaseReceivedDetails> purchaseOrders, Stream stream = null)
        {
            //_maxRow = 1;
            //_rowMBL = 0;
            //DataTable table = SetData(invoice);

            string Author = "Leading Star";
            string Title = purchaseOrders.FirstOrDefault().CustomerPurchaseOrderNumber;
            ResetData();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Title = "Purchase Receive";
                excelPackage.Workbook.Properties.Comments = "Purchase Receive of Leading Start";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Verdana", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeaderPU(workSheet);

                //DataTable table = SetDataPU(purchaseOrders);

                switch (purchaseOrders.FirstOrDefault()?.CustomerID)
                {
                    case "PU":
                        {
                            //workSheet.Cells[4, 1].LoadFromDataTable(table);
                            //FormatStyleFor_PU(workSheet, table);
                        }
                        break;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static Stream CreateDocumentIssued_ExcelFile(List<PurchaseReceivedDetails> purchaseOrders, Stream stream = null)
        {
            //_maxRow = 1;
            //_rowMBL = 0;
            //DataTable table = SetData(invoice);

            string Author = "Leading Star";

            string Title = "Issue document";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Title = "Delivery Document(Inventory)";
                excelPackage.Workbook.Properties.Comments = "Issue document of Leading Start";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Verdana", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeaderPU_IssueDocument(workSheet);


                DataTable table = SetDataPU_IssueDocument(purchaseOrders);

                switch (purchaseOrders.FirstOrDefault()?.CustomerID)
                {
                    case "PU":
                        {
                            workSheet.Cells[4, 1].LoadFromDataTable(table);
                            FormatStyleFor_PU_IssueDocument(workSheet, table);
                        }
                        break;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public static DataTable SetDataPU(List<PurchaseReceivedDetails> purchaseOrders)
        {
            DataTable table = new DataTable();
            List<ExportPurchaseOrderLinePU_Dto> fillData = new List<ExportPurchaseOrderLinePU_Dto>();

            var dicData = new Dictionary<string, ExportPurchaseOrderLinePU_Dto>();
            //var dicItemNo = new Dictionary<string, int>();
            var dicTotalOrderQuantity = new Dictionary<string, decimal>();

            //foreach (var purchaseOrder in purchaseOrders)
            //{
            var sortPOLine = purchaseOrders.OrderBy(x => x.ItemID);

            foreach (var purchaseLine in sortPOLine)
            {
                string key = purchaseLine.DeliveryNo + purchaseLine.ItemID + purchaseLine.CustomerPurchaseOrderNumber + purchaseLine.ItemColorName +
                             purchaseLine.MaterialTypeClass + purchaseLine.ContractNo + purchaseLine.TrxDate;

                string keyTotalQuantity = purchaseLine.DeliveryNo + purchaseLine.ItemID + purchaseLine.CustomerPurchaseOrderNumber + purchaseLine.ItemColorName +
                                          purchaseLine.MaterialTypeClass;
                //string keyItemNo = purchaseLine.ItemID + purchaseLine.CustomerPurchaseOrderNumber;
                //if (!dicItemNo.ContainsKey(keyItemNo))
                //{
                //    //Console.WriteLine(">>>>>" + keyItemNo);
                //    var number = dicItemNo.Keys.Count + 1;
                //    dicItemNo[keyItemNo] = number;
                //}

                if (dicData.TryGetValue(key, out ExportPurchaseOrderLinePU_Dto rsFillData))
                {
                    rsFillData.AllocQty += purchaseLine.Quantity;
                    //rsFillData.TotalReceiveQuantity += purchaseLine.Quantity;
                    dicData[key] = rsFillData;
                }
                else
                {
                    ExportPurchaseOrderLinePU_Dto newData = new ExportPurchaseOrderLinePU_Dto();
                    newData.PONo = purchaseLine.CustomerPurchaseOrderNumber;
                    newData.Warehouse = "VNE";
                    newData.Manufacturer = "VNE";
                    newData.MatrCode = purchaseLine.ItemID;
                    newData.GarmentColor = purchaseLine.ItemColorName;
                    newData.ContractNo = purchaseLine.ContractNo;
                    newData.CustomerStyle = purchaseLine.CustomerStyle;
                    newData.AllocQty = purchaseLine.Quantity;
                    newData.ItemNo = int.Parse(purchaseLine.ItemNo);
                    newData.MatrClass = purchaseLine.MaterialTypeClass;
                    newData.DeliveryNote = purchaseLine.DeliveryNo;
                    newData.OrderQuantity = purchaseLine.Quantity;

                    newData.ShipDate = purchaseLine.TrxDate;
                    newData.EstimateShipDate = purchaseLine.ArrivedDate;
                    //newData.VendorConfirmDate = purchaseLine.VendorConfirmDate;

                    //if (int.TryParse(purchaseLine.ItemNo, out int itemNo))
                    //{
                    //    newData.ItemNo = itemNo;
                    //}

                    dicData[key] = newData;
                }

                //if (dicTotalOrderQuantity.TryGetValue(keyOrderQuantity, out decimal totalQty))
                //{
                //    totalQty += purchaseLine.Quantity ?? 0;
                //    dicTotalOrderQuantity[keyOrderQuantity] = totalQty;
                //}
                //else
                //{
                //    dicTotalOrderQuantity[keyOrderQuantity] = purchaseLine.Quantity ?? 0;
                //}
            }
            //}

            foreach (var itemData in dicData)
            {
                //string keyOrderQuantity = itemData.Value.PONo + itemData.Value.GarmentColor;

                //if (dicTotalOrderQuantity.TryGetValue(keyOrderQuantity, out decimal totalQty))
                //{
                //    itemData.Value.OrderQuantity = totalQty;
                //    itemData.Value.TotalReceiveQuantity = totalQty;
                //}

                //string keyItemNo = itemData.Value.MatrCode + itemData.Value.PONo;
                //if (dicItemNo.TryGetValue(keyItemNo, out int itemNo))
                //{
                //    itemData.Value.ItemNo = itemNo;
                //}

                fillData.Add(itemData.Value);
            }

            //foreach (var data in fillData)
            //{

            //    DataRow row = table.NewRow();
            //    row["Column0"] = data.PONo;
            //    row["Column1"] = "";
            //    row["Column2"] = "";
            //    row["Column3"] = data.VendorConfirmDate;
            //    row["Column4"] = data.EstimateShipDate ?? DateTime.Parse("01-01-1990");
            //    row["Column5"] = data.DeliveryNote;
            //    row["Column6"] = "";
            //    row["Column7"] = data.Warehouse;
            //    row["Column8"] = "";
            //    row["Column9"] = "";
            //    row["Column10"] = "";
            //    row["Column11"] = data.Manufacturer;
            //    row["Column12"] = data.MatrClass;
            //    row["Column13"] = data.MatrCode;
            //    row["Column14"] = data.ItemNo;
            //    row["Column15"] = "";
            //    row["Column16"] = data.ShipDate;
            //    row["Column17"] = "";
            //    row["Column18"] = "";
            //    row["Column19"] = data.GarmentColor;
            //    row["Column20"] = "";
            //    row["Column21"] = data.OrderQuantity;
            //    row["Column22"] = "";
            //    row["Column23"] = "";
            //    row["Column24"] = data.GarmentColor;
            //    row["Column25"] = "";
            //    row["Column26"] = data.ContractNo;
            //    row["Column27"] = data.CustomerStyle;
            //    row["Column28"] = data.AllocQty;


            //    table.Rows.Add(row);

            //}

            return table;
        }

        public static void CreateHeaderPU(ExcelWorksheet workSheet)
        {
            //int column = 1;
            
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            workSheet.Cells[1, 1].Value = "Purchase Receive";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1, 1, 2].Merge = true;

            workSheet.Cells[2, 1].Value = "<PuOrderNo>";
            workSheet.Cells[2, 2].Value = "<Supp>";
            workSheet.Cells[2, 3].Value = "<SuppName>";
            workSheet.Cells[2, 4].Value = "<TrxDate(Date)>";
            workSheet.Cells[2, 5].Value = "<ATADate(Date)>";
            workSheet.Cells[2, 6].Value = "<DocRef>";
            workSheet.Cells[2, 7].Value = "<LotNo>";
            workSheet.Cells[2, 8].Value = "<Warehouse>";
            workSheet.Cells[2, 9].Value = "<BinNumber>";
            workSheet.Cells[2, 10].Value = "<ExtDoc>";
            workSheet.Cells[2, 11].Value = "<Mftr>";
            workSheet.Cells[2, 12].Value = "<SysOwner>";
            workSheet.Cells[2, 13].Value = "<MatrClass>";
            workSheet.Cells[2, 14].Value = "<MatrCode>";
            workSheet.Cells[2, 15].Value = "<ItemNo>";
            workSheet.Cells[2, 16].Value = "<MatrCode/ShortName>";
            workSheet.Cells[2, 17].Value = "<ExtTerm1>";
            workSheet.Cells[2, 18].Value = "<Remark>";
            workSheet.Cells[2, 19].Value = "<LotRef>";
            workSheet.Cells[2, 20].Value = "<Asm/Color>";
            workSheet.Cells[2, 21].Value = "<Asm/Sizx>";
            workSheet.Cells[2, 22].Value = "<Asm/OrdQty>";
            workSheet.Cells[2, 23].Value = "<Asm/RcvQty>";
            workSheet.Cells[2, 24].Value = "<RcvUnit>";
            workSheet.Cells[2, 25].Value = "<Alloc/Color>";
            workSheet.Cells[2, 26].Value = "<Alloc/Sizx>";
            workSheet.Cells[2, 27].Value = "<Alloc/OrderNo>";
            workSheet.Cells[2, 28].Value = "<Alloc/Style>";
            workSheet.Cells[2, 29].Value = "<Alloc/RcvQty>";

            workSheet.Cells[2, 1, 2, 29].Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));

            workSheet.Cells[3, 1].Value = "P/O No";
            workSheet.Cells[3, 2].Value = "Supplier";
            workSheet.Cells[3, 3].Value = "Supplier Name";
            workSheet.Cells[3, 4].Value = "Trx. Date";
            workSheet.Cells[3, 5].Value = "Actual Arrive Date (ATA)";
            workSheet.Cells[3, 6].Value = "Delivery Note";
            workSheet.Cells[3, 7].Value = "Lot No.";
            workSheet.Cells[3, 8].Value = "Warehouse";
            workSheet.Cells[3, 9].Value = "Bin Number";
            workSheet.Cells[3, 10].Value = "WH Entry No.";
            workSheet.Cells[3, 11].Value = "Manufacturer";
            workSheet.Cells[3, 12].Value = "Owner";
            workSheet.Cells[3, 13].Value = "Matr Class";
            workSheet.Cells[3, 14].Value = "Matr Code";
            workSheet.Cells[3, 15].Value = "Item No";
            workSheet.Cells[3, 16].Value = "Description";
            workSheet.Cells[3, 17].Value = "Extended P/O Receive Term1";
            workSheet.Cells[3, 18].Value = "Remark";
            workSheet.Cells[3, 19].Value = "Dye Lot";
            workSheet.Cells[3, 20].Value = "Color";
            workSheet.Cells[3, 21].Value = "Size";
            workSheet.Cells[3, 22].Value = "Order Qty";
            workSheet.Cells[3, 23].Value = "Qty";
            workSheet.Cells[3, 24].Value = "Unit";
            workSheet.Cells[3, 25].Value = "Color";
            workSheet.Cells[3, 26].Value = "Size";
            workSheet.Cells[3, 27].Value = "OrderNo";
            workSheet.Cells[3, 28].Value = "Style";
            workSheet.Cells[3, 29].Value = "Alloc Qty";

            _maxRow = 3;
        }

        public static void FormatStyleFor_PU(ExcelWorksheet workSheet, DataTable table)
        {
            string modelRangeBorder = "A3:AM" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelRangeBorder])
            {
                //range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.AutoFitColumns();
            }

            //using (var range = workSheet.Cells["A2:T2"])
            //{
            //    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            //    range.Style.Font.Bold = true;
            //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
            //    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //}

            //string modelNumeric = "A3:T" + (table.Rows.Count + 2).ToString();
            //using (var range = workSheet.Cells[modelNumeric])
            //{
            //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //}

            string modelDate = "D3:D" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "mm/dd/yyyy";
            }

            modelDate = "Q3:Q" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "mm/dd/yyyy";
            }

            modelDate = "E3:E" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelDate])
            {
                range.Style.Numberformat.Format = "mm/dd/yyyy";
            }

        }


        public static DataTable SetDataPU_IssueDocument(List<PurchaseReceivedDetails> purchaseOrders)
        {
            DataTable table = new DataTable();
            for (int i = 0; i <= 25; i++)
            {
                DataColumn column = new DataColumn("Column" + i);
                table.Columns.Add(column);

                if (i == 20 || i == 25)
                {
                    column.DataType = typeof(decimal);
                }
            }

            List<ExportPurchaseOrderLinePU_Dto> fillData = new List<ExportPurchaseOrderLinePU_Dto>();

            var dicData = new Dictionary<string, ExportPurchaseOrderLinePU_Dto>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                //foreach (var purchaseLine in purchaseOrder.PurchaseOrderLines)
                //{
                //    string key = purchaseLine.GarmentColorName + purchaseLine.GarmentColorCode + purchaseLine.ContractNo + purchaseLine.CustomerStyle + purchaseLine.CustomerPurchaseOrderNumber;

                //    if (dicData.TryGetValue(key, out ExportPurchaseOrderLinePU_Dto rsFillData))
                //    {
                //        rsFillData.AllocQty += purchaseLine.Quantity;
                //        dicData[key] = rsFillData;
                //    }
                //    else
                //    {
                //        ExportPurchaseOrderLinePU_Dto newData = new ExportPurchaseOrderLinePU_Dto();
                //        newData.PONo = purchaseLine.CustomerPurchaseOrderNumber;
                //        newData.Warehouse = "VNE";
                //        newData.Manufacturer = "VNE";
                //        newData.MatrCode = purchaseLine.ItemID;
                //        newData.GarmentColor = purchaseLine.GarmentColorName + purchaseLine.GarmentColorCode;
                //        newData.ContractNo = purchaseLine.ContractNo;
                //        newData.CustomerStyle = purchaseLine.CustomerStyle;
                //        newData.AllocQty = purchaseLine.Quantity;

                //        dicData[key] = newData;
                //    }
                //}
            }

            foreach (var itemData in dicData)
            {
                fillData.Add(itemData.Value);
            }

            foreach (var data in fillData)
            {

                DataRow row = table.NewRow();
                row["Column0"] = data.PONo;
                row["Column1"] = "";
                row["Column2"] = "";
                row["Column3"] = "";
                row["Column4"] = "";
                row["Column5"] = "";
                row["Column6"] = "";
                row["Column7"] = data.Warehouse;
                row["Column8"] = "";
                row["Column9"] = "";
                row["Column10"] = "";
                row["Column11"] = data.Manufacturer;
                row["Column12"] = "";
                row["Column13"] = data.MatrCode;
                row["Column14"] = "";
                row["Column15"] = "";
                row["Column16"] = "";
                row["Column17"] = "";
                row["Column18"] = "";
                row["Column19"] = "";
                row["Column20"] = "";
                row["Column21"] = "";
                row["Column22"] = "";
                row["Column23"] = "";
                row["Column24"] = "";
                row["Column25"] = "";
                row["Column26"] = "";
                row["Column27"] = "";
                row["Column28"] = "";
                row["Column29"] = data.GarmentColor;
                row["Column30"] = "";
                row["Column31"] = "";
                row["Column32"] = "";
                row["Column33"] = "";
                row["Column34"] = data.GarmentColor;
                row["Column35"] = "";
                row["Column36"] = data.ContractNo;
                row["Column37"] = data.CustomerStyle;
                row["Column38"] = float.Parse(data.AllocQty?.ToString("G29"));

                table.Rows.Add(row);

            }

            return table;
        }

        public static void CreateHeaderPU_IssueDocument(ExcelWorksheet workSheet)
        {
            workSheet.Cells[1, 1].Value = "Delivery Document";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1, 1, 2].Merge = true;

            workSheet.Cells[2, 1].Value = "<IssDocRef>";
            workSheet.Cells[2, 2].Value = "<DocNoPrefix>";
            workSheet.Cells[2, 3].Value = "<DeliveryDate(Date)>";
            workSheet.Cells[2, 4].Value = "<ETADate(Date)>";
            workSheet.Cells[2, 5].Value = "<SendTo(Manufacturer.Code)>";
            workSheet.Cells[2, 6].Value = "<SendTo(Locate.Code)>";
            workSheet.Cells[2, 7].Value = "<ShipTo>";
            workSheet.Cells[2, 8].Value = "<Type>";
            workSheet.Cells[2, 9].Value = "<Status>";
            workSheet.Cells[2, 10].Value = "<SysOwner>";
            workSheet.Cells[2, 11].Value = "<Material/MatrClass>";
            workSheet.Cells[2, 12].Value = "<Material/MatrCode(MaterialCode.MatrCode:MatrClass)>";
            workSheet.Cells[2, 13].Value = "<Material/LotNo>";
            workSheet.Cells[2, 14].Value = "<Material/Warehouse>";
            workSheet.Cells[2, 15].Value = "<Material/LotRef>";
            workSheet.Cells[2, 16].Value = "<Material/TrfDocRef>";
            workSheet.Cells[2, 17].Value = "<Material/IssUnit>";
            workSheet.Cells[2, 18].Value = "<Material/Remark>";
            workSheet.Cells[2, 19].Value = "<Material/Assortment/Color>";
            workSheet.Cells[2, 20].Value = "<Material/Assortment/Sizx>";
            workSheet.Cells[2, 21].Value = "<Material/Assortment/IssQty>";
            workSheet.Cells[2, 22].Value = "<Material/JobAllocation/OrderNo>";
            workSheet.Cells[2, 23].Value = "<Material/JobAllocation/Style>";
            workSheet.Cells[2, 24].Value = "<Material/JobAllocation/Color>";
            workSheet.Cells[2, 25].Value = "<Material/JobAllocation/Sizx>";
            workSheet.Cells[2, 26].Value = "<Material/JobAllocation/IssQty>";

            workSheet.Cells[2, 1, 2, 26].Style.Font.Color.SetColor(Color.FromArgb(128, 128, 128));

            workSheet.Cells[3, 1].Value = "Document No";
            workSheet.Cells[3, 2].Value = "Doc No Prefix";
            workSheet.Cells[3, 3].Value = "Delivery Date";
            workSheet.Cells[3, 4].Value = "ETA Date";
            workSheet.Cells[3, 5].Value = "Send To(Issue)";
            workSheet.Cells[3, 6].Value = "Send To(Transfer)";
            workSheet.Cells[3, 7].Value = "Address";
            workSheet.Cells[3, 8].Value = "Type";
            workSheet.Cells[3, 9].Value = "Status";
            workSheet.Cells[3, 10].Value = "Owner";
            workSheet.Cells[3, 11].Value = "Material Class";
            workSheet.Cells[3, 12].Value = "Material Code";
            workSheet.Cells[3, 13].Value = "Lot No";
            workSheet.Cells[3, 14].Value = "Warehouse";
            workSheet.Cells[3, 15].Value = "Lot Ref";
            workSheet.Cells[3, 16].Value = "Transfer Doc Ref";
            workSheet.Cells[3, 17].Value = "Issue Unit";
            workSheet.Cells[3, 18].Value = "Remark";
            workSheet.Cells[3, 19].Value = "Color";
            workSheet.Cells[3, 20].Value = "Size";
            workSheet.Cells[3, 21].Value = "Issue Qty";
            workSheet.Cells[3, 22].Value = "Order No";
            workSheet.Cells[3, 23].Value = "Style";
            workSheet.Cells[3, 24].Value = "Alc Color";
            workSheet.Cells[3, 25].Value = "Alc Size";
            workSheet.Cells[3, 26].Value = "Alc Issue Qty";


        }

        public static void FormatStyleFor_PU_IssueDocument(ExcelWorksheet workSheet, DataTable table)
        {
            string modelRangeBorder = "A3:Z" + (table.Rows.Count + 3).ToString();
            using (var range = workSheet.Cells[modelRangeBorder])
            {
                //range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.AutoFitColumns();
            }

            //using (var range = workSheet.Cells["A2:T2"])
            //{
            //    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
            //    range.Style.Font.Bold = true;
            //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
            //    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //}

            //string modelNumeric = "A3:T" + (table.Rows.Count + 2).ToString();
            //using (var range = workSheet.Cells[modelNumeric])
            //{
            //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            //}

            //string modelDate = "B3:B" + (table.Rows.Count + 2).ToString();
            //using (var range = workSheet.Cells[modelDate])
            //{
            //    range.Style.Numberformat.Format = "dd-MMM-yyyy";
            //}

            //modelDate = "E3:E" + (table.Rows.Count + 2).ToString();
            //using (var range = workSheet.Cells[modelDate])
            //{
            //    range.Style.Numberformat.Format = "dd-MMM-yyyy";
            //}

            //modelDate = "S3:T" + (table.Rows.Count + 2).ToString();
            //using (var range = workSheet.Cells[modelDate])
            //{
            //    range.Style.Numberformat.Format = "dd-MMM-yyyy";
            //}

        }


        private static void ResetData()
        {
            _maxRow = 1;
            _maxColumn = 0;
        }
    }
}
