using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Controllers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = LS_ERP.XAF.Module.Helpers.Message;
using LS_ERP.XAF.Module.Process;
using LS_ERP.XAF.Module.DomainComponent;
using DevExpress.ExpressApp.Editors;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ExportPackingListItemStyle_Win_ViewController : ExportPacking_ItemStyle_ViewController
    {
        private const string FROM_NO = "From No";
        private const string TO_NO = "To No";
        private const string CARTON_NUMBER = "Carton Number";
        private const string COLOR = "Color";
        private const string SIZE = "SIZE";
        private const string UNITS_CARTON = "Units / carton";
        private const string NO_OF_CARTON = "No. of Carton";
        private const string TOTAL_UNITS = "Total Units";
        private const string NET_NET_WEIGHT = "Net,Net Weight (kg)";
        private const string NET_WEIGHT = "Net Weight (kg)";
        private const string GROSS_WEIGHT = "Gross Weight (kg)";
        private const string CARTON_DIMENSION = "Carton Dimension";
        private const string LENGTH = "Length";
        private const string HEIGHT = "Height";
        private const string WIDTH = "Width";
        private const string MEAS_M3 = "Meas (M3)";
        private const string TOTAL_NET_WEIGHT = "Total N.Weight (kg)";
        private const string TOTAL_GROSS_WEIGHT = "Total G.Weight (kg)";

        private int _maxRow = 1;
        private int _maxColumn = 0;
        private int _countSize = 0;
        private decimal _totalNoOfCarton = 0;
        private decimal _totalUnits = 0;
        private decimal _total_TotalNetWeight = 0;
        private decimal _totalGrossWeight = 0;
        private decimal _total_TotalGrossWeight = 0;
        private decimal _totalMeasM3 = 0;

        private ItemStyle _itemStyle;
        private Dictionary<string, int> _dicPositionColumnPackingLine; // Units / carton = 10, No. of Carton = 11
        private Dictionary<int, string> _dicAlphabel; // 1 = A, 2 = B
        private Dictionary<string, int> _dicTotalCartons; // S = 100, X = 50
        private int _rowSum = 0;

        public override void ExportPackingListItemStyle_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportPackingListItemStyle_Execute(sender, e);
            ResetData();

            var export = View.CurrentObject as ItemStyle;
            _itemStyle = export;
            SaveFileDialog dialog = new SaveFileDialog();

            MessageOptions options = null;
            string errorMessage = string.Empty;

            /// Check exist packing list
            var failSheetID = ObjectSpace.GetObjects<PackingSheetName>()
                    .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;
            var criteria = CriteriaOperator.Parse("[LSStyles] IN ('" + _itemStyle.LSStyle + "')");
            var packingLists = ObjectSpace.GetObjects<PackingList>(criteria)
                                .Where(x => (x.SheetNameID ?? 0) != failSheetID).ToList();

            if (packingLists.Any())
            {
                options = Message.GetMessageOptions("Packing list was existed!" + errorMessage, "Error",
                                InformationType.Error, null, 5000);
            }
            else
            {
                dialog.FileName = export.LSStyle.Replace('/', '-'); //export.SalesContracts.Number.Replace('/', '-');

                dialog.Filter = "Excel Files|*.xlsx;";
                
                //var objectSpace = Application.CreateObjectSpace(typeof(ItemStyle));
                //purchaseOrder = objectSpace.GetObjectByKey<ItemStyle>(purchaseOrder.ID);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (_dicAlphabel == null)
                    {
                        SetDictionaryAlphabet(3);
                    }

                    if (_itemStyle == null)
                    {
                        options = Message.GetMessageOptions("Item style is null, please contact to admin or ERP team", "Error",
                           InformationType.Error, null, 5000);
                        return;
                    }

                    var styleNetWeights = ObjectSpace.GetObjects<StyleNetWeight>(
                        CriteriaOperator.Parse(" [CustomerID] = ? AND [CustomerStyle] = ?", _itemStyle.SalesOrder?.CustomerID, _itemStyle.CustomerStyle));

                    var allocDailyOutputs = ObjectSpace.GetObjects<AllocDailyOutput>(
                        CriteriaOperator.Parse(" [LSStyle] = ? AND [FabricContrastName] = ?", _itemStyle.LSStyle, "A"))?.ToList();

                    var filter = "";
                    if(allocDailyOutputs.Any())
                    {
                        filter = " AND [AllocDailyOutputID] " +
                            " IN (" + string.Join(",", allocDailyOutputs?.Select(x => x.ID)) + ")";
                    }

                    var setCutting = ObjectSpace.GetObjects<DailyTarget>(
                            CriteriaOperator.Parse(" [LSStyle] = ? AND [Operation] = ?", _itemStyle.LSStyle, "CUTTING"))?.FirstOrDefault();

                    var cuttingLots = new List<CuttingLot>();
                    if ((setCutting?.Set ?? string.Empty).Contains(";"))
                    {
                        cuttingLots = ObjectSpace.GetObjects<CuttingLot>(
                            CriteriaOperator.Parse(" [LSStyle] = ? AND [Set] = ? " + filter, _itemStyle.LSStyle, setCutting?.Set.Split(";")[0])).ToList();
                    }
                    else
                    {
                        cuttingLots = ObjectSpace.GetObjects<CuttingLot>(
                            CriteriaOperator.Parse(" [LSStyle] = ? " + filter, _itemStyle.LSStyle)).ToList();
                    }

                    var packingList = _itemStyle.Barcodes?.Select(x => x.Packing.Replace(" ", "").Replace(".0", ""));
                    var boxDimensions = ObjectSpace.GetObjectsQuery<BoxDimension>(true).Where(x => packingList.Contains(x.Code));

                    var dicBoxDimensions = boxDimensions.ToDictionary(x => x.Code);

                    var boxNetWeights = new List<StyleNetWeight>();
                    var netNetWeights = new List<StyleNetWeight>();
                    _itemStyle?.OrderDetails.ToList().ForEach(x =>
                    {
                        var netWeights = styleNetWeights.Where(w => w.Size == x.Size).ToList();
                        if(netWeights.Count <= 1)
                        {
                            boxNetWeights.AddRange(netWeights);
                            netNetWeights.AddRange(netWeights);
                        }
                        else
                        {
                            var boxSize = _itemStyle.Barcodes?
                               .Where(b => b?.Size?.Trim().Replace(" ", "").ToUpper()
                                               == netWeights?.FirstOrDefault()?.Size?.Trim().Replace(" ", "").ToUpper())
                               .Select(x => x.Packing.Replace(" ", "").Replace(".0", "")).FirstOrDefault();

                            var boxNetWeight = netWeights.Where(w =>
                                w?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                == boxSize.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", ""))?.FirstOrDefault();

                            if (boxNetWeight != null)
                            {
                                boxNetWeights.Add(boxNetWeight);
                            }
                            else
                            {
                                boxNetWeights.Add(netWeights.FirstOrDefault());
                            }

                            if (netWeights.Find(w => (w.GarmentColorCode ?? string.Empty) == _itemStyle.ColorCode) != null)
                            {
                                netNetWeights.Add(netWeights
                                    .Where(w => w.GarmentColorCode == _itemStyle.ColorCode).FirstOrDefault());
                            }
                            else
                            {
                                netNetWeights.Add(netWeights.FirstOrDefault());
                            }
                        }
                    });

                    _countSize = _itemStyle.OrderDetails.Count();

                    byte[] logoImage = SaveFileHelpers.Dowload(ConfigurationManager.AppSettings.Get("LogoPackingListDE").ToString());

                    try
                    {
                        switch (export.SalesOrder?.CustomerID)
                        {
                            case "DE":
                                {
                                    //var stream = CreateExcelFile(export, logoImage, dicBoxDimensions, styleNetWeights, out errorMessage);
                                    var stream = CreateExcelFile(export, logoImage, dicBoxDimensions, netNetWeights, boxNetWeights, cuttingLots, out errorMessage);
                                    var buffer = stream as MemoryStream;

                                    File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                                    options = Message.GetMessageOptions("Export successfully. ", "Successs",
                                   InformationType.Success, null, 5000);
                                }
                                break;
                        }

                        options = Message.GetMessageOptions("Export successfully. ", "Successs", InformationType.Success, null, 5000);
                    }
                    catch (Exception EE)
                    {
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            options = Message.GetMessageOptions("Export failed. " + errorMessage, "Error",
                                InformationType.Error, null, 5000);
                        }
                        else
                        {
                            options = Message.GetMessageOptions("Export failed. " + EE.Message, "Error",
                                InformationType.Error, null, 5000);
                        }

                    }
                    finally
                    {
                        if (options.Type == InformationType.Success)
                        {
                            SavePackingList(export, dicBoxDimensions, styleNetWeights);
                        }
                    }
                }
            }
            Application.ShowViewStrategy.ShowMessage(options);
            View.Refresh();
        }

        private Stream CreateExcelFile(ItemStyle packingList, byte[] logoImage,
            Dictionary<string, BoxDimension> boxDimensions,
            IList<StyleNetWeight> netNetWeights, IList<StyleNetWeight> boxNetWeights,
            IList<CuttingLot> cuttingLots, out string errorMessage,
            Stream stream = null)
        {
            //DataTable table = SetData(packingList);
            errorMessage = string.Empty;
            string Author = "Leading Star VN";

            if (!String.IsNullOrEmpty(packingList.CreatedBy))
            {
                Author = packingList.CreatedBy;
            }

            string Title = "Sheet";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Packing List of Leading Start VN";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Arial", 10));

                CreateHeaderPage(workSheet, logoImage);
                CreateHeaderPacking(workSheet);
                FillDataPackingLine(workSheet, boxDimensions, netNetWeights, boxNetWeights, out errorMessage);
                TotalCarton(workSheet, cuttingLots);
                //SummaryQuantitySize(workSheet, packingList, dontShortShipImage);


                //if (!_isFitToPage)
                //{
                //workSheet.PrinterSettings.FitToPage = true;
                //}
                decimal marginPage = (decimal)0.2;
                workSheet.PrinterSettings.FitToPage = true;
                workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                workSheet.PrinterSettings.TopMargin = marginPage;
                workSheet.PrinterSettings.LeftMargin = marginPage;
                workSheet.PrinterSettings.RightMargin = marginPage;
                workSheet.PrinterSettings.BottomMargin = marginPage;
                workSheet.PrinterSettings.VerticalCentered = true;
                workSheet.PrinterSettings.HorizontalCentered = true;

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }

        public void CreateHeaderPage(ExcelWorksheet workSheet, byte[] logoImage)
        {
            Image logo2 = LogoDownload(logoImage);
            var pictureLeft = workSheet.Drawings.AddPicture("pictureLeft", logo2);
            pictureLeft.SetPosition(0, 0, 0, 0);
            pictureLeft.SetSize(198, 45);

            workSheet.Row(_maxRow).Height = 22;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "PACKING LIST";

            workSheet.Cells[_maxRow, 1, _maxRow, 16].Merge = true;/////////////////////////////////
            workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Font.SetFromFont(new Font("Arial", 18));
            workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.Font.Color.SetColor(Color.Blue);
            workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, 1, _maxRow, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(_maxRow).Height = 22;

            _maxRow++;
            int posCST = 5 + _countSize;
            //SetCountry(ref posCustomer);

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "JOB NO:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].EntireColumn.Width = 5.5;
            workSheet.Cells[_maxRow, 2].EntireColumn.Width = 2;
            workSheet.Cells[_maxRow, 3].EntireColumn.Width = 5.5;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.LSStyle;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "STYLE NAME:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.CustomerStyle;
            workSheet.Cells[_maxRow, posCST].Value = "CST No: ";
            workSheet.Cells[_maxRow, posCST].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCST + 1].Value = _itemStyle.DeliveryPlace;
            //workSheet.Cells[_maxRow, posCST + 1, _maxRow, posCST + 7].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "ORDER NUMBER:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.PurchaseOrderNumber;


            _maxRow++;
            posCST += 4;
            workSheet.Cells[_maxRow, posCST].Value = "PSDD:";
            workSheet.Cells[_maxRow, posCST].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCST + 1].Value = _itemStyle?.ProductionSkedDeliveryDate;

            workSheet.Cells[_maxRow, posCST].Style.Numberformat.Format = "MM/dd/yyyy";
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL")
            {
                DateTimeFormat = { YearMonthPattern = "MM/dd/yyyy" }
            };

            workSheet.Cells[_maxRow, posCST + 1].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.YearMonthPattern;
            workSheet.Cells[_maxRow, posCST + 1, _maxRow, posCST + 3].Merge = true;
            workSheet.Cells[_maxRow, posCST + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            _maxRow += 2;
        }

        public void CreateHeaderPacking(ExcelWorksheet workSheet)
        {
            int column = 1;

            string range = "A" + _maxRow + ":C" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = CARTON_NUMBER;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(FROM_NO, 1); // add position column 
            _dicPositionColumnPackingLine.Add(TO_NO, 3);

            column += 3;

            range = "D" + _maxRow + ":D" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(COLOR, column);

            column++;

            workSheet.Cells[_maxRow, column].Value = SIZE;
            workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            _dicPositionColumnPackingLine.Add(SIZE, column);
            var sizeList = _itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            workSheet.Row(_maxRow).Height = 20;
            workSheet.Row(_maxRow + 1).Height = 24;

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    workSheet.Cells[_maxRow + 1, column].Style.Font.Bold = true;
                    _dicPositionColumnPackingLine.Add(size.Replace(" ", "").ToUpper(), column);

                    if (size.Length > 5)
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 11.5;
                    }
                    else
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 4;
                    }

                    workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;

                    column++;
                }
            }

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = "E" + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_dicAlphabel.TryGetValue(column, out string unitsCharacter))
            {
                range = unitsCharacter + _maxRow + ":" + unitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = UNITS_CARTON;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Cells[range].Style.WrapText = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(UNITS_CARTON, column);
            }

            column++;


            if (_dicAlphabel.TryGetValue(column, out string noOfCartons))
            {
                range = noOfCartons + _maxRow + ":" + noOfCartons + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NO_OF_CARTON;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(NO_OF_CARTON, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))
            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS, column);
            }

            //column++;

            //if (_dicAlphabel.TryGetValue(column, out string netNetWeightCharacter))
            //{
            //    range = netNetWeightCharacter + _maxRow + ":" + netNetWeightCharacter + (_maxRow + 1);
            //    workSheet.Cells[_maxRow, column].Value = NET_NET_WEIGHT;
            //    workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
            //    workSheet.Cells[_maxRow, column].Style.WrapText = true;
            //    workSheet.Cells[range].Merge = true;
            //    workSheet.Column(column).Width = 7;
            //    _dicPositionColumnPackingLine.Add(NET_NET_WEIGHT, column);
            //}

            column++;

            if (_dicAlphabel.TryGetValue(column, out string netWeightCharacter))
            {
                range = netWeightCharacter + _maxRow + ":" + netWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NET_WEIGHT;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(NET_WEIGHT, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string grossWeightCharacter))
            {
                range = grossWeightCharacter + _maxRow + ":" + grossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = GROSS_WEIGHT;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(GROSS_WEIGHT, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string cartonDimensionCharacter))
            {

                workSheet.Cells[_maxRow, column].Value = CARTON_DIMENSION;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;

                _dicPositionColumnPackingLine.Add(LENGTH, column);
                workSheet.Column(column++).Width = 4;
                _dicPositionColumnPackingLine.Add(WIDTH, column);
                workSheet.Column(column++).Width = 4;
                _dicPositionColumnPackingLine.Add(HEIGHT, column);
                workSheet.Column(column).Width = 4;

                if (_dicAlphabel.TryGetValue(column, out string endCartonDimensionCharacter))
                {
                    range = cartonDimensionCharacter + _maxRow + ":" + endCartonDimensionCharacter + (_maxRow + 1);
                    workSheet.Cells[range].Merge = true;
                }
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string measCharacter))
            {
                range = measCharacter + _maxRow + ":" + measCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = MEAS_M3;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(MEAS_M3, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalNetWeightCharacter))
            {
                range = totalNetWeightCharacter + _maxRow + ":" + totalNetWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_NET_WEIGHT;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 8;
                _dicPositionColumnPackingLine.Add(TOTAL_NET_WEIGHT, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalGrossWeightCharacter))
            {
                range = totalGrossWeightCharacter + _maxRow + ":" + totalGrossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_GROSS_WEIGHT;
                workSheet.Cells[_maxRow, column].Style.Font.Bold = true;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;

                string rangeBorder = "A9:" + totalGrossWeightCharacter + (_maxRow + 1);

                // border header packing line
                workSheet.Cells[rangeBorder].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[rangeBorder].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(column).Width = 8;
                _dicPositionColumnPackingLine.Add(TOTAL_GROSS_WEIGHT, column);
            }

            _maxColumn = column;
            _maxRow += 2;
        }

        public void FillDataPackingLine(ExcelWorksheet workSheet,
            Dictionary<string, BoxDimension> boxDimensions,
            IList<StyleNetWeight> netNetWeights, IList<StyleNetWeight> boxNetWeights,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            var dicStyleNetWeight = netNetWeights.ToDictionary(x => x.Size.Replace(" ", "").ToUpper());

            if (dicStyleNetWeight.Count == 0)
            {
                errorMessage = "StyleNetWeight not exists, please re-check!!!";
            }

            string range = "A" + _maxRow + ":";
            
            var config = new MapperConfiguration(
                       cfg => cfg.CreateMap<ItemStyleBarCode, ItemStyleBarCode>()
                       .ForMember(d => d.ItemStyle, o => o.Ignore())
                       );
            var mapper = new Mapper(config);

            // load color garment code - item barcode

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]]
                             .Value = _itemStyle.ColorCode;

            foreach (var itemBarcode in _itemStyle.Barcodes)
            {
                if (itemBarcode.Size.Replace(" ", "").ToUpper().Count() > itemBarcode.BarCode.Count())
                {
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
                                        .Value = itemBarcode.BarCode;
                    workSheet.Row(_maxRow).Height = 24;
                }
                else
                {
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
                                        .Value = itemBarcode.BarCode;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
                        .EntireColumn.AutoFit();
                    workSheet.Row(_maxRow).Height = 24;
                }

            }

            _maxRow++;
            _rowSum = _maxRow;
            /// fill data size
            Stack<ItemStyleBarCode> fillData = new Stack<ItemStyleBarCode>();

            var sizeList = _itemStyle.OrderDetails.OrderByDescending(x => x.SizeSortIndex).ToList();

            foreach (var itemSize in sizeList)
            {
                foreach (var itemBarcode in _itemStyle.Barcodes)
                {
                    if (itemSize.Size.Replace(" ", "").ToUpper().Equals(itemBarcode.Size.Replace(" ", "").ToUpper()))
                    {
                        fillData.Push(itemBarcode);
                    }
                }
            }

            int preToNo = 0;

            while (fillData.Count > 0)
            {
                ItemStyleBarCode itemBarcode = fillData.Pop();
                decimal pcb = 1;
                var styleNetWeight = dicStyleNetWeight[itemBarcode.Size.Replace(" ", "").ToUpper()];
                var boxDimension = boxDimensions[itemBarcode.Packing.Replace(" ", "").Replace(".0", "")];

                if (decimal.TryParse(itemBarcode.PCB, out decimal rsPCB))
                {
                    pcb = rsPCB;
                }

                int noOfCarton = 0;
                int surplus = 0;

                if (pcb <= itemBarcode.Quantity)
                {
                    noOfCarton = (int)(Math.Floor(itemBarcode.Quantity / pcb));
                    surplus = Convert.ToInt32(itemBarcode.Quantity % pcb);
                }
                else
                {
                    surplus = Convert.ToInt32(itemBarcode.Quantity % pcb);
                }

                if(noOfCarton > 0)
                {
                    if (preToNo == 0)
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = 1;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = noOfCarton;
                    }
                    else
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = preToNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = preToNo;
                    }


                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]]
                                 .Value = _itemStyle.Description;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
                             .Value = pcb;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                             .Value = pcb;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]]
                             .Value = noOfCarton;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                             .Value = pcb * noOfCarton;


                    var netWeight = styleNetWeight.NetWeight * pcb;
                    //var grossWeight = netWeight + (styleNetWeight?.BoxWeight ?? boxDimension?.Weight ?? 0);
                    decimal grossWeight = 0;
                    if (boxNetWeights?.Sum(x => x.BoxWeight ?? 0) == 0)
                        grossWeight = netWeight + boxDimension?.Weight ?? 0;
                    else
                    {
                        boxNetWeights.Where(x => x?.Size?.Trim().Replace(" ", "").ToUpper()
                                                == itemBarcode?.Size?.Trim().Replace(" ", "").ToUpper())
                            .ToList().ForEach(w =>
                            {
                                if (!string.IsNullOrEmpty(w?.BoxDimensionCode))
                                {
                                    if (w?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                        == boxDimension?.Code?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", ""))
                                        grossWeight = netWeight + w?.BoxWeight ?? 0;
                                }
                            });

                        if (grossWeight == 0)
                            grossWeight = netWeight + boxDimension?.Weight ?? 0;
                    }

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]]
                            .Value = netWeight;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                            .Value = grossWeight;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                            .Value = boxDimension.Length;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                            .Value = boxDimension.Width;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                            .Value = boxDimension.Height;

                    var measM3 = Math.Round(
                            noOfCarton
                            * (decimal)boxDimension.Length
                            * (decimal)boxDimension.Width
                            * (decimal)boxDimension.Height / 1000000, 3);

                    var measM3Formla = "ROUND("
                            + _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow + "*"
                            + _dicAlphabel[_dicPositionColumnPackingLine[LENGTH]] + _maxRow + "*"
                            + _dicAlphabel[_dicPositionColumnPackingLine[WIDTH]] + _maxRow + "*"
                            + _dicAlphabel[_dicPositionColumnPackingLine[HEIGHT]] + _maxRow + "/ 1000000, 3)";

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Formula = measM3Formla;
                            //.Value = measM3;

                    //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                    //        .Value = netWeight * noOfCarton;
                    //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                    //        .Value = grossWeight * noOfCarton;

                    var netFormula = _dicAlphabel[_dicPositionColumnPackingLine[NET_WEIGHT]] + _maxRow + "*"
                                            + _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow;

                    var grossFormula = _dicAlphabel[_dicPositionColumnPackingLine[GROSS_WEIGHT]] + _maxRow + "*"
                                            + _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                            .Formula = netFormula;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                            .Formula = grossFormula;

                    _total_TotalNetWeight += (decimal)netWeight * noOfCarton;
                    _totalGrossWeight += (decimal)grossWeight;
                    _total_TotalGrossWeight += (decimal)grossWeight * noOfCarton;
                    _totalNoOfCarton += noOfCarton;
                    _totalUnits += pcb * noOfCarton;
                    _totalMeasM3 += measM3;

                    if (_dicTotalCartons.TryGetValue(boxDimension.Code.Replace(" ", "").Replace(".0", ""), out int rsQty))
                    {
                        _dicTotalCartons[boxDimension.Code.Replace(" ", "").Replace(".0", "")] = rsQty + noOfCarton;
                    }
                    else
                    {
                        _dicTotalCartons[boxDimension.Code.Replace(" ", "").Replace(".0", "")] = noOfCarton;
                    }                   
                }


                if (surplus != 0)
                {
                    preToNo = noOfCarton + 1;

                    ItemStyleBarCode newItemStyleBarCode = new ItemStyleBarCode();

                    mapper.Map(itemBarcode, newItemStyleBarCode);

                    newItemStyleBarCode.Quantity = surplus;
                    newItemStyleBarCode.PCB = surplus.ToString();

                    fillData.Push(newItemStyleBarCode);

                    string modelRangeBorder = "";
                    if (noOfCarton > 0)
                    {
                        modelRangeBorder = "A" + (_maxRow + 1) + ":"
                                        + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                                        + (_maxRow + 1); 
                    }
                    else
                    {
                        modelRangeBorder = "A" + _maxRow + ":"
                                        + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                                        + _maxRow;
                    }

                    using (var range1 = workSheet.Cells[modelRangeBorder])
                    {
                        range1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range1.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    }

                    //workSheet.Row(_maxRow + 1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //workSheet.Row(_maxRow + 1).Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                }
                else
                {
                    preToNo = 0;
                }

                if(noOfCarton > 0)
                {
                    workSheet.Row(_maxRow).Height = 24;
                    _maxRow++;
                }
            }

            // set total quantity of size
            foreach (var itemBarcode in _itemStyle.Barcodes)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
                         .Value = itemBarcode.Quantity;
                workSheet.Row(_maxRow).Height = 24;
            }

            _maxRow++;

            // set UE
            foreach (var itemBarcode in _itemStyle.Barcodes)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
                         .Value = "UE = " + itemBarcode.UE;
                workSheet.Row(_maxRow).Style.Font.Bold = true;
                workSheet.Row(_maxRow).Height = 24;
            }

            // set border 
            // STYLE BORDER, FIT COLUMN
            if (_dicAlphabel.TryGetValue(_maxColumn, out string maxColumnCharacter))
            {
                range += maxColumnCharacter + (_maxRow);
                /// BORDER ALL DATA
                using (var borderData = workSheet.Cells[range])
                {
                    borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }

                /// BORDER FIRST TOTAL
                using (var borderData = workSheet.Cells[_maxRow, 1, _maxRow, 4])
                {
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    borderData.Style.Font.Bold = true;
                }

                using (var borderData = workSheet.Cells[range])
                {
                    borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }

                workSheet.Column(4).AutoFit();
            }
            _maxRow++;
        }

        void TotalCarton(ExcelWorksheet workSheet, IList<CuttingLot> cuttingLots)
        {
            _maxRow++;
            var sizeList = _itemStyle.OrderDetails.OrderByDescending(x => x.SizeSortIndex).ToList();
            string range = "D" + _maxRow + ":";
            string cellTotalNW = "";
            string cellTotalGW = "";
            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = "Lót/Size";
            //workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[COLOR]].Value = "Order Qty";

            //foreach (var itemBarcode in _itemStyle.Barcodes)
            //{
            //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
            //             .Value = itemBarcode.Size;

            //    workSheet.Cells[_maxRow + 1, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
            //             .Value = itemBarcode.Quantity;

            //    if (itemBarcode.Size.Count() > 5)
            //    {
            //        workSheet.Row(_maxRow).Height = 24;
            //    }
            //    else
            //    {
            //        workSheet.Row(_maxRow).Height = 18;
            //    }
            //}

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = "Total";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = _totalNoOfCarton;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalUnits;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value = _totalGrossWeight;
            var totGrossFormula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[GROSS_WEIGHT]] + _rowSum + ":"
                                + _dicAlphabel[_dicPositionColumnPackingLine[GROSS_WEIGHT]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Formula = totGrossFormula;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Value = _totalMeasM3;
            var totMeasM3Formula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3]] + _rowSum + ":"
                              + _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Formula = totMeasM3Formula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]].Value = _total_TotalNetWeight;
            var totTotalNetFormula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]] + _rowSum + ":"
                               + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]].Formula = totTotalNetFormula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cellTotalNW = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]] + _maxRow;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]].Value = _total_TotalGrossWeight;
            var totTotalGrossFormula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]] + _rowSum + ":"
                               + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]].Formula = totTotalGrossFormula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cellTotalGW = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]] + _maxRow;
            /// BORDER
            //if (_dicAlphabel.TryGetValue(_dicPositionColumnPackingLine[UNITS_CARTON] - 1, out string columnUnitCartonCharacter))
            //{
            //    range += columnUnitCartonCharacter + (_maxRow + 1);

            //    using (var borderData = workSheet.Cells[range])
            //    {
            //        borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            //        borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //        borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //        borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //        borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //        borderData.Style.Font.Bold = true;
            //        borderData.Style.WrapText = true;
            //    }
            //}
            _maxRow++;
            var rowCutting = _maxRow;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = "Cnts";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = "PCS";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value = "Kg";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]].Value = "Kg";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]].Value = "Kg";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            _maxRow += 2;


            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = "TOTAL NW:";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                            _maxRow, _dicPositionColumnPackingLine[UNITS_CARTON] + 1].Merge = true;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _total_TotalNetWeight;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Formula = cellTotalNW;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]].Value = "Kg";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            _maxRow++;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = "TOTAL GW:";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                            _maxRow, _dicPositionColumnPackingLine[UNITS_CARTON] + 1].Merge = true;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _total_TotalGrossWeight;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Formula = cellTotalGW;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]].Value = "Kg";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            _maxRow += 2;

            if (_dicAlphabel.TryGetValue(_dicPositionColumnPackingLine[UNITS_CARTON], out string character))
            {
                range = character + _maxRow + ":";
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = "CTNS SUMMARY";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                            _maxRow, _dicPositionColumnPackingLine[UNITS_CARTON] + 1].Merge = true;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = "QTY";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            _maxRow++;

            foreach (var itemCarton in _dicTotalCartons)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = itemCarton.Key;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                                _maxRow, _dicPositionColumnPackingLine[UNITS_CARTON] + 1].Merge = true;

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = itemCarton.Value;

                _maxRow++;
            }

            if (_dicAlphabel.TryGetValue(_dicPositionColumnPackingLine[TOTAL_UNITS], out string characterTotal))
            {
                range += characterTotal + (_maxRow - 1);
            }

            /// BORDER CTNS
            using (var cell = workSheet.Cells[range])
            {
                cell.Style.Font.SetFromFont(new Font("Arial", 10));
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }

            /// Handle load cutting lot
            if (_dicAlphabel.TryGetValue(_dicPositionColumnPackingLine[COLOR], out string charFirstSize))
            {
                range = charFirstSize + rowCutting;
            }
            else
            {
                range = "D" + rowCutting;
            }
                
            workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]]
                                .Value = "Size";
            workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]]
                    .Style.Font.SetFromFont(new Font("Arial", 10));
            workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]]
                     .Style.Font.Bold = true;
            

            foreach (var itemSize in sizeList)
            {
                var size = itemSize.Size.Replace(" ", "").ToUpper();
                workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]].Value = itemSize.Size;
                workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]]
                        .Style.Font.SetFromFont(new Font("Arial", 10));
                workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]].Style.Font.Bold = true;
                workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]].Style.WrapText = true;
            }

            rowCutting++;

            workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]].Value = "Order Qty";
            workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]]
                                            .Style.Font.SetFromFont(new Font("Arial", 10));
            workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]].Style.Font.Bold = true;

            foreach (var order in _itemStyle.OrderDetails)
            {
                var size = order.Size.Replace(" ", "").ToUpper();
                workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]].Value = order.Quantity;
                workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]]
                                            .Style.Font.SetFromFont(new Font("Arial", 10));
            }

            if(cuttingLots.Count > 0)
            {
                var lotDetails = new Dictionary<string, decimal>();
                foreach (var lot in cuttingLots.OrderBy(x => x.Lot).ToList())
                {
                    var key = lot.Lot + ";" + lot.Size.Replace(" ", "").ToUpper(); 
                    if(!lotDetails.ContainsKey(key))
                    {
                        lotDetails.Add(key, lot.Quantity);
                    }
                    else
                    {
                        lotDetails[key] += lot.Quantity;
                    }
                }

                var lotNumber = "";
                foreach (var lot in lotDetails.OrderBy(x => x.Key).ToList())
                {
                    var size = lot.Key.Split(";")[1];
                    if (lot.Key.Split(";")[0] != lotNumber)
                    {
                        rowCutting++;
                        workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]].Value = lot.Key.Split(";")[0];
                        workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[COLOR]]
                                                .Style.Font.SetFromFont(new Font("Arial", 10));

                        workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]].Value = lot.Value;
                        workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]]
                                                .Style.Font.SetFromFont(new Font("Arial", 10));
                        lotNumber = lot.Key.Split(";")[0];
                    }
                    else
                    {
                        workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]].Value = lot.Value;
                        workSheet.Cells[rowCutting, _dicPositionColumnPackingLine[size]]
                                                .Style.Font.SetFromFont(new Font("Arial", 10));
                        lotNumber = lot.Key.Split(";")[0];
                    }
                }
            }

            if (_dicAlphabel.TryGetValue(_dicPositionColumnPackingLine[UNITS_CARTON] - 1, out string charLastSize))
            {
                range += ":" + charLastSize + rowCutting;
            }

            /// BORDER 
            using (var cell = workSheet.Cells[range])
            {  
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            }

            if (rowCutting > _maxRow)
                _maxRow = rowCutting++;

            _maxRow++;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Value = "LEADING STAR VIETNAM GARMENT  COMPANY LIMITED";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                     .Style.Font.Italic = true;

        }

        void ResetData()
        {
            _maxRow = 1;
            _maxColumn = 0;
            _totalNoOfCarton = 0;
            _totalUnits = 0;
            _total_TotalNetWeight = 0;
            _totalGrossWeight = 0;
            _totalMeasM3 = 0;
            _countSize = 0;
            _total_TotalGrossWeight = 0;

            _itemStyle = new ItemStyle();
            _dicTotalCartons = new Dictionary<string, int>();
            _dicPositionColumnPackingLine = new Dictionary<string, int>();
        }

        private void SetDictionaryAlphabet(int range = 1)
        {
            _dicAlphabel = new Dictionary<int, string>();
            int colNumber = 1; // start = 1
            int rangeCol = range; // A,B,C, ..., AA, AB,... BA, BB,... CA, CB... default 26 character
            string[] strAlpha = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
                                  "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            for (int i = 0; i <= rangeCol; i++)
            {
                if (i == 0) // range 1 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        _dicAlphabel.Add(colNumber++, strAlpha[j]);
                    }
                }
                else if (i <= 26) // range 2 character excel
                {
                    for (int j = 0; j < strAlpha.Count(); j++)
                    {
                        _dicAlphabel.Add(colNumber++, strAlpha[i - 1] + strAlpha[j]);
                    }
                }
            }
        }

        private Image LogoDownload(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        /// Insert data packing list DE
        private void SavePackingList(ItemStyle itemStyle, Dictionary<string, BoxDimension> boxDimensions,
            IList<StyleNetWeight> styleNetWeights)
        {
            var objectSpacePackingList = Application.CreateObjectSpace();
            var newPackingList = objectSpacePackingList.CreateObject<PackingList>();
            var criteria = CriteriaOperator.Parse("[Number] IN ('" + itemStyle.Number + "')");
            var itemStyles = objectSpacePackingList.GetObjects<ItemStyle>(criteria);

            newPackingList.PackingListCode = "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6);
            newPackingList.PackingListDate = DateTime.Now;
            newPackingList.CompanyCode = "LS";
            newPackingList.LSStyles = itemStyle?.LSStyle;
            newPackingList.CustomerID = itemStyle?.SalesOrder?.CustomerID;
            newPackingList.SetCreateAudit(SecuritySystem.CurrentUserName);
            newPackingList.SalesOrderID = itemStyle?.SalesOrderID; 
            newPackingList.OrdinalShip = 1;
            newPackingList.SheetNameID = 1;
            newPackingList.Confirm = true;
            newPackingList.PackingListImageThumbnails.Add(new PackingListImageThumbnail
            {
                ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListDE").ToString(),
                SortIndex = 1
            });

            newPackingList.PackingLines = DE_PackingLineProcess
                .Generate(styleNetWeights.ToList(), boxDimensions, itemStyle?.OrderDetails.ToList(), itemStyle?.Barcodes.ToList());
            if(itemStyles.FirstOrDefault().PackingOverQuantities.Any())
            {
                itemStyles.FirstOrDefault().PackingOverQuantities.ToList().ForEach(x =>
                {
                    x.Quantity = 0;
                });
            }
            else
            {
                itemStyles.FirstOrDefault().OrderDetails.ToList().ForEach(x =>
                {
                    var overQuantity = new PackingOverQuantity()
                    {
                        Quantity = 0,
                        ColorCode = itemStyle.ColorCode,
                        ColorName = itemStyle.ColorName,
                        ItemStyleNumber = itemStyle.Number,
                        Size = x.Size,
                        SizeSortIndex = x.SizeSortIndex
                    };
                    itemStyles.FirstOrDefault().PackingOverQuantities.Add(overQuantity);
                });
            }
         
            newPackingList.ItemStyles = itemStyles;
            newPackingList.TotalQuantity = newPackingList.PackingLines.Sum(x => x.TotalQuantity);

            objectSpacePackingList.CommitChanges();
        }
    }
}

