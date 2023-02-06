using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ExportMultiShipPackingList_Win_ViewController : ExportMultiShipPackingList_ViewController
    {
        private const string FROM_NO = "From No";
        private const string TO_NO = "To No";   
        private const string CARTON_NUMBER = "Carton Number";
        private const string COLOR = "Color";
        private const string SIZE = "SIZE";
        private const string UNITS_CARTON = "Units / carton";
        private const string UNITS_MASTER_CARTON = "Units / Master carton";
        private const string NO_OF_CARTON = "No. of Carton";
        private const string NO_OF_INNER_CARTONS = "No. of Inner Cartons";
        private const string NO_OF_MASTER_CARTONS = "No. of Master Cartons";
        private const string NO_OF_QTY_PREPACK = "No. of Qty Pre-pack";
        private const string TOTAL_UNITS = "Total Units";
        private const string PREPACK_TOTAL_QTY = "Pre-pack Total Qty";
        private const string PREPACK_QTY = "Prepack Qty";
        private const string NET_WEIGHT = "Net Weight (kg)";
        private const string GROSS_WEIGHT = "Gross Weight (kg)";
        private const string CARTON_DIMENSION = "Carton Dimension";
        private const string LENGTH = "Length";
        private const string HEIGHT = "Height";
        private const string WIDTH = "Width";
        private const string INNER_CARTON_DIMENSION = "Inner Carton Dimension";
        private const string INNER_LENGTH = "InnerLength";
        private const string INNER_HEIGHT = "InnerHeight";
        private const string INNER_WIDTH = "InnerWidth";
        private const string MASTER_CARTON_DIMENSION = "Master Carton Dimension";
        private const string MEAS_M3 = "Meas M3";
        private const string TOTAL_NET_WEIGHT = "Total N.Weight (kg)";
        private const string TOTAL_GROSS_WEIGHT = "Total G.Weight (kg)";
        private const string JAPAN = "JPPUJ"; // CUSTOMER CODE
        private const string CANADA = "CAWIN"; // UCUSTOMER CODE
        private const string ARUNI = "ARUNI"; // UCUSTOMER CODE
        private const string PANAMA = "PAWOR"; // UCUSTOMER CODE

        /// IFG & HA const
        private const string STYLE_NO_IFG = "Style No (order)";
        private const string STYLE_IFG = "STYLE (spm)";
        private const string COLOR_DESCR_IFG = "COLOR DESC (spm)";
        private const string COLOR_CODE_IFG = "Color code";
        private const string LABEL_CODE_IFG = "Label code";
        private const string SIZE_IFG = "Size";
        private const string NET_WEIGHT_IFG = "Net Weight";
        private const string GROSS_WEIGHT_IFG = "Gross Weight";
        private const string CARTON_DIMENSION_IFG = "Carton Dimension";
        private const string LENGTH_IFG = "L";
        private const string HEIGHT_IFG = "H";
        private const string WIDTH_IFG = "W";
        private const string MEAS_M3_IFG = "Meas (M3)";
        private const string TOTAL_NET_WEIGHT_IFG = "Total N.Weight";
        private const string TOTAL_GROSS_WEIGHT_IFG = "Total G.Weight";

        /// GA const
        private const string BARCODE_NO_GA = "Barcode No.";
        private const string COLOR_CODE_GA = "Color Code";
        private const string STYLE_DESCRIPTON = "STYLE DESCRIPTION";
        private const string SIZE_GA = "Size";
        private const string UNITS_POLYBAG = "Units/ polybag";
        private const string POLYBAGS_CARTON = "Polybags/ carton";
        private const string MEAS_M3_GA = "Meas (M3)";
        private const string TOTAL_NET_WEIGHT_GA = "Total N.Weight";
        private const string TOTAL_GROSS_WEIGHT_GA = "Total G.Weight";

        /// HM conts
        private const string COLOR_NAME = "COLOR NAME";
        private const string BODY_COLOR = "BODY COLOR/ART NAME";
        private const string SIZE_COLOR = "size/color";

        ///  HA const
        private const string COLOR_FASHION = "COLOR FASHION";
        private const string COLOR_HA = "COLOR";

        private int _countSize = 0;
        private int _maxColumn = 0;
        private int _maxRow = 1;
        private int _mergeColumn = 0;
        private bool _isCanada = false;
        private bool _isJapan = false;
        private bool _isAruni = false;
        private bool _isPanama = false;
        private bool _isAssortedSize_AssortedColor = false;
        private bool _isFitToPage = false;
        private string _LSStyle = string.Empty;
        private decimal _totalNoOfCarton = 0;
        private decimal _totalUnits = 0;
        private decimal _totalNetWeight = 0;
        private decimal _totalGrossWeight = 0;
        private decimal _unitCarton = 0;
        private bool? _dontShortShip;
        private Dictionary<int, string> _dicAlphabel; // 1 = A, 2 = B
        private Dictionary<string, int> _dicPositionColumnPackingLine; // Units / carton = 10, No. of Carton = 11
        private Dictionary<string, decimal> _dicTotalQuantitySize; // S = 100, X = 50
        private Dictionary<string, decimal> _dicShipmentQuantity; // S = 100, X = 50
        private Dictionary<string, int> _dicTotalCartons; // S = 100, X = 50
        private Dictionary<string, int> _dicTotalInnerCartons; // S = 100, X = 50
        private ItemStyle _itemStyle;
        private string _style_IFG = "";
        private string _color_Descr_IFG = "";
        private Dictionary<string, decimal> _dicTotalQuantitySizeOverStyle;
        private string _length_Unit_IFG = "";
        private string _weight_Unit_IFG = "";
        private string _packingType = "";
        private bool _checkAssortedStyle = false;
        private List<int> _oddCarton;
        private decimal _totalGrossWeightCarton = 0;
        private decimal _totalMeanM3 = 0;
        private int _rowSum = 0;
        private bool _multiShip = true;
        private Dictionary<int, string> _dicSizeList;
        public override void ExportMultiShipPackingList(object sender, SimpleActionExecuteEventArgs e)
        {
            base.ExportMultiShipPackingList(sender, e);
            MessageOptions options = null;

            var viewObject = View.CurrentObject as PackingListCreateParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)View)
               .FindItem("PackingLists") as ListPropertyEditor;

            var selectedPackingList = listPropertyEditor.ListView
                .SelectedObjects.Cast<PackingList>().ToList();

            var criteria = CriteriaOperator
                .Parse($"[ID] IN ({string.Join(",", selectedPackingList.Select(x => x.ID))})");
            var export = ObjectSpace.GetObjects<PackingList>(criteria).ToList();

            string fileName = "";
            _multiShip = true;
            SaveFileDialog dialog = new SaveFileDialog();

            var firstPackingList = export?.First();
            _itemStyle = firstPackingList?.ItemStyles?.FirstOrDefault();
            var errorMessage = "";

            foreach(var data in export)
            {
                if (!data.ItemStyles.ToList().Any())
                {
                    errorMessage = "Item style is null, please contact to admin or ERP team";
                    break;
                }

                if (!data.PackingLines.ToList().Any())
                {
                    errorMessage = "Packing line is null, please contact to admin or ERP team";
                    break;
                    
                }      
                
                foreach(var style in data?.ItemStyles)
                {
                    if (style.ItemStyleStatusCode?.Trim() == "3")
                    {
                        errorMessage = String.Format("Item Style: {0} was canceled", style.LSStyle);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                options = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(options);
                return;
            }

            if (firstPackingList?.CustomerID != "HA")
            {
                foreach (var list in export?.Where(x => x.ID != firstPackingList.ID))
                {
                    if (firstPackingList?.ItemStyles?.Count != list?.ItemStyles?.Count)
                    {
                        _multiShip = false;
                        break;
                    }
                    else
                    {
                        foreach (var style in list?.ItemStyles)
                        {
                            if (!firstPackingList.ItemStyles.Contains(style))
                            {
                                _multiShip = false;
                                break;
                            }
                        }
                    }

                    if (!_multiShip)
                        break;
                }
            }
            
            if(_multiShip)
            {
                if (firstPackingList?.CustomerID == "GA" || firstPackingList?.CustomerID == "DE" || firstPackingList?.CustomerID == "IFG")
                {
                    export = export.Where(x => !string.IsNullOrEmpty(x?.SheetNameID.ToString()))
                                            .OrderBy(x => x?.SheetName?.SheetSortIndex).ToList();

                    if (!export.Any())
                    {
                        options = Message.GetMessageOptions("Please update sheet name before", "Error",
                           InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(options);
                        return;
                    }
                }
                else if (firstPackingList?.CustomerID == "HA")
                {
                    export = export.OrderBy(x => x?.PackingListDate).ToList();
                }

                if (_dicAlphabel == null)
                {
                    SetDictionaryAlphabet(3);
                }

                //if (_itemStyle == null)
                //{
                //    options = Message.GetMessageOptions("Item style is null, please contact to admin or ERP team", "Error",
                //       InformationType.Error, null, 5000);
                //    Application.ShowViewStrategy.ShowMessage(options);
                //    return;
                //}
                if (firstPackingList?.CustomerID == "HA")
                {
                    export.ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(_LSStyle))
                            _LSStyle = x.LSStyles;
                        else
                            _LSStyle += " & " + x.LSStyles;
                    });
                }
                else
                    CheckAssortedSize_AssortedColor(firstPackingList);

                fileName = firstPackingList?.CustomerID == "PU" ?
                                  _itemStyle.LSStyle.Replace('/', '-') : _LSStyle;
            }
            else
            {
                if (_dicAlphabel == null)
                {
                    SetDictionaryAlphabet(3);
                }

                fileName = firstPackingList?.CustomerID + " - PackingList";
            }

            dialog.FileName = fileName;

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (firstPackingList?.CustomerID)
                    {
                        case "GA":
                            {
                                var stream = CreateExcelFile_GA(export);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                                options = Message.GetMessageOptions("Export successfully. ", "Successs",
                               InformationType.Success, null, 5000);
                            }
                            break;
                        case "HA":
                            {
                                var stream = CreateExcelFile_HA(export);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                                options = Message.GetMessageOptions("Export successfully. ", "Successs",
                               InformationType.Success, null, 5000);
                            }
                            break;
                        case "DE":
                            {
                                var stream = CreateExcelFile_DE(export);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                                options = Message.GetMessageOptions("Export successfully. ", "Successs",
                               InformationType.Success, null, 5000);
                            }
                            break;
                        case "IFG":
                            {
                                var stream = CreateExcelFile_IFG(export);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                                options = Message.GetMessageOptions("Export successfully. ", "Successs",
                               InformationType.Success, null, 5000);
                            }
                            break;
                    }

                }
                catch (Exception EE)
                {
                    options = Message.GetMessageOptions("Export failed. " + EE.Message, "Error",
                   InformationType.Error, null, 5000);

                }

                Application.ShowViewStrategy.ShowMessage(options);
                View.Refresh();
            }
        }

        #region GARAN
        private Stream CreateExcelFile_GA(List<PackingList> packingLists, Stream stream = null)
        {
            string Author = "Leading Star VN";
            var indexSheet = 0;

            if (!String.IsNullOrEmpty(packingLists?.First().CreatedBy))
            {
                Author = packingLists?.First().CreatedBy;
            }

            var sortThumbnail = packingLists.First().PackingListImageThumbnails.OrderBy(x => x.SortIndex).ToList();

            byte[] shippingImage1 = null;

            if (sortThumbnail.Count > 0)
            {
                shippingImage1 = sortThumbnail[0].ImageData;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = _LSStyle;
                excelPackage.Workbook.Properties.Comments = "Packing List of Leading Start VN";

                foreach(var packingList in packingLists)
                {
                    string Title = "";
                    if (_multiShip)
                    {
                        Title = packingList.SheetName.SheetName;
                    }
                    else
                    {
                        Title = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle + "-" +
                                ((packingList.SheetNameID ?? 0) != 0 ? packingList?.SheetName?.SheetName : "Total");
                    }
                    excelPackage.Workbook.Worksheets.Add(Title);
                    ResetData();

                    _itemStyle = packingList.ItemStyles?.FirstOrDefault();
                    //_countSize = _itemStyle.OrderDetails.Count();
                    GetSizeList(packingList);

                    var workSheet = excelPackage.Workbook.Worksheets[indexSheet];

                    workSheet.Cells.Style.Font.SetFromFont(new Font("Arial", 8));

                    CreateHeaderPage_GA(workSheet, packingList);
                    CreateHeaderPackingLine_GA(workSheet, packingList);
                    FillDataPackingLine_GA(workSheet, packingList);
                    SummaryQuantitySize_Carton_GA(workSheet, packingList, shippingImage1);

                    string modelRangeBorder = "A1:"
                                            + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]]
                                            + (_maxRow + 9);

                    using (var range = workSheet.Cells[modelRangeBorder])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }

                    if (_dicAlphabel.TryGetValue(_maxColumn, out string characterColumnPacking))
                    {
                        using (var range = workSheet.Cells["A1:" + characterColumnPacking + "1"])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont(new Font("Arial", 14));
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }
                    workSheet.PrinterSettings.FitToPage = true;
                    workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    workSheet.PrinterSettings.VerticalCentered = true;
                    workSheet.PrinterSettings.HorizontalCentered = true;

                    indexSheet++;
                }
                
                excelPackage.Save();
                return excelPackage.Stream;
            }

        }
        public void CreateHeaderPage_GA(ExcelWorksheet workSheet, PackingList packingList)
        {
            workSheet.Cells[1, 1].Value = "PACKING LIST";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);

            //_countSize = packingList?.ItemStyles?.First()?.OrderDetails
            //                .Where(x => x.Quantity > 0).ToList().Count() ?? 0;
            _countSize = _dicSizeList.Count;
            int posCustomer = 10 + _countSize;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "JOB NO:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].EntireColumn.Width = 5.5;
            workSheet.Cells[_maxRow, 2].EntireColumn.Width = 2;
            workSheet.Cells[_maxRow, 3].EntireColumn.Width = 5.5;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 5].Value = packingList.LSStyles.Replace(";", " / ");
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            _maxRow++;
            var styles = "";
            //if (packingList.ItemStyles.Select(x => x.CustomerStyle).Count() > 1)
            //{
            //    foreach (var itemStyle in packingList.ItemStyles.OrderBy(x => x.LSStyle))
            //    {
            //        if (string.IsNullOrEmpty(styles))
            //            styles = itemStyle.CustomerStyle;
            //        else
            //            styles += " / " + itemStyle.CustomerStyle;
            //    }
            //}
            //else
            //{
            //    styles = packingList.ItemStyles.FirstOrDefault().CustomerStyle;
            //}

            if (packingList.ItemStyles.Select(x => x.CustomerStyle).Distinct().Count() > 1)
            {
                _checkAssortedStyle = true;
                foreach (var itemStyle in packingList.ItemStyles.OrderBy(x => x.LSStyle))
                {
                    if (string.IsNullOrEmpty(styles))
                        styles = itemStyle.CustomerStyle;
                    else
                        styles += " / " + itemStyle.CustomerStyle;
                }
            }
            else
            {
                _checkAssortedStyle = false;
                styles = packingList.ItemStyles.FirstOrDefault().CustomerStyle;
            }
            var shippingStyle = "";
            var shippingColor = "";
            foreach (var itemStyle in packingList.ItemStyles.ToList())
            {
                if (!string.IsNullOrEmpty(itemStyle.ShipColor) && string.IsNullOrEmpty(shippingColor))
                {
                    shippingColor = itemStyle.ShipColor;
                }
                if (!string.IsNullOrEmpty(itemStyle.ShippingStyle) && string.IsNullOrEmpty(shippingStyle))
                {
                    shippingStyle = itemStyle.ShippingStyle;
                }
            }

            workSheet.Cells[_maxRow, 1].Value = "STYLE NO:";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 5].Value = styles;
            workSheet.Cells[_maxRow, posCustomer].Value = "DATE:";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "ORDER STYLE:";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 5].Value = _itemStyle.ExternalPurchaseOrderTypeName ?? string.Empty;
            workSheet.Cells[_maxRow, posCustomer].Value = "CST No:";
            workSheet.Cells[_maxRow, posCustomer].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = packingList.Customer?.Name;


            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIP COLOR";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 5].Value = shippingColor;
            workSheet.Cells[_maxRow, posCustomer].Value = "ORDER NO:";
            workSheet.Cells[_maxRow, posCustomer].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = _itemStyle?.PurchaseOrderNumber ?? string.Empty;
            workSheet.Cells[_maxRow, posCustomer + 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIPPING STYLE#";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 5].Value = shippingStyle;

            if (_packingType == "SolidSize_SolidColor" || _packingType == "SolidSize_AssortedColor")
            {
                workSheet.Cells[_maxRow, 9].Value = !_checkAssortedStyle ? "solid size" : string.Empty;
            }
            else
            {
                //var ratioSize = "";
                //workSheet.Cells[_maxRow, 7].Value = packingList.PackingLines.Sum(x => x.Quantity);
                //                                    // /packingList.ItemStyles.Select(x => x.CustomerStyle).Distinct().Count();
                //workSheet.Cells[_maxRow, 7].Style.Numberformat.Format = "0##";
                //packingList.PackingLines.Where(x => x.LSStyle == packingList.ItemStyles.First().LSStyle)
                //    .OrderBy(x => x.SequenceNo).ToList().ForEach(x =>
                //    {
                //        if (string.IsNullOrEmpty(ratioSize))
                //            ratioSize = ((int)x.Quantity).ToString();
                //        else
                //            ratioSize += "-" + ((int)x.Quantity).ToString();
                //    });
                //workSheet.Cells[_maxRow, 9].Value = ratioSize;
                var pack = "";
                workSheet.Cells[_maxRow, 7].Value = packingList.PackingLines.Sum(x => x.Quantity);
                workSheet.Cells[_maxRow, 7].Style.Numberformat.Format = "0##";

                foreach (var style in packingList?.ItemStyles.OrderBy(x => x.LSStyle))
                {
                    var ratioSize = "";
                    packingList.PackingLines.Where(x => x.LSStyle == style.LSStyle)
                    .OrderBy(x => x.SequenceNo).ToList().ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(ratioSize))
                            ratioSize = ((int)x.Quantity).ToString();
                        else
                            ratioSize += "-" + ((int)x.Quantity).ToString();
                    });

                    if (string.IsNullOrEmpty(pack))
                        pack = ratioSize;
                    else if (!pack.Contains(ratioSize))
                        pack += "/" + ratioSize;
                }
                workSheet.Cells[_maxRow, 9].Value = pack;
            }
        }
        public void CreateHeaderPackingLine_GA(ExcelWorksheet workSheet, PackingList packinglist)
        {
            int column = 1;
            _maxRow++;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            string range = "A" + _maxRow + ":C" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = CARTON_NUMBER;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(FROM_NO, 1); // add position column 
            _dicPositionColumnPackingLine.Add(TO_NO, 3);

            column += 3;

            range = "D" + _maxRow + ":D" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = BARCODE_NO_GA;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Column(column).Width = 20;
            _dicPositionColumnPackingLine.Add(BARCODE_NO_GA, column);

            column++;

            range = "E" + _maxRow + ":E" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Column(column).Width = 10;
            _dicPositionColumnPackingLine.Add(COLOR, column);

            column++;

            range = "F" + _maxRow + ":F" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR_CODE_GA;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(COLOR_CODE_GA, column);

            column++;

            range = "G" + _maxRow + ":G" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = STYLE_DESCRIPTON;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Column(column).Width = 12;
            _dicPositionColumnPackingLine.Add(STYLE_DESCRIPTON, column);

            column++;

            workSheet.Cells[_maxRow, column].Value = SIZE_GA;
            _dicPositionColumnPackingLine.Add(SIZE_GA, column);
            //var sizeList = _itemStyle.OrderDetails.Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList();

            //for (int i = 0; i < _countSize; i++)
            //{
            //    string size = sizeList[i].Size;
            //    if (!string.IsNullOrEmpty(size))
            //    {
            //        workSheet.Cells[_maxRow + 1, column].Value = size;
            //        workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;
            //        _dicPositionColumnPackingLine.Add(size, column);
            //        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 7;
            //        column++;
            //    }
            //}

            foreach (var data in _dicSizeList
                .OrderBy(s => s.Key).ToList())
            {
                var size = data.Value;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;
                    _dicPositionColumnPackingLine.Add(size, column);
                    workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 7;
                    column++;
                }
            }

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_GA]] + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_packingType != "SolidSize_SolidColor" && _packingType != "SolidSize_AssortedColor" || _checkAssortedStyle)
            {
                if (_dicAlphabel.TryGetValue(column, out string unitsPolybagCharacter))
                {
                    range = unitsPolybagCharacter + _maxRow + ":" + unitsPolybagCharacter + (_maxRow + 1);
                    workSheet.Cells[_maxRow, column].Value = UNITS_POLYBAG;
                    workSheet.Cells[range].Merge = true;
                    workSheet.Cells[range].Style.WrapText = true;
                    workSheet.Column(column).Width = 7;
                    _dicPositionColumnPackingLine.Add(UNITS_POLYBAG, column);
                }

                column++;

                if (_dicAlphabel.TryGetValue(column, out string polybagsCartonCharacter))
                {
                    range = polybagsCartonCharacter + _maxRow + ":" + polybagsCartonCharacter + (_maxRow + 1);
                    workSheet.Cells[_maxRow, column].Value = POLYBAGS_CARTON;
                    workSheet.Cells[range].Merge = true;
                    workSheet.Cells[range].Style.WrapText = true;
                    workSheet.Column(column).Width = 7;
                    _dicPositionColumnPackingLine.Add(POLYBAGS_CARTON, column);
                }

                column++;
            }

            if (_dicAlphabel.TryGetValue(column, out string unitsCharacter))
            {
                range = unitsCharacter + _maxRow + ":" + unitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = UNITS_CARTON;
                workSheet.Cells[range].Merge = true;
                workSheet.Cells[range].Style.WrapText = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(UNITS_CARTON, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string noOfCharacter))
            {
                range = noOfCharacter + _maxRow + ":" + noOfCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NO_OF_CARTON;
                _dicPositionColumnPackingLine.Add(NO_OF_CARTON, column);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;

            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))
            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string netWeightCharacter))
            {
                range = netWeightCharacter + _maxRow + ":" + netWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NET_WEIGHT;
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
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(GROSS_WEIGHT, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string cartonDimensionCharacter))
            {
                workSheet.Cells[_maxRow, column].Value = CARTON_DIMENSION + " (inch)"; ;
                _dicPositionColumnPackingLine.Add(CARTON_DIMENSION, column);

                _dicPositionColumnPackingLine.Add(LENGTH, column);
                workSheet.Column(column).Width = 5;

                column++;
                _dicPositionColumnPackingLine.Add(WIDTH, column);
                workSheet.Column(column).Width = 5;

                column++;
                _dicPositionColumnPackingLine.Add(HEIGHT, column);
                workSheet.Column(column).Width = 5;

                if (_dicAlphabel.TryGetValue(column, out string endCartonDimensionCharacter))
                {
                    range = cartonDimensionCharacter + _maxRow + ":" + endCartonDimensionCharacter + (_maxRow + 1);
                    workSheet.Cells[range].Merge = true;
                    workSheet.Cells[range].Style.WrapText = true;
                }
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string measCharacter))
            {
                range = measCharacter + _maxRow + ":" + measCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = MEAS_M3_GA;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(MEAS_M3_GA, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalNetWeightCharacter))
            {
                range = totalNetWeightCharacter + _maxRow + ":" + totalNetWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_NET_WEIGHT_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 8;
                _dicPositionColumnPackingLine.Add(TOTAL_NET_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalGrossWeightCharacter))
            {
                range = totalGrossWeightCharacter + _maxRow + ":" + totalGrossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_GROSS_WEIGHT_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_GROSS_WEIGHT_IFG, column);

                string rangeBorder = "A8:" + totalGrossWeightCharacter + (_maxRow + 1);

                // border header packing line
                workSheet.Cells[rangeBorder].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[rangeBorder].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(column).Width = 8;
            }

            workSheet.Row(_maxRow).Height = 18;
            workSheet.Row(_maxRow + 1).Height = 18;

            _maxColumn = column;
            _maxRow += 2;
        }
        public void FillDataPackingLine_GA(ExcelWorksheet workSheet, PackingList packinglist)
        {
            string range = "A" + _maxRow + ":";
            decimal totalMeasM3 = 0;
            decimal netWeight = 0;
            decimal grossWeight = 0;
            decimal measM3 = 0;
            bool firstRow = true;
            int fromNo = 0;
            int summaryCarton = 0;
            int styleRow = 0;
            string lsStyle = "";
            var itemStyle = new ItemStyle();
            var mergeCell = new List<int>();
            var styles = packinglist.ItemStyles.OrderBy(x => x.LSStyle).ToList();
            var totalStyle = packinglist.PackingLines.Select(x => x.LSStyle).Distinct().Count();
            var inchConvertValue = 1728 * 35.31;
            var styleColorName = "";
            var styleColorCode = "";
            var styleColorDescription = "";

            _dicTotalQuantitySizeOverStyle = new Dictionary<string, decimal>();
            _dicTotalQuantitySize = new Dictionary<string, decimal>();
            styleRow = _maxRow;

            var style = packinglist.ItemStyles.OrderBy(x => x.LSStyle).FirstOrDefault();
            var sortPackingLines = packinglist.PackingLines
                    .Where(x => x.LSStyle == style.LSStyle).OrderBy(x => x.SequenceNo).ToList();

            //var sizePacking = sortPackingLines?.FirstOrDefault()?.Size;
            foreach (var data in styles)
            {
                if (string.IsNullOrEmpty(styleColorName.Trim()))
                    styleColorName = data.ColorName.Trim();
                else
                    styleColorName += " // " + data.ColorName.Trim();

                if (string.IsNullOrEmpty(styleColorCode.Trim()))
                    styleColorCode = data.ColorCode.Trim();
                else
                    styleColorCode += " // " + data.ColorCode.Trim();

                if (string.IsNullOrEmpty(styleColorDescription.Trim()))
                    styleColorDescription = data.Description.Trim();
                else
                    styleColorDescription += " // " + data.Description.Trim();

                /// Calculation net weight for assorted
                netWeight += (decimal)(packinglist.PackingLines
                    .Where(x => x.LSStyle == data.LSStyle).FirstOrDefault().NetWeight);
            }

            // Fill Data 
            if (_packingType == "SolidSize_SolidColor" || _packingType == "SolidSize_AssortedColor")
            {
                foreach (var packingLine in sortPackingLines)
                {
                    /// Convert packing unit
                    measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                    if (_checkAssortedStyle)
                    {
                        netWeight = (decimal)(packinglist.PackingLines.Where(x => x.Size == packingLine.Size).Sum(x => x.NetWeight));
                        grossWeight = (decimal)(netWeight + (packingLine.BoxDimension.Weight * packingLine.TotalCarton));
                    }
                    else
                    {
                        netWeight = (decimal)packingLine.NetWeight * totalStyle;
                        grossWeight = (decimal)(netWeight + packingLine.GrossWeight - packingLine.NetWeight);
                    }

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[BARCODE_NO_GA]].Value = (packingLine?.BarCode ?? string.Empty).Trim().Replace(';', '\n');

                    if (firstRow)
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = styleColorName;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_GA]].Value = styleColorCode;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_DESCRIPTON]].Value = styleColorDescription;
                    }

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]]
                                        .Value = _checkAssortedStyle ? packingLine.Quantity * totalStyle : packingLine.Quantity;
                    if (_checkAssortedStyle)
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.Quantity * totalStyle;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[POLYBAGS_CARTON]].Value = packingLine.PackagesPerBox;
                    }
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton * totalStyle;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                            = packingLine.QuantityPerCarton * packingLine.TotalCarton * totalStyle;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]].Value
                            = Math.Round(netWeight / (decimal)(packingLine.TotalCarton == 0 ? 1 : packingLine.TotalCarton), 2);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value
                            = Math.Round(grossWeight / (decimal)(packingLine.TotalCarton == 0 ? 1 : packingLine.TotalCarton), 2);
                    if (packingLine.BoxDimensionCode.Contains("*"))
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                    }
                    else
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                    }
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                            = Math.Round(netWeight, 2);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                            = Math.Round(grossWeight, 2);

                    decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.QuantitySize * totalStyle);

                    // add total quantity
                    var key = style.LSStyle + ";" + packingLine.Size;
                    if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                    {
                        quantity += totalQuantity;
                        _dicTotalQuantitySizeOverStyle[key] = quantity;
                    }
                    else
                    {
                        _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                    }

                    if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                    {
                        sizeQuantity += totalQuantity;
                        _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                    }
                    else
                    {
                        _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                    }
                    _totalUnits += totalQuantity;

                    // add total carton
                    if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                    {
                        cartonQty += (int)packingLine.TotalCarton;
                        _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                    }
                    else
                    {
                        _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                    }
                    _totalNoOfCarton += (int)packingLine.TotalCarton;
                    _totalNetWeight += (decimal)Math.Round(netWeight, 2);
                    _totalGrossWeight += (decimal)Math.Round(grossWeight, 2);
                    totalMeasM3 += Math.Round(measM3, 3);

                    //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                    //{
                    //    _isFitToPage = true;
                    //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                    //}
                    firstRow = false;
                    workSheet.Row(_maxRow).Height = 20;
                    _maxRow++;
                }
            }
            else if (_packingType == "AssortedSize_SolidColor")
            {
                foreach (var packingLine in sortPackingLines)
                {
                    if (firstRow || fromNo != packingLine.FromNo)
                    {
                        if (fromNo != packingLine.FromNo && !firstRow)
                        {
                            _maxRow++;
                        }

                        /// Convert packing unit
                        measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                            (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);

                        grossWeight = (decimal)(netWeight + packingLine.GrossWeight - packingLine.NetWeight);

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[BARCODE_NO_GA]].Value = (packingLine?.BarCode ?? string.Empty).Trim().Replace(';', '\n');
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = style.ColorName;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_GA]].Value = style.ColorCode;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_DESCRIPTON]].Value = style.Description;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.QuantityPerPackage;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[POLYBAGS_CARTON]].Value = packingLine.PackagesPerBox;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]].Value
                                = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value
                                = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                        if (packingLine.BoxDimensionCode.Contains("*"))
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                    .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                        }
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                                = Math.Round(netWeight, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                                = Math.Round(grossWeight, 2);

                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                        // add total quantity per style
                        var key = style.LSStyle + ";" + packingLine.Size;
                        if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                        {
                            quantity += totalQuantity;
                            _dicTotalQuantitySizeOverStyle[key] = quantity;
                        }
                        else
                        {
                            _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                        }
                        // add total quantity per size
                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;

                        // add total carton
                        if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                        {
                            cartonQty += (int)packingLine.TotalCarton;
                            _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                        }
                        else
                        {
                            _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                        }
                        _totalNoOfCarton += (int)packingLine.TotalCarton;
                        _totalNetWeight += (decimal)Math.Round(netWeight, 2);
                        _totalGrossWeight += (decimal)Math.Round(grossWeight, 2);
                        totalMeasM3 += Math.Round(measM3, 3);

                        firstRow = false;
                        fromNo = (int)packingLine.FromNo;
                        if (packingLine.TotalCarton < 100)
                        {
                            workSheet.Row(_maxRow).Height = 30;
                        }
                        else if (packingLine.TotalCarton < 300)
                        {
                            workSheet.Row(_maxRow).Height = 40;
                        }
                        else if (packingLine.TotalCarton < 500)
                        {
                            workSheet.Row(_maxRow).Height = 50;
                        }
                        else
                        {
                            workSheet.Row(_maxRow).Height = 60;
                        }
                    }
                    else
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                        // add total quantity per style
                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);
                        var key = style.LSStyle + ";" + packingLine.Size;
                        if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                        {
                            quantity += totalQuantity;
                            _dicTotalQuantitySizeOverStyle[key] = quantity;
                        }
                        else
                        {
                            _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                        }
                        // add total quantity per size
                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;
                        fromNo = (int)packingLine.FromNo;

                    }
                }
                _maxRow++;
            }
            else
            {
                sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo)
                                            .OrderByDescending(x => x.TotalCarton).ToList();
                summaryCarton = (int)sortPackingLines[0].TotalCarton;
                lsStyle = sortPackingLines[0].LSStyle;
                foreach (var packingLine in sortPackingLines)
                {
                    itemStyle = packinglist.ItemStyles.FirstOrDefault(x => x.LSStyle == packingLine.LSStyle);
                    if (packingLine.TotalCarton != summaryCarton)
                    {
                        firstRow = true;
                        lsStyle = packingLine.LSStyle;
                        _maxRow++;
                    }
                    if (firstRow || packingLine.LSStyle == lsStyle)
                    {
                        if (firstRow)
                        {
                            /// Convert packing unit
                            measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);

                            grossWeight = (decimal)(netWeight + packingLine.GrossWeight - packingLine.NetWeight);

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[BARCODE_NO_GA]].Value = (packingLine?.BarCode ?? string.Empty).Trim().Replace(';', '\n');
                            if (_checkAssortedStyle)
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = styleColorName;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_GA]].Value = styleColorCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_DESCRIPTON]].Value = styleColorDescription;
                            }
                            else
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_GA]].Value = itemStyle.ColorCode;
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_DESCRIPTON]].Value = itemStyle.Description;
                            }
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.QuantityPerPackage;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[POLYBAGS_CARTON]].Value = packingLine.PackagesPerBox;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                    = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]].Value
                                    = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value
                                    = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                            if (packingLine.BoxDimensionCode.Contains("*"))
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimensionCode.Split("*")[0].Trim();
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimensionCode.Split("*")[1].Trim();
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimensionCode.Split("*")[2].Trim();
                            }
                            else
                            {
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]]
                                        .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[0].Trim();
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]]
                                        .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[1].Trim();
                                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]]
                                        .Value = packingLine.BoxDimensionCode.Trim().ToLower().Split("x")[2].Trim();
                            }
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                                    = Math.Round(netWeight, 2);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                                    = Math.Round(grossWeight, 2);

                            decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                            // add total quantity per style
                            var key = itemStyle.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                            {
                                quantity += totalQuantity;
                                _dicTotalQuantitySizeOverStyle[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                            }
                            // add total quantity per size
                            if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                            {
                                sizeQuantity += totalQuantity;
                                _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                            }
                            _totalUnits += totalQuantity;

                            // add total carton
                            if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                            {
                                cartonQty += (int)packingLine.TotalCarton;
                                _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                            }
                            else
                            {
                                _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                            }
                            _totalNoOfCarton += (int)packingLine.TotalCarton;
                            _totalNetWeight += (decimal)Math.Round(netWeight, 2);
                            _totalGrossWeight += (decimal)Math.Round(grossWeight, 2);
                            totalMeasM3 += Math.Round(measM3, 3);

                            //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                            //{
                            //    _isFitToPage = true;
                            //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                            //}

                            mergeCell.Add(_maxRow);
                            firstRow = false;
                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                            // add total quantity per style
                            var key = itemStyle.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                            {
                                quantity += totalQuantity;
                                _dicTotalQuantitySizeOverStyle[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                            }
                            // add total quantity per size
                            if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                            {
                                sizeQuantity += totalQuantity;
                                _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                            }
                            _totalUnits += totalQuantity;
                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                            //workSheet.Row(_maxRow).Height = 30;
                            if (packingLine.TotalCarton < 100)
                            {
                                workSheet.Row(_maxRow).Height = 30;
                            }
                            else if (packingLine.TotalCarton < 300)
                            {
                                workSheet.Row(_maxRow).Height = 40;
                            }
                            else if (packingLine.TotalCarton < 500)
                            {
                                workSheet.Row(_maxRow).Height = 50;
                            }
                            else
                            {
                                workSheet.Row(_maxRow).Height = 60;
                            }
                        }
                    }
                    else
                    {
                        _maxRow++;

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = itemStyle.ColorName;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_GA]].Value = itemStyle.ColorCode;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_DESCRIPTON]].Value = itemStyle.Description;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_POLYBAG]].Value = packingLine.QuantityPerPackage;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[POLYBAGS_CARTON]].Value = packingLine.PackagesPerBox;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                                .Value = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                        // add total quantity per style
                        var key = itemStyle.LSStyle + ";" + packingLine.Size;
                        if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                        {
                            quantity += totalQuantity;
                            _dicTotalQuantitySizeOverStyle[key] = quantity;
                        }
                        else
                        {
                            _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                        }
                        // add total quantity per size
                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;

                        summaryCarton = (int)packingLine.TotalCarton;
                        lsStyle = packingLine.LSStyle;
                        workSheet.Row(_maxRow).Height = 20;
                    }
                }

                _maxRow++;
            }

            if (_packingType == "SolidSize_SolidColor" || _packingType == "SolidSize_AssortedColor" || _checkAssortedStyle)
            {
                workSheet.Cells["E" + styleRow + ":" + "E" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["F" + styleRow + ":" + "F" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["G" + styleRow + ":" + "G" + (_maxRow - 1)].Merge = true;
            }

            foreach (var merge in mergeCell)
            {
                workSheet.Cells["A" + merge + ":" + "A" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells["B" + merge + ":" + "B" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells["C" + merge + ":" + "C" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells["D" + merge + ":" + "D" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[NO_OF_CARTON],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[NO_OF_CARTON]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[NET_WEIGHT],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[NET_WEIGHT]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[GROSS_WEIGHT],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[LENGTH],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[LENGTH]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[WIDTH],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[WIDTH]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[HEIGHT],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[HEIGHT]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[MEAS_M3_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Merge = true;
            }

            // SET DATA TOTAL PACKING LINE
            workSheet.Cells[_maxRow, 1].Value = "TOTAL";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Rows[_maxRow].Height = 20;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[COLOR]].Merge = true;

            foreach (var size in _dicTotalQuantitySize)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[size.Key]].Value = size.Value;
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = _totalNoOfCarton;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalUnits;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = true;

            if (totalMeasM3 == 0)
            {
                totalMeasM3 = measM3;
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(totalMeasM3, 3);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value = _totalNetWeight;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value = _totalGrossWeight;

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
                    borderData.Style.WrapText = true;
                }

                /// BORDER FIRST TOTAL
                using (var borderData = workSheet.Cells[_maxRow, 1, _maxRow, 4])
                {
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Font.Bold = true;
                }

                /// BORDER SECOND TOTAL
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_GA]] + _maxRow + ":" +
                        _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]] + _maxRow;

                using (var borderData = workSheet.Cells[range])
                {
                    borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Font.Bold = true;
                    borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }
            }

            /// FORMAT NUMBER
            range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_UNITS]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            range = _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0.000";

            range = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0.00";

            /// SET UNIT FOR TOTAL
            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = "Cnts";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = packinglist?.ItemStyles?.FirstOrDefault()?.UnitID ?? "PCS";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value = "KGS";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value = "KGS";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                            _maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]]
                            .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
        }
        public void SummaryQuantitySize_Carton_GA(ExcelWorksheet workSheet, PackingList packingList, byte[] logoShipping)
        {
            string range = "";
            int column = 4;

            /// Quy cách phối hàng
            range = "D" + _maxRow + ":D" + (_maxRow + 6);
            workSheet.Cells[_maxRow, column].Value = _itemStyle.Packing;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[_maxRow, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[_maxRow, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[range].Merge = true;

            /// Insert logo 
            Image logo1 = LogoDownload(logoShipping);
            var pictureRight = workSheet.Drawings.AddPicture("shippingImage1", logo1);
            pictureRight.SetPosition((_maxRow + 7), 0, 0, 0);
            pictureRight.SetSize(70);

            _maxRow += 2;

            int columnSize = _dicPositionColumnPackingLine[COLOR];
            int columnCartonDimension = _dicPositionColumnPackingLine[CARTON_DIMENSION];
            int columnTotalUnit = 0;
            if ((_packingType == "SolidSize_SolidColor" || _packingType == "SolidSize_AssortedColor") && !_checkAssortedStyle)
                columnTotalUnit = _dicPositionColumnPackingLine[UNITS_CARTON];
            else
                columnTotalUnit = _dicPositionColumnPackingLine[UNITS_POLYBAG];

            workSheet.Cells[_maxRow, 5].Value = "SUMMARY";
            workSheet.Cells[_maxRow, 5].Style.Font.Bold = true;

            _maxRow++;
            column++;
            string rangeBoder = "E" + _maxRow + ":";
            string rangeCarton = _dicAlphabel[columnCartonDimension] + _maxRow + ":";

            range = "E" + _maxRow + ":E" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;

            column++;

            range = "F" + _maxRow + ":F" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR_CODE_GA;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;

            column++;

            range = "G" + _maxRow + ":G" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = "Status";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add("STATUS", column);

            column++;

            workSheet.Cells[_maxRow, column].Value = SIZE_GA;
            workSheet.Row(_maxRow).Height = 15;
            //var sizeList = _itemStyle.OrderDetails
            //        .Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList();

            //for (int i = 0; i < _countSize; i++)
            //{
            //    string size = sizeList[i].Size;
            //    if (!string.IsNullOrEmpty(size))
            //    {
            //        workSheet.Cells[_maxRow + 1, column].Value = size;
            //        column++;
            //    }
            //}

            foreach (var data in _dicSizeList
                .OrderBy(s => s.Key).ToList())
            {
                var size = data.Value;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    column++;
                }
            }

            workSheet.Row(_maxRow + 1).Height = 15;

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_GA]] + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))

            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
            }

            range = _dicAlphabel[columnCartonDimension] + _maxRow + ":" + _dicAlphabel[columnCartonDimension + 2] + _maxRow;
            workSheet.Cells[_maxRow, columnCartonDimension].Value = "CTNS SUMMARY";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;

            workSheet.Cells[_maxRow, columnCartonDimension + 3].Value = "QTY";

            int cartonRow = _maxRow + 1;
            _maxRow += 2;

            /// SUMMARY SIZE HANDLE
            int formatRow = _maxRow;
            var styles = packingList.ItemStyles.OrderBy(x => x.LSStyle).ToList();
            var firstRow = _maxRow;
            var styleRow = _maxRow;
            var summaryOrderDetails = new List<OrderDetail>();

            Dictionary<string, decimal> ordinalShipQuantity = new Dictionary<string, decimal>();

            foreach (var style in styles)
            {
                column = 5;
                styleRow = _maxRow;

                workSheet.Cells[_maxRow, column++].Value = style.ColorName;
                workSheet.Cells[_maxRow, column++].Value = style.ColorCode;

                workSheet.Cells[_maxRow, column++].Value = "ORDER";
                workSheet.Row(_maxRow).Height = 18;
                var orderDetails = style.OrderDetails
                    .Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList();
                orderDetails.OrderBy(x => x.SizeSortIndex).ToList().ForEach(x =>
                {
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]].Value = x.Quantity;
                    summaryOrderDetails.Add(x);
                });
                workSheet.Cells[_maxRow, columnTotalUnit].Value = style.TotalQuantity;
                if (_itemStyle.MultiShip == true)
                {
                    for (var i = 1; i < packingList.OrdinalShip; i++)
                    {
                        var oldPackingLists = style.PackingLists
                                .Where(x => x.OrdinalShip == i && (x.ParentPackingListID ?? 0) == 0)
                                .OrderByDescending(x => x.PackingListDate)?.FirstOrDefault();
                        var ordinalShip = i.ToString() + (i == 1 ? "st" : (i == 2 ? "nd" : (i == 3 ? "rd" : "th")));
                        _maxRow++;
                        column = 7;
                        workSheet.Cells[_maxRow, column++].Value = ordinalShip + " SHIP";
                        workSheet.Row(_maxRow).Height = 18;
                        var oldPackingLines = oldPackingLists.PackingLines
                            .Where(x => x.LSStyle == style.LSStyle).OrderBy(x => x.SequenceNo).ToList();
                        oldPackingLines.ForEach(x =>
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]].Value = x.QuantitySize * x.TotalCarton;
                            ordinalShipQuantity.Add(i + ";" + x.Size + ";" + x.LSStyle, (decimal)(x.QuantitySize * x.TotalCarton));
                        });
                        workSheet.Cells[_maxRow, columnTotalUnit].Value = oldPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                    }
                }

                _maxRow++;
                column = 7;
                workSheet.Cells[_maxRow, column++].Value = "SHIP";
                workSheet.Row(_maxRow).Height = 18;
                var packingLines = packingList.PackingLines
                    .Where(x => x.LSStyle == style.LSStyle).OrderBy(x => x.SequenceNo).ToList();
                packingLines.ForEach(x =>
                {
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]].Value = x.QuantitySize * x.TotalCarton;
                });
                workSheet.Cells[_maxRow, columnTotalUnit].Value = packingLines.Sum(x => x.QuantitySize * x.TotalCarton);

                _maxRow++;
                column = 7;
                workSheet.Cells[_maxRow, column++].Value = "BALANCE";
                workSheet.Row(_maxRow).Height = 18;
                decimal totalShip = 0;
                orderDetails.OrderBy(x => x.SizeSortIndex).ToList().ForEach(x =>
                {
                    decimal oldQuantity = 0;
                    foreach (var data in ordinalShipQuantity)
                    {
                        if (data.Key.Split(";")[1] == x.Size && data.Key.Split(";")[2] == style.LSStyle)
                        {
                            oldQuantity += data.Value;
                        }
                    }
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]]
                            .Value = packingLines.Where(y => y.Size == x.Size && y.LSStyle == style.LSStyle)
                                        .Sum(y => y.QuantitySize * y.TotalCarton) + oldQuantity - x.Quantity;
                    totalShip += oldQuantity;
                });
                workSheet.Cells[_maxRow, columnTotalUnit]
                    .Value = packingLines.Sum(x => x.QuantitySize * x.TotalCarton)
                             + totalShip - style.TotalQuantity;
                range = _dicAlphabel[columnSize + 3] + styleRow + ":" + _dicAlphabel[columnTotalUnit] + _maxRow;
                workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

                _maxRow++;
                column = 7;
                workSheet.Cells[_maxRow, column++].Value = "% SHORT SHIP";
                workSheet.Row(_maxRow).Height = 18;
                orderDetails.OrderBy(x => x.SizeSortIndex).ToList().ForEach(x =>
                {
                    decimal oldQuantity = 0;
                    foreach (var data in ordinalShipQuantity)
                    {
                        if (data.Key.Split(";")[1] == x.Size && data.Key.Split(";")[2] == style.LSStyle)
                        {
                            oldQuantity += data.Value;
                        }
                    }
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]]
                        .Value = (packingLines.Where(y => y.Size == x.Size && y.LSStyle == style.LSStyle)
                                    .Sum(y => y.QuantitySize * y.TotalCarton) + oldQuantity - x.Quantity) /
                                    (x.Quantity == 0 ? 1 : x.Quantity);

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]].Style.Numberformat.Format = "##0.00" + "%";
                });
                workSheet.Cells[_maxRow, columnTotalUnit]
                    .Value = (packingLines.Sum(x => x.QuantitySize * x.TotalCarton) + totalShip - style.TotalQuantity)
                                    / style.TotalQuantity;
                workSheet.Cells[_maxRow, columnTotalUnit].Style.Numberformat.Format = "##0.00" + "%";

                workSheet.Cells["E" + styleRow + ":E" + _maxRow].Merge = true;
                workSheet.Cells["F" + styleRow + ":F" + _maxRow].Merge = true;

                _maxRow++;
            }

            /// TOTAL 
            column = 5;
            styleRow = _maxRow;

            workSheet.Cells[_maxRow, column++].Value = "TOTAL";

            column += 1;
            workSheet.Cells[_maxRow, column++].Value = "ORDER";
            workSheet.Row(_maxRow).Height = 18;
            //_itemStyle.OrderDetails.Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList()
            summaryOrderDetails.ForEach(x =>
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]]
                    .Value = summaryOrderDetails.Where(y => y.Size == x.Size).Sum(x => x.Quantity);
            });
            workSheet.Cells[_maxRow, columnTotalUnit].Value = packingList.ItemStyles.Sum(x => x.TotalQuantity);

            if (_itemStyle.MultiShip == true)
            {
                for (var i = 1; i < packingList.OrdinalShip; i++)
                {
                    decimal totalOrdinalShip = 0;
                    var ordinalShip = i.ToString() + (i == 1 ? "st" : (i == 2 ? "nd" : (i == 3 ? "rd" : "th")));
                    _maxRow++;
                    column = 7;
                    workSheet.Cells[_maxRow, column++].Value = ordinalShip + " SHIP";
                    workSheet.Row(_maxRow).Height = 18;
                    //_itemStyle.OrderDetails.Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList()
                    summaryOrderDetails.ForEach(x =>
                    {
                        decimal oldQuantity = 0;
                        foreach (var data in ordinalShipQuantity)
                        {
                            if (data.Key.Split(";")[0] == i.ToString() && data.Key.Split(";")[1] == x.Size)
                            {
                                oldQuantity += data.Value;
                            }
                        }
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]].Value = oldQuantity;
                        totalOrdinalShip += oldQuantity;
                    });

                    workSheet.Cells[_maxRow, columnTotalUnit].Value = totalOrdinalShip;
                }
            }

            _maxRow++;
            column = 7;
            workSheet.Cells[_maxRow, column++].Value = "SHIP";
            workSheet.Row(_maxRow).Height = 18;
            //_itemStyle.OrderDetails.Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList()
            summaryOrderDetails.ForEach(x =>
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]]
                    .Value = packingList.PackingLines.Where(y => y.Size == x.Size).Sum(y => y.QuantitySize * y.TotalCarton);
            });
            workSheet.Cells[_maxRow, columnTotalUnit].Value = packingList.TotalQuantity;

            _maxRow++;
            column = 7;
            workSheet.Cells[_maxRow, column++].Value = "BALANCE";
            workSheet.Row(_maxRow).Height = 18;
            //_itemStyle.OrderDetails.Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList()
            summaryOrderDetails.ForEach(x =>
            {
                decimal oldQuantity = 0;
                foreach (var data in ordinalShipQuantity)
                {
                    if (data.Key.Split(";")[1] == x.Size)
                    {
                        oldQuantity += data.Value;
                    }
                }
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]]
                    .Value = packingList.PackingLines.Where(y => y.Size == x.Size)
                        .Sum(y => y.QuantitySize * y.TotalCarton) + oldQuantity -
                        summaryOrderDetails.Where(y => y.Size == x.Size).Sum(x => x.Quantity);
            });
            workSheet.Cells[_maxRow, columnTotalUnit]
                .Value = packingList.PackingLines.Sum(x => x.QuantitySize * x.TotalCarton)
                        + ordinalShipQuantity.Values.Sum()
                        - packingList.ItemStyles.Sum(x => x.TotalQuantity);
            range = _dicAlphabel[columnSize + 3] + styleRow + ":" + _dicAlphabel[columnTotalUnit] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            _maxRow++;
            column = 7;
            workSheet.Cells[_maxRow, column++].Value = "% SHORT SHIP";
            workSheet.Row(_maxRow).Height = 18;
            decimal summaryQuantity = 0;
            //_itemStyle.OrderDetails.Where(x => x.Quantity > 0).OrderBy(x => x.SizeSortIndex).ToList()
            summaryOrderDetails.ForEach(x =>
            {
                decimal oldQuantity = 0;
                foreach (var data in ordinalShipQuantity)
                {
                    if (data.Key.Split(";")[1] == x.Size)
                    {
                        oldQuantity += data.Value;
                    }
                }
                summaryQuantity = (decimal)summaryOrderDetails.Where(y => y.Size == x.Size).Sum(x => x.Quantity);
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]]
                    .Value = (packingList.PackingLines.Where(y => y.Size == x.Size)
                        .Sum(y => y.QuantitySize * y.TotalCarton) + oldQuantity -
                         summaryQuantity) / (summaryQuantity == 0 ? 1 : summaryQuantity);
                summaryOrderDetails.Where(y => y.Size == x.Size).Sum(x => x.Quantity);
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[x.Size]].Style.Numberformat.Format = "##0.00" + "%";
            });
            summaryQuantity = (decimal)packingList.ItemStyles.Sum(x => x.TotalQuantity);
            workSheet.Cells[_maxRow, columnTotalUnit]
                .Value = (packingList.PackingLines.Sum(x => x.QuantitySize * x.TotalCarton) + ordinalShipQuantity.Values.Sum() -
                          summaryQuantity) / (summaryQuantity == 0 ? 1 : summaryQuantity);
            workSheet.Cells[_maxRow, columnTotalUnit].Style.Numberformat.Format = "##0.00" + "%";

            workSheet.Cells["E" + styleRow + ":F" + _maxRow].Merge = true;

            range = _dicAlphabel[columnTotalUnit] + formatRow + ":" + _dicAlphabel[columnTotalUnit] + _maxRow;
            workSheet.Cells[range].Style.Font.Bold = true;

            ///// FORMAT BORDER

            range = _dicAlphabel[columnSize] + (formatRow - 2) + ":" + _dicAlphabel[columnTotalUnit] + _maxRow;

            using (var borderData = workSheet.Cells[range])
            {
                borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                borderData.Style.WrapText = true;
            }

            /// SUMMARY CARTON
            decimal totaCarton = 0;
            foreach (var carton in _dicTotalCartons)
            {
                workSheet.Cells[cartonRow, columnCartonDimension].Value = carton.Key;
                workSheet.Cells[cartonRow, columnCartonDimension, cartonRow, columnCartonDimension + 2].Merge = true;
                workSheet.Cells[cartonRow, columnCartonDimension + 3].Value = (decimal)carton.Value;
                workSheet.Cells[cartonRow, columnCartonDimension + 3].Style.Numberformat.Format = "#,##0";
                totaCarton += (decimal)carton.Value;

                cartonRow++;
            }

            workSheet.Cells[cartonRow, columnCartonDimension, cartonRow, columnCartonDimension + 2].Merge = true;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Value = totaCarton;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Style.Font.Bold = true;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Style.Numberformat.Format = "#,##0";

            rangeCarton += _dicAlphabel[columnCartonDimension + 3] + (cartonRow - 1);
            using (var cartonData = workSheet.Cells[rangeCarton])
            {
                cartonData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cartonData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cartonData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cartonData.Style.WrapText = true;
            }

            _maxRow += 3;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                .Value = packingList.Company != null ? packingList.Company.Name.Trim().ToUpper()
                         : "LEADING STAR VIETNAM GARMENT COMPANY LIMITED";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Italic = true;
        }
        #endregion

        #region HADDAD
        private Stream CreateExcelFile_HA(List<PackingList> packingLists, Stream stream = null)
        {
            string Author = "Leading Star VN";
            var indexSheet = 0;
            var excelTitle = "";

            if (!String.IsNullOrEmpty(packingLists?.First().CreatedBy))
            {
                Author = packingLists?.First().CreatedBy;
            }

            //string Title = string.IsNullOrEmpty(packingList.OrdinalShip.ToString()) ? "Sheet" : packingList.OrdinalShip.ToString();

            var sortThumbnail = packingLists?.First().PackingListImageThumbnails.OrderBy(x => x.SortIndex).ToList();

            byte[] shippingImage1 = null;

            if (sortThumbnail.Count > 0)
            {
                shippingImage1 = sortThumbnail[0].ImageData;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;
                
                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Comments = "Packing List of Leading Star VN";
                excelPackage.Workbook.Properties.Title = _LSStyle;
                foreach (var packingList in packingLists)
                {
                    ResetData();
                    _itemStyle = packingList.ItemStyles?.FirstOrDefault();
                    string Title = _itemStyle.LSStyle;
                    _countSize = _itemStyle?.OrderDetails.ToList().Count() ?? 0;

                    excelPackage.Workbook.Worksheets.Add(Title);
                    var workSheet = excelPackage.Workbook.Worksheets[indexSheet];

                    workSheet.Cells.Style.Font.SetFromFont(new Font("Arial", 8));

                    CreateHeaderPage_HA(workSheet, packingList);
                    CreateHeaderPackingLine_HA(workSheet, packingList);
                    FillDataPackingLine_HA(workSheet, packingList);
                    SummaryQuantitySize_Carton_HA(workSheet, packingList, shippingImage1);

                    string modelRangeBorder = "A1:"
                                            + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]]
                                            + (_maxRow + 9);

                    using (var range = workSheet.Cells[modelRangeBorder])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }

                    if (_dicAlphabel.TryGetValue(_maxColumn, out string characterColumnPacking))
                    {
                        using (var range = workSheet.Cells["A1:" + characterColumnPacking + "1"])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont(new Font("Arial", 14));
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }
                    workSheet.PrinterSettings.FitToPage = true;
                    workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    workSheet.PrinterSettings.VerticalCentered = true;
                    workSheet.PrinterSettings.HorizontalCentered = true;

                    indexSheet++;
                }

                
                //if (!_isFitToPage)
                //{
                //    workSheet.PrinterSettings.FitToPage = true;
                //}
                
                excelPackage.Save();
                return excelPackage.Stream;
            }

        }
        public void CreateHeaderPage_HA(ExcelWorksheet workSheet, PackingList packingList)
        {
            workSheet.Cells[1, 1].Value = "PACKING LIST";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);

            int posCustomer = 6 + _countSize;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "JOB NO:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].EntireColumn.Width = 6;
            workSheet.Cells[_maxRow, 2].EntireColumn.Width = 2;
            workSheet.Cells[_maxRow, 3].EntireColumn.Width = 6;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Value = packingList.LSStyles.Replace(";", "+");
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Label";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.LabelCode;
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "STYLE NO:";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.CustomerStyle;
            workSheet.Cells[_maxRow, posCustomer].Value = "DATE:";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "STYLE DESCRIPTION";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.ProductionDescription;
            workSheet.Cells[_maxRow, posCustomer].Value = "CST No:";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = "";

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "ORDER TYPE";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle?.PurchaseOrderType?.Name;
            workSheet.Cells[_maxRow, posCustomer].Value = "ORDER NO:";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = _itemStyle?.PurchaseOrderNumber;
            workSheet.Cells[_maxRow, posCustomer + 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, posCustomer].Value = "CANADA PO#";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = packingList?.BrandPurchaseOrder ?? string.Empty;
            workSheet.Cells[_maxRow, posCustomer + 2].Style.Font.Bold = true;

            _maxRow++;
            var hangFlatColumn = 6 + (int)((_countSize - 1) / 2);
            workSheet.Cells[_maxRow, hangFlatColumn].Value = _itemStyle?.HangFlat;
            workSheet.Cells[_maxRow, hangFlatColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, hangFlatColumn + 1].Value = _itemStyle?.Packing;
        }
        public void CreateHeaderPackingLine_HA(ExcelWorksheet workSheet, PackingList packinglist)
        {
            int column = 1;
            _maxRow++;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            string prepack = packinglist.PackingLines?.FirstOrDefault().PrePack;
            _length_Unit_IFG = packinglist?.PackingUnit?.LengthUnit ?? "CM";
            _weight_Unit_IFG = packinglist?.PackingUnit?.WeightUnit ?? "KGS";

            string range = "A" + _maxRow + ":C" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = CARTON_NUMBER;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(FROM_NO, 1); // add position column 
            _dicPositionColumnPackingLine.Add(TO_NO, 3);

            column += 3;
            range = "D" + _maxRow + ":D" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR_FASHION;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Column(column).Width = 18;
            _dicPositionColumnPackingLine.Add(COLOR_FASHION, column);

            column++;
            range = "E" + _maxRow + ":E" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = COLOR_HA;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Column(column).Width = 10;
            _dicPositionColumnPackingLine.Add(COLOR_HA, column);

            column++;
            workSheet.Cells[_maxRow, column].Value = SIZE_IFG;
            _dicPositionColumnPackingLine.Add(SIZE_IFG, column);
            var sizeList = _itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;
                    _dicPositionColumnPackingLine.Add(size, column);

                    if (size.Length > 7)
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 8;
                    }
                    else if (size.Length > 5)
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 7;
                    }
                    else
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 5;
                    }
                    column++;
                }
            }

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_dicAlphabel.TryGetValue(column, out string unitsCharacter))
            {
                range = unitsCharacter + _maxRow + ":" + unitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = UNITS_CARTON;
                workSheet.Cells[range].Merge = true;
                workSheet.Cells[range].Style.WrapText = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(UNITS_CARTON, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string noOfCharacter))
            {
                range = noOfCharacter + _maxRow + ":" + noOfCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NO_OF_CARTON;
                _dicPositionColumnPackingLine.Add(NO_OF_CARTON, column);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;

            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))
            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string netWeightCharacter))
            {
                range = netWeightCharacter + _maxRow + ":" + netWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NET_WEIGHT_IFG + " (" + _weight_Unit_IFG + ")";
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(NET_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string grossWeightCharacter))
            {
                range = grossWeightCharacter + _maxRow + ":" + grossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = GROSS_WEIGHT_IFG + " (" + _weight_Unit_IFG + ")"; ;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(GROSS_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string cartonDimensionCharacter))
            {
                workSheet.Cells[_maxRow, column].Value = CARTON_DIMENSION_IFG + " (" + _length_Unit_IFG + ")"; ;
                _dicPositionColumnPackingLine.Add(CARTON_DIMENSION_IFG, column);

                _dicPositionColumnPackingLine.Add(LENGTH, column);
                workSheet.Column(column).Width = 4;

                column++;
                _dicPositionColumnPackingLine.Add(WIDTH, column);
                workSheet.Column(column).Width = 4;

                column++;
                _dicPositionColumnPackingLine.Add(HEIGHT, column);
                workSheet.Column(column).Width = 4;

                if (_dicAlphabel.TryGetValue(column, out string endCartonDimensionCharacter))
                {
                    range = cartonDimensionCharacter + _maxRow + ":" + endCartonDimensionCharacter + (_maxRow + 1);
                    workSheet.Cells[range].Merge = true;
                    workSheet.Cells[range].Style.WrapText = true;
                }
            }

            column++;
            if (_dicAlphabel.TryGetValue(column, out string measCharacter))
            {
                range = measCharacter + _maxRow + ":" + measCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = MEAS_M3_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(MEAS_M3_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalNetWeightCharacter))
            {
                range = totalNetWeightCharacter + _maxRow + ":" + totalNetWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_NET_WEIGHT_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 8;
                _dicPositionColumnPackingLine.Add(TOTAL_NET_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalGrossWeightCharacter))
            {
                range = totalGrossWeightCharacter + _maxRow + ":" + totalGrossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_GROSS_WEIGHT_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_GROSS_WEIGHT_IFG, column);

                string rangeBorder = "A10:" + totalGrossWeightCharacter + (_maxRow + 1);

                // border header packing line
                workSheet.Cells[rangeBorder].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[rangeBorder].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(column).Width = 8;
            }

            workSheet.Row(_maxRow).Height = 18;
            workSheet.Row(_maxRow + 1).Height = 18;
            _maxColumn = column;
            _maxRow += 2;
        }
        public void FillDataPackingLine_HA(ExcelWorksheet workSheet, PackingList packinglist)
        {
            string range = "A" + _maxRow + ":";
            decimal totalMeasM3 = 0;
            decimal netWeight = 0;
            decimal grossWeight = 0;
            decimal measM3 = 0;
            bool firstRow = true;
            int fromNo = 0;
            int styleRow = 0;
            bool isSolidSize = false;
            var inchConvertValue = 1728 * 35.31;
            var lbConvertValue = 2.2046;

            _dicTotalQuantitySize = new Dictionary<string, decimal>();
            styleRow = _maxRow;

            var style = packinglist.ItemStyles.FirstOrDefault();
            var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo).ToList();

            // Fill Data 
            foreach (var packingLine in sortPackingLines)
            {
                if (packingLine.PrePack == "Assorted Size - Solid Color")
                {
                    if (firstRow || fromNo != packingLine.FromNo)
                    {
                        if (fromNo != packingLine.FromNo && !firstRow)
                        {
                            _maxRow++;
                        }

                        /// Convert packing unit
                        if (_length_Unit_IFG.Trim().ToUpper() == "INCH")
                        {
                            measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                        }
                        else
                        {
                            measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                            packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                        }

                        if (_weight_Unit_IFG.Trim().ToUpper() == "LBS")
                        {
                            netWeight = (decimal)packingLine.NetWeight * (decimal)lbConvertValue;
                            grossWeight = (decimal)packingLine.GrossWeight;// * (decimal)lbConvertValue;
                        }
                        else
                        {
                            netWeight = (decimal)packingLine.NetWeight;
                            grossWeight = (decimal)packingLine.GrossWeight;
                        }

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_FASHION]].Value = style?.ColorName;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_HA]].Value = style?.ColorCode;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Value
                                = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG]].Value
                                = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.Length;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.Width;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.Height;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                                = Math.Round(netWeight, 2);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                                = Math.Round(grossWeight, 2);

                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.QuantitySize);

                        // add total quantity per size
                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;

                        // add total carton
                        if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                        {
                            cartonQty += (int)packingLine.TotalCarton;
                            _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                        }
                        else
                        {
                            _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                        }
                        _totalNoOfCarton += (int)packingLine.TotalCarton;
                        _totalNetWeight += (decimal)Math.Round(netWeight, 3);
                        _totalGrossWeight += (decimal)Math.Round(grossWeight, 3);
                        totalMeasM3 += Math.Round(measM3, 3);

                        //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                        //{
                        //    _isFitToPage = true;
                        //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                        //}

                        firstRow = false;
                        fromNo = (int)packingLine.FromNo;
                        workSheet.Row(_maxRow).Height = 20;
                    }
                    else
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;

                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.QuantitySize);
                        // add total quantity per size
                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;
                        fromNo = (int)packingLine.FromNo;
                    }
                }
                else
                {
                    /// Convert packing unit
                    if (_length_Unit_IFG.Trim().ToUpper() == "INCH")
                    {
                        measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                    }
                    else
                    {
                        measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                        packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                    }

                    if (_weight_Unit_IFG.Trim().ToUpper() == "LBS")
                    {
                        netWeight = (decimal)packingLine.NetWeight * (decimal)lbConvertValue;
                        grossWeight = (decimal)packingLine.GrossWeight * (decimal)lbConvertValue;
                    }
                    else
                    {
                        netWeight = (decimal)packingLine.NetWeight;
                        grossWeight = (decimal)packingLine.GrossWeight;
                    }

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_FASHION]].Value = style?.ColorName;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_HA]].Value = style?.ColorCode;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                            = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Value
                            = Math.Round(netWeight / (decimal)(packingLine.TotalCarton > 0 ? packingLine.TotalCarton : 1), 2);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG]].Value
                            = Math.Round(grossWeight / (decimal)(packingLine.TotalCarton > 0 ? packingLine.TotalCarton : 1), 2);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.Length;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.Width;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.Height;
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                            = Math.Round(netWeight, 2);
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                            = Math.Round(grossWeight, 2);

                    decimal totalQuantity = (decimal)(packingLine.QuantityPerCarton * packingLine.TotalCarton);

                    // add total quantity

                    if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                    {
                        sizeQuantity += totalQuantity;
                        _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                    }
                    else
                    {
                        _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                    }
                    _totalUnits += totalQuantity;

                    // add total carton
                    if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                    {
                        cartonQty += (int)packingLine.TotalCarton;
                        _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                    }
                    else
                    {
                        _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                    }
                    _totalNoOfCarton += (int)packingLine.TotalCarton;
                    _totalNetWeight += (decimal)Math.Round(netWeight, 3);
                    _totalGrossWeight += (decimal)Math.Round(grossWeight, 3);
                    totalMeasM3 += Math.Round(measM3, 3);

                    //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                    //{
                    //    _isFitToPage = true;
                    //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                    //}
                    isSolidSize = true;

                    workSheet.Row(_maxRow).Height = 20;
                    _maxRow++;
                }
            }

            _maxRow++;

            if (isSolidSize)
            {
                _maxRow--;
            }

            // SET DATA TOTAL PACKING LINE
            workSheet.Cells[_maxRow, 1].Value = "TOTAL";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Rows[_maxRow].Height = 20;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[COLOR_HA]].Merge = true;

            foreach (var size in _dicTotalQuantitySize)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[size.Key]].Value = size.Value;
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = _totalNoOfCarton;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalUnits;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = true;

            if (totalMeasM3 == 0)
            {
                totalMeasM3 = measM3;
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(totalMeasM3, 3);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value = _totalNetWeight;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value = _totalGrossWeight;

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
                    borderData.Style.WrapText = true;
                }

                /// BORDER FIRST TOTAL
                using (var borderData = workSheet.Cells[_maxRow, 1, _maxRow, 4])
                {
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Font.Bold = true;
                }

                /// BORDER SECOND TOTAL
                range = _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow + ":" +
                        _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]] + _maxRow;

                using (var borderData = workSheet.Cells[range])
                {
                    borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Font.Bold = true;
                    borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }
            }

            /// FORMAT NUMBER
            range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_UNITS]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            range = _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0.000";

            range = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0.00";

            /// SET UNIT FOR TOTAL
            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = "Cnts";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = packinglist?.ItemStyles?.FirstOrDefault()?.UnitID ?? "PCS";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value = _weight_Unit_IFG;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value = _weight_Unit_IFG;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                            _maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]]
                            .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;

        }
        public void SummaryQuantitySize_Carton_HA(ExcelWorksheet workSheet, PackingList packingList, byte[] logoShipping)
        {
            string range = "";
            int column = 4;
            int columnSize = _dicPositionColumnPackingLine[SIZE_IFG] + _countSize - 1;
            int columnTotalUnit = _dicPositionColumnPackingLine[UNITS_CARTON];

            workSheet.Cells[_maxRow, 4].Value = "SUMMARY:";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;

            string rangeBoder = "D" + _maxRow + ":";

            range = "D" + _maxRow + ":D" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = "Color";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;

            column++;
            range = "E" + _maxRow + ":E" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = "Color code";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;

            column++;
            workSheet.Cells[_maxRow, column].Value = SIZE_IFG;
            var sizeList = _itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size))
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    column++;
                }
            }

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))

            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
            }

            _maxRow += 2;
            column = 4;
            decimal totalUnit = 0;
            workSheet.Cells[_maxRow, column++].Value = _itemStyle.ColorName;
            workSheet.Cells[_maxRow, column++].Value = _itemStyle.ColorCode;
            foreach (var data in _dicTotalQuantitySize)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[data.Key]].Value = data.Value;
                totalUnit += (decimal)data.Value;
            }
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = totalUnit;
            workSheet.Row(_maxRow).Height = 20;

            /// FORMAT NUMBER
            range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + _maxRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[UNITS_CARTON]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            rangeBoder += _dicAlphabel[columnTotalUnit] + _maxRow;
            using (var borderData = workSheet.Cells[rangeBoder])
            {
                borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                borderData.Style.WrapText = true;
            }

            _maxRow += 2;
            workSheet.Cells[_maxRow, columnTotalUnit].Value = "TOTAL NW:";
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Value = _totalNetWeight;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, columnTotalUnit + 3].Value = _weight_Unit_IFG;
            workSheet.Cells[_maxRow, columnTotalUnit + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow++;
            workSheet.Cells[_maxRow, columnTotalUnit].Value = "TOTAL GW:";
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Value = _totalGrossWeight;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Style.Numberformat.Format = "#,##0.00";
            workSheet.Cells[_maxRow, columnTotalUnit + 3].Value = _weight_Unit_IFG;
            workSheet.Cells[_maxRow, columnTotalUnit + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow += 2;
            /// Insert logo 
            Image logo1 = LogoDownload(logoShipping);
            var pictureRight = workSheet.Drawings.AddPicture("shippingImage1", logo1);
            pictureRight.SetPosition(_maxRow, 0, 2, 0);

            string rangeCarton = _dicAlphabel[columnTotalUnit] + _maxRow + ":";
            range = _dicAlphabel[columnTotalUnit] + _maxRow + ":" + _dicAlphabel[columnTotalUnit + 1] + _maxRow;
            workSheet.Cells[_maxRow, columnTotalUnit].Value = "CTNS SUMMARY";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Value = "QTY";

            _maxRow++;
            decimal totalCarton = 0;
            foreach (var carton in _dicTotalCartons)
            {
                workSheet.Cells[_maxRow, columnTotalUnit].Value = carton.Key;
                workSheet.Cells[_maxRow, columnTotalUnit, _maxRow, columnTotalUnit + 1].Merge = true;
                workSheet.Cells[_maxRow, columnTotalUnit + 2].Value = (decimal)carton.Value;
                totalCarton += (decimal)carton.Value;

                _maxRow++;
            }

            workSheet.Cells[_maxRow, columnTotalUnit, _maxRow, columnTotalUnit + 1].Merge = true;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Value = totalCarton;
            workSheet.Cells[_maxRow, columnTotalUnit + 2].Style.Font.Bold = true;

            rangeCarton += _dicAlphabel[columnTotalUnit + 2] + _maxRow;
            using (var cartonData = workSheet.Cells[rangeCarton])
            {
                cartonData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cartonData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cartonData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cartonData.Style.WrapText = true;
            }

            _maxRow += 2;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]]
                .Value = packingList.Company != null ? packingList.Company.Name.Trim().ToUpper()
                         : "LEADING STAR VIETNAM GARMENT COMPANY LIMITED";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Style.Font.Italic = true;
        }
        #endregion

        #region DECATHLON
        private Stream CreateExcelFile_DE(List<PackingList> packingLists, Stream stream = null)
        {
            //DataTable table = SetData(packingList);
            var errorMessage = string.Empty;
            string Author = "Leading Star VN";
            int indexSheet = 0;

            if (!String.IsNullOrEmpty(packingLists?.First().CreatedBy))
            {
                Author = packingLists?.First().CreatedBy;
            }

            var sortThumbnail = packingLists?.First().PackingListImageThumbnails.OrderBy(x => x.SortIndex).ToList();

            byte[] shippingImage1 = null;

            if (sortThumbnail.Count > 0)
            {
                shippingImage1 = sortThumbnail[0].ImageData;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = _LSStyle;
                excelPackage.Workbook.Properties.Comments = "Packing List of Leading Start VN";

                foreach (var packingList in packingLists)
                {
                    string Title = "";
                    if(_multiShip)
                    {
                        Title = packingList.SheetName.SheetName;
                    }
                    else
                    {
                        Title = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle + "-" +
                                ((packingList.SheetNameID ?? 0) != 0 ? packingList?.SheetName?.SheetName : "Total");
                    }
                    
                    excelPackage.Workbook.Worksheets.Add(Title);
                    ResetData();

                    _itemStyle = packingList.ItemStyles?.FirstOrDefault();
                    _countSize = _itemStyle.OrderDetails.Count();

                    var setCutting = ObjectSpace.GetObjects<DailyTarget>(
                           CriteriaOperator.Parse(" [LSStyle] = ? AND [Operation] = ?", _itemStyle.LSStyle, "CUTTING"))?.FirstOrDefault();

                    var allocDailyOutputs = ObjectSpace.GetObjects<AllocDailyOutput>(
                        CriteriaOperator.Parse(" [LSStyle] = ? AND [FabricContrastName] = ?", _itemStyle.LSStyle, "A"))?.ToList();

                    var filter = "";
                    if (allocDailyOutputs.Any())
                    {
                        filter = " AND [AllocDailyOutputID] " +
                            " IN (" + string.Join(",", allocDailyOutputs?.Select(x => x.ID)) + ")";
                    }

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

                    var workSheet = excelPackage.Workbook.Worksheets[indexSheet];
                    workSheet.Cells.Style.Font.SetFromFont(new Font("Arial", 10));

                    CreateHeaderPage_DE(workSheet, shippingImage1);
                    CreateHeaderPacking_DE(workSheet);
                    FillDataPackingLine_DE(workSheet, packingList);
                    TotalCarton_DE(workSheet, cuttingLots);

                    var character = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]];
                    foreach (var odd in _oddCarton)
                    {
                        var oddRange = "A" + odd + ":" + character + odd;
                        using (var range = workSheet.Cells[oddRange])
                        {
                            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                        }
                    }

                    decimal marginPage = (decimal)0.2;
                    workSheet.PrinterSettings.FitToPage = true;
                    workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    workSheet.PrinterSettings.TopMargin = marginPage;
                    workSheet.PrinterSettings.LeftMargin = marginPage;
                    workSheet.PrinterSettings.RightMargin = marginPage;
                    workSheet.PrinterSettings.BottomMargin = marginPage;
                    workSheet.PrinterSettings.VerticalCentered = true;
                    workSheet.PrinterSettings.HorizontalCentered = true;

                    indexSheet++;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }

        public void CreateHeaderPage_DE(ExcelWorksheet workSheet, byte[] logoImage)
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

        public void CreateHeaderPacking_DE(ExcelWorksheet workSheet)
        {
            int column = 1;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

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
                    _dicPositionColumnPackingLine.Add(size.Trim().Replace(" ", "").ToUpper(), column);

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

        public void FillDataPackingLine_DE(ExcelWorksheet workSheet, PackingList packingList)
        {
            _oddCarton = new List<int>();
            _dicTotalQuantitySize = new Dictionary<string, decimal>();
            // load color garment code - item barcode

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]]
                             .Value = _itemStyle.ColorCode;

            foreach (var itemBarcode in _itemStyle.Barcodes)
            {
                if (itemBarcode.Size.Trim().Replace(" ", "").ToUpper().Count() > itemBarcode.BarCode.Count())
                {
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Trim().Replace(" ", "").ToUpper()]]
                                        .Value = itemBarcode.BarCode;
                    workSheet.Row(_maxRow).Height = 24;
                }
                else
                {
                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Trim().Replace(" ", "").ToUpper()]]
                                        .Value = itemBarcode.BarCode;

                    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Trim().Replace(" ", "").ToUpper()]]
                        .EntireColumn.AutoFit();
                    workSheet.Row(_maxRow).Height = 24;
                }

            }

            _maxRow++;
            _rowSum = _maxRow;
            /// fill data size
            foreach (var packingLine in packingList.PackingLines
                .Where(x => x.TotalQuantity > 0).OrderBy(x => x.SequenceNo))
            {
                var measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                          packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                var netWeight = (decimal)packingLine.NetWeight;
                var grossWeight = (decimal)(netWeight + packingLine.GrossWeight - packingLine.NetWeight);

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR]].Value = _itemStyle?.Description;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size.Trim().Replace(" ", "").ToUpper()]]
                                        .Value = packingLine.Quantity;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = packingLine.TotalQuantity;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT]].Value
                            = Math.Round(netWeight / (decimal)(packingLine.TotalCarton == 0 ? 1 : packingLine.TotalCarton), 3);
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value
                        = Math.Round(grossWeight / (decimal)(packingLine.TotalCarton == 0 ? 1 : packingLine.TotalCarton), 3);
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LENGTH]].Value = packingLine.BoxDimension.Length;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH]].Value = packingLine.BoxDimension.Width;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT]].Value = packingLine.BoxDimension.Height;
                //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Value = Math.Round(measM3, 3);
                //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]].Value = Math.Round(netWeight, 3);
                //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]].Value = Math.Round(grossWeight, 3);
                var measM3Formla = "ROUND("
                            + _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow + "*"
                            + _dicAlphabel[_dicPositionColumnPackingLine[LENGTH]] + _maxRow + "*"
                            + _dicAlphabel[_dicPositionColumnPackingLine[WIDTH]] + _maxRow + "*"
                            + _dicAlphabel[_dicPositionColumnPackingLine[HEIGHT]] + _maxRow + "/ 1000000, 3)";

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Formula = measM3Formla;

                var netFormula = _dicAlphabel[_dicPositionColumnPackingLine[NET_WEIGHT]] + _maxRow + "*"
                                            + _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow;

                var grossFormula = _dicAlphabel[_dicPositionColumnPackingLine[GROSS_WEIGHT]] + _maxRow + "*"
                                        + _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow;

                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                        .Formula = netFormula;
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                        .Formula = grossFormula;

                _totalNoOfCarton += (decimal)packingLine.TotalCarton;
                _totalUnits += (decimal)packingLine.TotalQuantity;
                _totalNetWeight += (decimal)netWeight;
                _totalGrossWeight += (decimal)packingLine.GrossWeight;
                _totalGrossWeightCarton += grossWeight / (decimal)packingLine.TotalCarton;
                _totalMeanM3 += measM3;

                /// add total quantity size
                if (_dicTotalQuantitySize.TryGetValue(packingLine.Size.Trim().Replace(" ", "").ToUpper(), out decimal sizeQuantity))
                {
                    sizeQuantity += (decimal)packingLine.TotalQuantity;
                    _dicTotalQuantitySize[packingLine.Size.Trim().Replace(" ", "").ToUpper()] = sizeQuantity;
                }
                else
                {
                    _dicTotalQuantitySize[packingLine.Size.Trim().Replace(" ", "").ToUpper()] = (decimal)packingLine.TotalQuantity;
                }

                /// add total carton
                if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                {
                    cartonQty += (int)packingLine.TotalCarton;
                    _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                }
                else
                {
                    _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                }

                if (packingLine.PrePack == "R")
                {
                    _oddCarton.Add(_maxRow);
                }

                workSheet.Row(_maxRow).Height = 24;
                _maxRow++;
            }


            // set total quantity of size
            //foreach (var itemBarcode in _itemStyle.Barcodes)
            //{
            //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Replace(" ", "").ToUpper()]]
            //             .Value = itemBarcode.Quantity;
            //}
            foreach (var quantitySize in _dicTotalQuantitySize)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[quantitySize.Key.Trim().Replace(" ", "").ToUpper()]]
                         .Value = quantitySize.Value;
                workSheet.Row(_maxRow).Height = 24;
            }

            _maxRow++;

            // set UE
            foreach (var itemBarcode in _itemStyle.Barcodes)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[itemBarcode.Size.Trim().Replace(" ", "").ToUpper()]]
                         .Value = "UE = " + itemBarcode.UE;
                workSheet.Row(_maxRow).Style.Font.Bold = true;
                workSheet.Row(_maxRow).Height = 24;
            }

            // set border 
            // STYLE BORDER, FIT COLUMN
            if (_dicAlphabel.TryGetValue(_maxColumn, out string maxColumnCharacter))
            {
                var range = "A9:" + maxColumnCharacter + (_maxRow);
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

        public void TotalCarton_DE(ExcelWorksheet workSheet, IList<CuttingLot> cuttingLots)
        {
            _maxRow++;
            var range = "";
            string cellTotalNW = "";
            string cellTotalGW = "";

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

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Value = _totalGrossWeightCarton;
            var totGrossFormula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[GROSS_WEIGHT]] + _rowSum + ":"
                                + _dicAlphabel[_dicPositionColumnPackingLine[GROSS_WEIGHT]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]].Formula = totGrossFormula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Value = _totalMeanM3;
            var totMeasM3Formula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3]] + _rowSum + ":"
                              + _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]].Formula = totMeasM3Formula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]].Value = _totalNetWeight;
            var totTotalNetFormula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]] + _rowSum + ":"
                               + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]].Formula = totTotalNetFormula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cellTotalNW = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_NET_WEIGHT]] + _maxRow;

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]].Value = _totalGrossWeight;
            var totTotalGrossFormula = "SUM(" + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]] + _rowSum + ":"
                               + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]] + (_maxRow - 4) + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]].Formula = totTotalGrossFormula;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                     .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]]
                     .Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cellTotalGW = _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT]] + _maxRow;
            ///// BORDER
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

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalNetWeight;
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

            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalGrossWeight;
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
                cell.Style.Font.SetFromFont(new Font("Arial", 9));
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


            foreach (var itemSize in _itemStyle.OrderDetails)
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

            if (cuttingLots.Count > 0)
            {
                var lotDetails = new Dictionary<string, decimal>();
                foreach (var lot in cuttingLots.OrderBy(x => x.Lot).ToList())
                {
                    var key = lot.Lot + ";" + lot.Size.Replace(" ", "").ToUpper();
                    if (!lotDetails.ContainsKey(key))
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
        #endregion

        #region IFG
        private Stream CreateExcelFile_IFG(List<PackingList> packingLists, Stream stream = null)
        {
            string Author = "Leading Star VN";
            int indexSheet = 0;
            if (!String.IsNullOrEmpty(packingLists?.First().CreatedBy))
            {
                Author = packingLists?.First().CreatedBy;
            }

            //string Title = string.IsNullOrEmpty(packingList.OrdinalShip.ToString()) ? "Sheet" : packingList.OrdinalShip.ToString();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = _LSStyle;
                excelPackage.Workbook.Properties.Comments = "Packing List of Leading Start VN";

                foreach (var packingList in packingLists)
                {
                    string Title = "";
                    if (_multiShip)
                    {
                        Title = packingList.SheetName.SheetName;
                    }
                    else
                    {
                        Title = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle + "-" +
                                ((packingList.SheetNameID ?? 0) != 0 ? packingList?.SheetName?.SheetName : "Total");
                    }
                    excelPackage.Workbook.Worksheets.Add(Title);
                    ResetData();

                    //CheckAssortedSize_AssortedColor(packingList);
                    _itemStyle = packingList.ItemStyles?.FirstOrDefault();
                    _countSize = _itemStyle.OrderDetails.Count();

                    var workSheet = excelPackage.Workbook.Worksheets[indexSheet];
                    workSheet.Cells.Style.Font.SetFromFont(new Font("Arial", 8));

                    CreateHeaderPage_IFG(workSheet, packingList);
                    CreateHeaderPackingLine_IFG(workSheet, packingList);
                    FillDataPackingLine_IFG(workSheet, packingList);
                    SummaryQuantitySize_Carton_IFG(workSheet, packingList);
                    ShippingMark_IFG(workSheet, packingList);

                    string modelRangeBorder = "A1:"
                                            + _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]]
                                            + (_maxRow + 9);

                    using (var range = workSheet.Cells[modelRangeBorder])
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.White);
                    }

                    if (_dicAlphabel.TryGetValue(_maxColumn, out string characterColumnPacking))
                    {
                        using (var range = workSheet.Cells["A1:" + characterColumnPacking + "1"])
                        {
                            range.Merge = true;
                            range.Style.Font.SetFromFont(new Font("Arial", 14));
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }
                    workSheet.PrinterSettings.FitToPage = true;
                    workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                    workSheet.PrinterSettings.VerticalCentered = true;
                    workSheet.PrinterSettings.HorizontalCentered = true;

                    indexSheet++;
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }
        public void CreateHeaderPage_IFG(ExcelWorksheet workSheet, PackingList packingList)
        {
            workSheet.Cells[1, 1].Value = "PACKING LIST";
            workSheet.Cells[1, 1].Style.Font.Bold = true;
            workSheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);

            int posCustomer = 10 + _countSize;

            _maxRow += 2;
            workSheet.Cells[_maxRow, 1].Value = "JOB NO:";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 1].EntireColumn.Width = 5.5;
            workSheet.Cells[_maxRow, 2].EntireColumn.Width = 2;
            workSheet.Cells[_maxRow, 3].EntireColumn.Width = 5.5;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4].Value = packingList.LSStyles.Replace(";", "+");
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "STYLE NO:";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.CustomerStyle;
            workSheet.Cells[_maxRow, posCustomer].Value = "Factory";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = packingList.Factory;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "MASTER STYLE#";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = "";
            workSheet.Cells[_maxRow, posCustomer].Value = "PO#:";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = _itemStyle.PurchaseOrderNumber;
            workSheet.Cells[_maxRow, posCustomer + 2].Style.Font.Bold = true;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "SHIP MODE";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.ShipMode;
            workSheet.Cells[_maxRow, posCustomer].Value = "SHIP TO";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = _itemStyle.ShipTo;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "DISTRIBUTION";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.PurchaseOrderType?.Name ?? string.Empty;
            workSheet.Cells[_maxRow, posCustomer].Value = "DESTINATION";
            workSheet.Cells[_maxRow, posCustomer, _maxRow, posCustomer + 1].Merge = true;
            workSheet.Cells[_maxRow, posCustomer + 2].Value = _itemStyle.DeliveryPlace;

            _maxRow++;
            workSheet.Cells[_maxRow, 1].Value = "Description";
            workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
            workSheet.Cells[_maxRow, 4].Value = _itemStyle.Description;

            _maxRow++;
            if (!_isAssortedSize_AssortedColor)
            {
                workSheet.Cells[_maxRow, 1].Value = "Color Name";
                workSheet.Cells["A" + _maxRow + ":C" + _maxRow].Merge = true;
                workSheet.Cells[_maxRow, 4].Value = _itemStyle.ColorName;
            }
        }
        public void CreateHeaderPackingLine_IFG(ExcelWorksheet workSheet, PackingList packinglist)
        {
            int column = 1;
            _maxRow++;
            _dicPositionColumnPackingLine = new Dictionary<string, int>();

            string prepack = packinglist.PackingLines?.FirstOrDefault()?.PrePack;
            bool checkAssortedSize = false;
            _length_Unit_IFG = packinglist?.PackingUnit?.LengthUnit ?? "cm";
            _weight_Unit_IFG = packinglist?.PackingUnit?.WeightUnit ?? "kgs";

            if (!string.IsNullOrEmpty(prepack) &&
                (prepack.Equals("A") || prepack.Equals("Assorted Size - Solid Color")))
            {
                if (!_isAssortedSize_AssortedColor)
                    checkAssortedSize = true;
            }

            string range = "A" + _maxRow + ":C" + (_maxRow + 1);
            workSheet.Cells[_maxRow, column].Value = CARTON_NUMBER;
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;
            _dicPositionColumnPackingLine.Add(FROM_NO, 1); // add position column 
            _dicPositionColumnPackingLine.Add(TO_NO, 3);

            column += 3;

            if (_isAssortedSize_AssortedColor)
            {
                range = "D" + _maxRow + ":D" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = STYLE_NO_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 18;
                _dicPositionColumnPackingLine.Add(STYLE_NO_IFG, column);

                column++;

                range = "E" + _maxRow + ":E" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = STYLE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;
                _dicPositionColumnPackingLine.Add(STYLE_IFG, column);

                column++;

                range = "F" + _maxRow + ":F" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_DESCR_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;
                _dicPositionColumnPackingLine.Add(COLOR_DESCR_IFG, column);

                column++;

                range = "G" + _maxRow + ":G" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(COLOR_CODE_IFG, column);

                column++;

                range = "H" + _maxRow + ":H" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = LABEL_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(LABEL_CODE_IFG, column);

                column++;
            }
            else
            {
                range = "D" + _maxRow + ":D" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = STYLE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 18;
                _dicPositionColumnPackingLine.Add(STYLE_IFG, column);

                column++;

                range = "E" + _maxRow + ":E" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_DESCR_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;
                _dicPositionColumnPackingLine.Add(COLOR_DESCR_IFG, column);

                column++;

                range = "F" + _maxRow + ":F" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(COLOR_CODE_IFG, column);

                column++;

                range = "G" + _maxRow + ":G" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = LABEL_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(LABEL_CODE_IFG, column);

                column++;
            }

            workSheet.Cells[_maxRow, column].Value = SIZE_IFG;
            _dicPositionColumnPackingLine.Add(SIZE_IFG, column);
            var sizeList = _itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size) &&
                    packinglist.PackingLines.Where(x => x.Size == size).Sum(x => x.TotalCarton) > 0)
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    workSheet.Cells[_maxRow + 1, column].Style.WrapText = true;
                    _dicPositionColumnPackingLine.Add(size, column);

                    if (size.Length > 7)
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 8;
                    }
                    else if (size.Length > 5)
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 7;
                    }
                    else
                    {
                        workSheet.Cells[_maxRow + 1, column].EntireColumn.Width = 5;
                    }
                    column++;
                }
            }

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_dicAlphabel.TryGetValue(column, out string unitsCharacter))
            {
                range = unitsCharacter + _maxRow + ":" + unitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = UNITS_CARTON;
                workSheet.Cells[range].Merge = true;
                workSheet.Cells[range].Style.WrapText = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(UNITS_CARTON, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string noOfCharacter))
            {
                range = noOfCharacter + _maxRow + ":" + noOfCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NO_OF_CARTON;
                _dicPositionColumnPackingLine.Add(NO_OF_CARTON, column);
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;

            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))
            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(TOTAL_UNITS, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string netWeightCharacter))
            {
                range = netWeightCharacter + _maxRow + ":" + netWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = NET_WEIGHT_IFG + " (" + _weight_Unit_IFG + ")";
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(NET_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string grossWeightCharacter))
            {
                range = grossWeightCharacter + _maxRow + ":" + grossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = GROSS_WEIGHT_IFG + " (" + _weight_Unit_IFG + ")"; ;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(GROSS_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string cartonDimensionCharacter))
            {
                workSheet.Cells[_maxRow, column].Value = CARTON_DIMENSION_IFG + " (" + _length_Unit_IFG + ")"; ;
                _dicPositionColumnPackingLine.Add(CARTON_DIMENSION_IFG, column);

                workSheet.Cells[_maxRow + 1, column].Value = LENGTH_IFG;
                _dicPositionColumnPackingLine.Add("LEN", column);
                workSheet.Column(column).Width = 4;

                workSheet.Cells[_maxRow + 1, ++column].Value = WIDTH_IFG;
                _dicPositionColumnPackingLine.Add(WIDTH_IFG, column);
                workSheet.Column(column).Width = 4;

                workSheet.Cells[_maxRow + 1, ++column].Value = HEIGHT_IFG;
                _dicPositionColumnPackingLine.Add(HEIGHT_IFG, column);
                workSheet.Column(column).Width = 4;

                if (_dicAlphabel.TryGetValue(column, out string endCartonDimensionCharacter))
                {
                    range = cartonDimensionCharacter + _maxRow + ":" + endCartonDimensionCharacter + _maxRow;
                    workSheet.Cells[range].Merge = true;
                    workSheet.Cells[range].Style.WrapText = true;
                }
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string measCharacter))
            {
                range = measCharacter + _maxRow + ":" + measCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = MEAS_M3_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
                _dicPositionColumnPackingLine.Add(MEAS_M3_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalNetWeightCharacter))
            {
                range = totalNetWeightCharacter + _maxRow + ":" + totalNetWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_NET_WEIGHT_IFG + " (" + _weight_Unit_IFG + ")"; ;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 8;
                _dicPositionColumnPackingLine.Add(TOTAL_NET_WEIGHT_IFG, column);
            }

            column++;

            if (_dicAlphabel.TryGetValue(column, out string totalGrossWeightCharacter))
            {
                range = totalGrossWeightCharacter + _maxRow + ":" + totalGrossWeightCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_GROSS_WEIGHT_IFG + " (" + _weight_Unit_IFG + ")"; ;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                _dicPositionColumnPackingLine.Add(TOTAL_GROSS_WEIGHT_IFG, column);

                string rangeBorder = "A10:" + totalGrossWeightCharacter + (_maxRow + 1);

                // border header packing line
                workSheet.Cells[rangeBorder].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rangeBorder].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[rangeBorder].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(column).Width = 8;
            }

            if (checkAssortedSize)
            {
                workSheet.Row(_maxRow).Height = 25;
                workSheet.Row(_maxRow + 1).Height = 25;
            }
            else
            {
                workSheet.Row(_maxRow).Height = 25;
                workSheet.Row(_maxRow + 1).Height = 18;
            }
            _maxColumn = column;
            _maxRow += 2;
        }
        public void FillDataPackingLine_IFG(ExcelWorksheet workSheet, PackingList packinglist)
        {
            string range = "A" + _maxRow + ":";
            decimal totalMeasM3 = 0;
            decimal netWeight = 0;
            decimal grossWeight = 0;
            decimal measM3 = 0;
            bool firstRow = true;
            int fromNo = 0;
            int summaryCarton = 0;
            string styleIFG = "";
            string colorDescrIFG = "";
            string descriptionIFG = "";
            int styleRow = 0;
            bool isSolidSize = false;
            string lsStyle = "";
            var itemStyle = new ItemStyle();
            var mergeCell = new List<int>();
            var styles = packinglist.ItemStyles.OrderBy(x => x.LSStyle).ToList();
            var totalStyle = packinglist.PackingLines.Select(x => x.LSStyle).Distinct().Count();
            var inchConvertValue = 1728 * 35.31;
            var lbConvertValue = 2.2046;

            _dicTotalQuantitySizeOverStyle = new Dictionary<string, decimal>();
            _dicTotalQuantitySize = new Dictionary<string, decimal>();
            styleRow = _maxRow;

            /// Get last carton previous ship - multi ship
            var newCartonNo = 0;
            var sheetNameID = ObjectSpace.GetObjects<PackingSheetName>()
                   .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;

            if (styles?.FirstOrDefault()?.MultiShip == true)
            {
                for (var i = 1; i < packinglist.OrdinalShip; i++)
                {
                    var firstStyle = packinglist.ItemStyles.FirstOrDefault(x => x.LSStyle == packinglist.LSStyles.Split(";")[0]);
                    var oldPackingList = firstStyle?.PackingLists
                                        .Where(x => x.OrdinalShip == i &&
                                              (x.SheetNameID ?? 0) != sheetNameID &&
                                              (x.ParentPackingListID ?? 0) == 0)
                                        .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                    if (oldPackingList != null)
                    {
                        newCartonNo += (int)(oldPackingList?.PackingLines?
                                                .Where(x => x.LSStyle == firstStyle?.LSStyle)
                                                .OrderByDescending(x => x.ToNo).FirstOrDefault()?.ToNo);
                    }
                }
            }

            if (!_isAssortedSize_AssortedColor)
            {
                var style = packinglist.ItemStyles.FirstOrDefault();
                var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo).ToList();

                // Fill Data 
                foreach (var packingLine in sortPackingLines)
                {
                    if (packingLine.PrePack == "Assorted Size - Solid Color")
                    {
                        if (firstRow || fromNo != packingLine.FromNo)
                        {
                            if (fromNo != packingLine.FromNo && !firstRow)
                            {
                                _maxRow++;
                            }
                            if (styleIFG.Length == 0)
                            {
                                styleIFG = style.CustomerStyle;
                            }
                            if (colorDescrIFG.Length == 0)
                            {
                                colorDescrIFG = style.ColorName;
                            }

                            /// Convert packing unit
                            if (_length_Unit_IFG.Trim().ToUpper() == "INCH")
                            {
                                measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                    (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                            }
                            else
                            {
                                measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                              packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                            }

                            if (_weight_Unit_IFG.Trim().ToUpper() == "LBS")
                            {
                                netWeight = (decimal)packingLine.NetWeight * (decimal)lbConvertValue;
                                grossWeight = (decimal)packingLine.GrossWeight * (decimal)lbConvertValue;
                            }
                            else
                            {
                                netWeight = (decimal)packingLine.NetWeight;
                                grossWeight = (decimal)packingLine.GrossWeight;
                            }

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo + newCartonNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo + newCartonNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_IFG]].Value = style.CustomerStyle;
                            //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO_IFG]].Value = style.CustomerStyle;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_DESCR_IFG]].Value = style.ColorName;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_IFG]]
                                .Value = style.Barcodes.Where(x => x.Size == packingLine.Size).ToList().Count > 0 ?
                                    style.Barcodes.FirstOrDefault(x => x.Size == packingLine.Size).Color : style.ColorCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL_CODE_IFG]].Value = style.LabelCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                    = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Value
                                    = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG]].Value
                                    = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine["LEN"]].Value = packingLine.Length;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH_IFG]].Value = packingLine.Width;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT_IFG]].Value = packingLine.Height;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                                    = Math.Round(netWeight, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                                    = Math.Round(grossWeight, 3);

                            decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                            // add total quantity per style
                            var key = style.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                            {
                                quantity += totalQuantity;
                                _dicTotalQuantitySizeOverStyle[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                            }
                            // add total quantity per size
                            if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                            {
                                sizeQuantity += totalQuantity;
                                _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                            }
                            _totalUnits += totalQuantity;

                            // add total carton
                            if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                            {
                                cartonQty += (int)packingLine.TotalCarton;
                                _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                            }
                            else
                            {
                                _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                            }
                            _totalNoOfCarton += (int)packingLine.TotalCarton;
                            _totalNetWeight += (decimal)Math.Round(netWeight, 3);
                            _totalGrossWeight += (decimal)Math.Round(grossWeight, 3);
                            totalMeasM3 += Math.Round(measM3, 3);

                            //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                            //{
                            //    _isFitToPage = true;
                            //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                            //}

                            firstRow = false;
                            fromNo = (int)packingLine.FromNo;
                            workSheet.Row(_maxRow).Height = 20;
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            // add total quantity per style
                            decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);
                            var key = style.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                            {
                                quantity += totalQuantity;
                                _dicTotalQuantitySizeOverStyle[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                            }
                            // add total quantity per size
                            if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                            {
                                sizeQuantity += totalQuantity;
                                _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                            }
                            _totalUnits += totalQuantity;
                            fromNo = (int)packingLine.FromNo;
                        }
                    }
                    else
                    {
                        if (packingLine.TotalCarton == 0)
                        {
                            continue;
                        }
                        if (firstRow)
                        {
                            if (styleIFG.Length == 0)
                            {
                                styleIFG = style.CustomerStyle;
                            }
                            if (colorDescrIFG.Length == 0)
                            {
                                colorDescrIFG = style.ColorName;
                            }

                            firstRow = false;
                        }

                        /// Convert packing unit
                        if (_length_Unit_IFG.Trim().ToUpper() == "INCH")
                        {
                            measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                    (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                        }
                        else
                        {
                            measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                          packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                        }

                        if (_weight_Unit_IFG.Trim().ToUpper() == "LBS")
                        {
                            netWeight = (decimal)packingLine.NetWeight * (decimal)lbConvertValue;
                            grossWeight = (decimal)packingLine.GrossWeight * (decimal)lbConvertValue;
                        }
                        else
                        {
                            netWeight = (decimal)packingLine.NetWeight;
                            grossWeight = (decimal)packingLine.GrossWeight;
                        }

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo + newCartonNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo + newCartonNo;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_IFG]].Value = style.CustomerStyle;
                        //workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO_IFG]].Value = style.CustomerStyle;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_DESCR_IFG]].Value = style.ColorName;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_IFG]]
                                .Value = style.Barcodes.Where(x => x.Size == packingLine.Size).ToList().Count > 0 ?
                                    style.Barcodes.FirstOrDefault(x => x.Size == packingLine.Size).Color : style.ColorCode;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL_CODE_IFG]].Value = style.LabelCode;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Value
                                = Math.Round(netWeight / (decimal)(packingLine.TotalCarton > 0 ? packingLine.TotalCarton : 1), 3);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG]].Value
                                = Math.Round(grossWeight / (decimal)(packingLine.TotalCarton > 0 ? packingLine.TotalCarton : 1), 3);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine["LEN"]].Value = packingLine.Length;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH_IFG]].Value = packingLine.Width;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT_IFG]].Value = packingLine.Height;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                                = Math.Round(netWeight, 3);
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                                = Math.Round(grossWeight, 3);

                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity);

                        // add total quantity
                        var key = style.LSStyle + ";" + packingLine.Size;
                        if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                        {
                            quantity += totalQuantity;
                            _dicTotalQuantitySizeOverStyle[key] = quantity;
                        }
                        else
                        {
                            _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                        }

                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;

                        // add total carton
                        if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                        {
                            cartonQty += (int)packingLine.TotalCarton;
                            _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                        }
                        else
                        {
                            _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                        }
                        _totalNoOfCarton += (int)packingLine.TotalCarton;
                        _totalNetWeight += (decimal)Math.Round(netWeight, 3);
                        _totalGrossWeight += (decimal)Math.Round(grossWeight, 3);
                        totalMeasM3 += Math.Round(measM3, 3);

                        //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                        //{
                        //    _isFitToPage = true;
                        //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                        //}
                        isSolidSize = true;

                        workSheet.Row(_maxRow).Height = 20;
                        _maxRow++;
                    }
                }

                _maxRow++;
            }
            else
            {
                foreach (var style in styles)
                {
                    if (styleIFG.Length == 0)
                    {
                        styleIFG = style.CustomerStyle.Trim();
                    }
                    else if (!styleIFG.Contains(style.CustomerStyle.Trim()))
                    {
                        var customerStyles = style.CustomerStyle.Trim().ToCharArray();
                        var index = 0;
                        for (int i = customerStyles.Length - 1; i >= 0; i--)
                        {
                            if (!int.TryParse(customerStyles[i].ToString(), out int value))
                            {
                                index = i;
                            }
                            else
                            {
                                break;
                            }
                        }
                        styleIFG += "/" + style.CustomerStyle.Substring(index, style.CustomerStyle.Length - index);
                    }

                    if (colorDescrIFG.Length == 0)
                    {
                        colorDescrIFG = style.ColorName.Trim();
                    }
                    else if (!colorDescrIFG.Contains(style.ColorName.Trim()))
                    {
                        colorDescrIFG += "/ " + style.ColorName.Trim();
                    }

                    if (descriptionIFG.Length == 0)
                    {
                        descriptionIFG = style.Description.Trim();
                    }
                    else
                    {
                        descriptionIFG += "/ " + style.Description.Trim();
                    }

                    if (_weight_Unit_IFG.Trim().ToUpper() == "LBS")
                    {
                        netWeight += (decimal)packinglist?.PackingLines?.
                            FirstOrDefault(x => x.LSStyle == style.LSStyle)?.NetWeight * (decimal)lbConvertValue;
                    }
                    else
                    {
                        netWeight += (decimal)packinglist?.PackingLines?.
                            FirstOrDefault(x => x.LSStyle == style.LSStyle)?.NetWeight;
                    }
                }
                workSheet.Cells[4, 4].Value = styleIFG;
                workSheet.Cells[8, 4].Value = descriptionIFG;

                var sortPackingLines = packinglist.PackingLines.OrderBy(x => x.SequenceNo)
                                            .OrderByDescending(x => x.TotalCarton).ToList();
                summaryCarton = (int)sortPackingLines[0].TotalCarton;
                lsStyle = sortPackingLines[0].LSStyle;
                foreach (var packingLine in sortPackingLines)
                {
                    itemStyle = packinglist.ItemStyles.FirstOrDefault(x => x.LSStyle == packingLine.LSStyle);
                    if (packingLine.TotalCarton != summaryCarton)
                    {
                        firstRow = true;
                        lsStyle = packingLine.LSStyle;
                        _maxRow++;
                    }
                    if (firstRow || packingLine.LSStyle == lsStyle)
                    {
                        if (firstRow)
                        {
                            /// Convert packing unit
                            if (_length_Unit_IFG.Trim().ToUpper() == "INCH")
                            {
                                measM3 = (decimal)(packingLine.Length * packingLine.Width * packingLine.Height /
                                    (decimal)inchConvertValue * (decimal)packingLine.TotalCarton);
                            }
                            else
                            {
                                measM3 = (decimal)(packingLine.Length / 100 * packingLine.Width / 100 *
                                              packingLine.Height / 100 * (decimal)packingLine.TotalCarton);
                            }

                            if (_weight_Unit_IFG.Trim().ToUpper() == "LBS")
                            {
                                grossWeight = netWeight + (decimal)((packingLine?.GrossWeight - packingLine?.NetWeight) * (decimal)lbConvertValue);
                            }
                            else
                            {
                                grossWeight = (decimal)(netWeight + packingLine?.GrossWeight - packingLine?.NetWeight);
                            }

                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO]].Value = packingLine.FromNo + newCartonNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[FROM_NO] + 1].Value = "~";
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TO_NO]].Value = packingLine.ToNo + newCartonNo;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO_IFG]].Value = itemStyle.CustomerStyle;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[COLOR_CODE_IFG]]
                                .Value = itemStyle.Barcodes.Where(x => x.Size == packingLine.Size).ToList().Count > 0 ?
                                         itemStyle.Barcodes.FirstOrDefault(x => x.Size == packingLine.Size).Color : itemStyle.ColorCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[LABEL_CODE_IFG]].Value = itemStyle.LabelCode;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value
                                    = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Value
                                    = Math.Round(netWeight / (decimal)packingLine.TotalCarton, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG]].Value
                                    = Math.Round(grossWeight / (decimal)packingLine.TotalCarton, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine["LEN"]].Value = packingLine.Length;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[WIDTH_IFG]].Value = packingLine.Width;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[HEIGHT_IFG]].Value = packingLine.Height;
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(measM3, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value
                                    = Math.Round(netWeight, 3);
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value
                                    = Math.Round(grossWeight, 3);

                            decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                            // add total quantity per style
                            var key = itemStyle.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                            {
                                quantity += totalQuantity;
                                _dicTotalQuantitySizeOverStyle[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                            }
                            // add total quantity per size
                            if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                            {
                                sizeQuantity += totalQuantity;
                                _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                            }
                            _totalUnits += totalQuantity;

                            // add total carton
                            if (_dicTotalCartons.TryGetValue(packingLine.BoxDimensionCode, out int cartonQty))
                            {
                                cartonQty += (int)packingLine.TotalCarton;
                                _dicTotalCartons[packingLine.BoxDimensionCode] = cartonQty;
                            }
                            else
                            {
                                _dicTotalCartons[packingLine.BoxDimensionCode] = (int)packingLine.TotalCarton;
                            }
                            _totalNoOfCarton += (int)packingLine.TotalCarton;
                            _totalNetWeight += (decimal)Math.Round(netWeight, 3);
                            _totalGrossWeight += (decimal)Math.Round(grossWeight, 3);
                            totalMeasM3 += Math.Round(measM3, 3);

                            //if (packingLine.Pad != null && !string.IsNullOrEmpty(packingLine.Pad.Description))
                            //{
                            //    _isFitToPage = true;
                            //    workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT] + 1].Value = packingLine.Pad.Description;
                            //}

                            mergeCell.Add(_maxRow);
                            firstRow = false;
                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                        }
                        else
                        {
                            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                            decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                            // add total quantity per style
                            var key = itemStyle.LSStyle + ";" + packingLine.Size;
                            if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                            {
                                quantity += totalQuantity;
                                _dicTotalQuantitySizeOverStyle[key] = quantity;
                            }
                            else
                            {
                                _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                            }
                            // add total quantity per size
                            if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                            {
                                sizeQuantity += totalQuantity;
                                _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                            }
                            else
                            {
                                _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                            }
                            _totalUnits += totalQuantity;
                            summaryCarton = (int)packingLine.TotalCarton;
                            lsStyle = packingLine.LSStyle;
                            workSheet.Row(_maxRow).Height = 20;
                        }

                    }
                    else
                    {
                        _maxRow++;

                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[STYLE_NO_IFG]].Value = itemStyle.CustomerStyle;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[packingLine.Size]].Value = packingLine.Quantity;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = packingLine.QuantityPerCarton;
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                                .Value = packingLine.QuantityPerCarton * packingLine.TotalCarton;
                        decimal totalQuantity = (decimal)(packingLine.TotalCarton * packingLine.Quantity * packingLine.PackagesPerBox);

                        // add total quantity per style
                        var key = itemStyle.LSStyle + ";" + packingLine.Size;
                        if (_dicTotalQuantitySizeOverStyle.TryGetValue(key, out decimal quantity))
                        {
                            quantity += totalQuantity;
                            _dicTotalQuantitySizeOverStyle[key] = quantity;
                        }
                        else
                        {
                            _dicTotalQuantitySizeOverStyle[key] = totalQuantity;
                        }
                        // add total quantity per size
                        if (_dicTotalQuantitySize.TryGetValue(packingLine.Size, out decimal sizeQuantity))
                        {
                            sizeQuantity += totalQuantity;
                            _dicTotalQuantitySize[packingLine.Size] = sizeQuantity;
                        }
                        else
                        {
                            _dicTotalQuantitySize[packingLine.Size] = totalQuantity;
                        }
                        _totalUnits += totalQuantity;

                        summaryCarton = (int)packingLine.TotalCarton;
                        lsStyle = packingLine.LSStyle;
                        workSheet.Row(_maxRow).Height = 20;
                    }
                }

                _maxRow++;
            }
            if (isSolidSize)
            {
                _maxRow--;
            }

            foreach (var merge in mergeCell)
            {
                workSheet.Cells["A" + merge + ":" + "A" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells["B" + merge + ":" + "B" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells["C" + merge + ":" + "C" + (merge + (totalStyle - 1))].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[NO_OF_CARTON],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[NO_OF_CARTON]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[NET_WEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[NET_WEIGHT_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[GROSS_WEIGHT_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine["LEN"],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine["LEN"]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[WIDTH_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[WIDTH_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[HEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[HEIGHT_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[MEAS_M3_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Merge = true;
                workSheet.Cells[merge, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG],
                               (merge + (totalStyle - 1)) + _mergeColumn, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Merge = true;
            }

            _style_IFG = styleIFG;
            _color_Descr_IFG = colorDescrIFG;
            workSheet.Cells[styleRow, _dicPositionColumnPackingLine[STYLE_IFG]].Value = _style_IFG;
            workSheet.Cells[styleRow, _dicPositionColumnPackingLine[COLOR_DESCR_IFG]].Value = _color_Descr_IFG;
            if (!isSolidSize)
            {
                workSheet.Cells["E" + styleRow + ":" + "E" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["F" + styleRow + ":" + "F" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["G" + styleRow + ":" + "G" + (_maxRow - 1)].Merge = true;
                if (_isAssortedSize_AssortedColor)
                {
                    workSheet.Cells["H" + styleRow + ":" + "H" + (_maxRow - 1)].Merge = true;
                }
                else
                {
                    workSheet.Cells["D" + styleRow + ":" + "D" + (_maxRow - 1)].Merge = true;
                }

            }

            // SET DATA TOTAL PACKING LINE
            workSheet.Cells[_maxRow, 1].Value = "TOTAL";
            workSheet.Cells[_maxRow, 1].Style.Font.Bold = true;
            workSheet.Rows[_maxRow].Height = 20;
            workSheet.Cells[_maxRow, 1, _maxRow, _dicPositionColumnPackingLine[LABEL_CODE_IFG]].Merge = true;

            foreach (var size in _dicTotalQuantitySize)
            {
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[size.Key]].Value = size.Value;
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = _totalNoOfCarton;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Style.Font.Bold = true;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = _totalUnits;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = true;

            if (totalMeasM3 == 0)
            {
                totalMeasM3 = measM3;
            }

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[MEAS_M3_IFG]].Value = Math.Round(totalMeasM3, 3);
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value = _totalNetWeight;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value = _totalGrossWeight;

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
                    borderData.Style.WrapText = true;
                }

                /// BORDER FIRST TOTAL
                using (var borderData = workSheet.Cells[_maxRow, 1, _maxRow, 4])
                {
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Font.Bold = true;
                }

                /// BORDER SECOND TOTAL
                range = _dicAlphabel[_dicPositionColumnPackingLine[NO_OF_CARTON]] + _maxRow + ":" +
                        _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]] + _maxRow;

                using (var borderData = workSheet.Cells[range])
                {
                    borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    borderData.Style.Font.Bold = true;
                    borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                }
            }

            /// FORMAT NUMBER
            range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_UNITS]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            range = _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + styleRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0.000";

            /// SET UNIT FOR TOTAL
            _maxRow++;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[NO_OF_CARTON]].Value = "Cnts";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Value = "(" + (packinglist?.ItemStyles?.FirstOrDefault()?.UnitID ?? "pcs") + ")";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_NET_WEIGHT_IFG]].Value = _weight_Unit_IFG;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]].Value = _weight_Unit_IFG;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON],
                            _maxRow, _dicPositionColumnPackingLine[TOTAL_GROSS_WEIGHT_IFG]]
                            .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            _maxRow += 3;

        }
        public void SummaryQuantitySize_Carton_IFG(ExcelWorksheet workSheet, PackingList packingList)
        {
            string range = "";
            int column = 4;
            int columnSize = _dicPositionColumnPackingLine[SIZE_IFG] + _countSize - 1;
            int columnTotalUnit = _dicPositionColumnPackingLine[UNITS_CARTON];
            int columnCartonDimension = _dicPositionColumnPackingLine[CARTON_DIMENSION_IFG];

            workSheet.Cells[_maxRow, 4].Value = "SUMMARY";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;

            _maxRow++;

            string rangeBoder = "D" + _maxRow + ":";
            string rangeCarton = _dicAlphabel[columnCartonDimension] + _maxRow + ":";

            if (_isAssortedSize_AssortedColor)
            {
                range = "D" + _maxRow + ":D" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = STYLE_NO_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;

                column++;

                range = "E" + _maxRow + ":E" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = STYLE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;

                column++;

                range = "F" + _maxRow + ":F" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_DESCR_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;

                column++;

                range = "G" + _maxRow + ":G" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;

                column++;

                range = "H" + _maxRow + ":H" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = LABEL_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;

                column++;
            }
            else
            {
                range = "D" + _maxRow + ":D" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = STYLE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;

                column++;

                range = "E" + _maxRow + ":E" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_DESCR_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 10;

                column++;

                range = "F" + _maxRow + ":F" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = COLOR_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;

                column++;

                range = "G" + _maxRow + ":G" + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = LABEL_CODE_IFG;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;

                column++;
            }

            workSheet.Cells[_maxRow, column].Value = SIZE_IFG;
            var sizeList = _itemStyle.OrderDetails.OrderBy(x => x.SizeSortIndex).ToList();

            for (int i = 0; i < _countSize; i++)
            {
                string size = sizeList[i].Size;
                if (!string.IsNullOrEmpty(size) &&
                    packingList.PackingLines.Where(x => x.Size == size).Sum(x => x.TotalCarton) > 0)
                {
                    workSheet.Cells[_maxRow + 1, column].Value = size;
                    column++;
                }
            }

            if (_dicAlphabel.TryGetValue(column - 1, out string character))
            {
                range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + _maxRow + ":" + character + _maxRow;
                workSheet.Cells[range].Merge = true;
            }

            if (_dicAlphabel.TryGetValue(column, out string totalUnitsCharacter))

            {
                range = totalUnitsCharacter + _maxRow + ":" + totalUnitsCharacter + (_maxRow + 1);
                workSheet.Cells[_maxRow, column].Value = TOTAL_UNITS;
                workSheet.Cells[_maxRow, column].Style.WrapText = true;
                workSheet.Cells[range].Merge = true;
                workSheet.Column(column).Width = 7;
            }

            range = _dicAlphabel[columnCartonDimension] + _maxRow + ":" + _dicAlphabel[columnCartonDimension + 2] + _maxRow;
            workSheet.Cells[_maxRow, columnCartonDimension].Value = "CTNS SUMMARY";
            workSheet.Cells[_maxRow, column].Style.WrapText = true;
            workSheet.Cells[range].Merge = true;

            workSheet.Cells[_maxRow, columnCartonDimension + 3].Value = "QTY";

            workSheet.Row(_maxRow).Height = 25;
            workSheet.Row(_maxRow + 1).Height = 18;

            int cartonRow = _maxRow + 1;

            _maxRow += 2;
            int formatRow = _maxRow;
            decimal totalUnitStyles = 0;
            var styles = packingList.ItemStyles.OrderBy(x => x.LSStyle).ToList();
            int firstRow = _maxRow;
            foreach (var style in styles)
            {
                column = 4;
                decimal totalUnit = 0;
                if (_isAssortedSize_AssortedColor)
                {
                    workSheet.Cells[_maxRow, column++].Value = style.ColorName;
                }
                workSheet.Cells[_maxRow, column++].Value = _style_IFG;
                workSheet.Cells[_maxRow, column++].Value = _color_Descr_IFG;
                workSheet.Cells[_maxRow, column++]
                    .Value = style.Barcodes.ToList().Count > 0 ? style.Barcodes.FirstOrDefault().Color : style.ColorCode;
                workSheet.Cells[_maxRow, column++].Value = style.LabelCode;
                foreach (var data in _dicTotalQuantitySizeOverStyle)
                {
                    if (data.Key.Split(";")[0] == style.LSStyle)
                    {
                        workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[data.Key.Split(";")[1]]].Value = data.Value;
                        totalUnit += (decimal)data.Value;
                    }
                }
                workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[UNITS_CARTON]].Value = totalUnit;
                totalUnitStyles += totalUnit;
                workSheet.Row(_maxRow).Height = 20;
                _maxRow++;
            }

            decimal totaCarton = 0;
            foreach (var carton in _dicTotalCartons)
            {
                workSheet.Cells[cartonRow, columnCartonDimension].Value = carton.Key;
                workSheet.Cells[cartonRow, columnCartonDimension, cartonRow, columnCartonDimension + 2].Merge = true;
                workSheet.Cells[cartonRow, columnCartonDimension + 3].Value = (decimal)carton.Value;
                totaCarton += (decimal)carton.Value;

                cartonRow++;
            }

            if (_isAssortedSize_AssortedColor)
            {
                workSheet.Cells["E" + firstRow + ":E" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["F" + firstRow + ":F" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["G" + firstRow + ":G" + (_maxRow - 1)].Merge = true;
                workSheet.Cells["H" + firstRow + ":H" + (_maxRow - 1)].Merge = true;
            }

            /// FORMAT NUMBER
            range = _dicAlphabel[_dicPositionColumnPackingLine[SIZE_IFG]] + formatRow + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[UNITS_CARTON]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            range = _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + (formatRow - 1) + ":" +
                    _dicAlphabel[_dicPositionColumnPackingLine[MEAS_M3_IFG]] + _maxRow;
            workSheet.Cells[range].Style.Numberformat.Format = "#,##0";

            range = "D" + _maxRow + ":" + _dicAlphabel[columnSize] + _maxRow;
            workSheet.Cells[_maxRow, 4].Value = "TOTAL";
            workSheet.Cells[_maxRow, 4].Style.Font.Bold = true;
            workSheet.Row(_maxRow).Height = 20;
            workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[range].Merge = true;

            workSheet.Cells[_maxRow, columnTotalUnit].Value = totalUnitStyles;
            workSheet.Cells[_maxRow, columnTotalUnit].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, columnTotalUnit].Style.Numberformat.Format = "#,##0";

            rangeBoder += _dicAlphabel[columnTotalUnit] + _maxRow;
            using (var borderData = workSheet.Cells[rangeBoder])
            {
                borderData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                borderData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                borderData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                borderData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                borderData.Style.WrapText = true;
            }

            workSheet.Cells[cartonRow, columnCartonDimension, cartonRow, columnCartonDimension + 2].Merge = true;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Value = totaCarton;
            workSheet.Cells[cartonRow, columnCartonDimension + 3].Style.Font.Bold = true;

            rangeCarton += _dicAlphabel[columnCartonDimension + 3] + cartonRow;
            using (var cartonData = workSheet.Cells[rangeCarton])
            {
                cartonData.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cartonData.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cartonData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cartonData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cartonData.Style.WrapText = true;
            }

            _maxRow += 3;
        }
        public void ShippingMark_IFG(ExcelWorksheet workSheet, PackingList packinglist)
        {
            string range = "";
            var style = packinglist.ItemStyles.FirstOrDefault();
            if (style != null)
            {
                range = "D" + _maxRow + ":" + "J" + (_maxRow + 2);
                workSheet.Cells[_maxRow, 4].Value = style.Packing;
                workSheet.Cells[_maxRow, 4].Style.WrapText = true;
                workSheet.Cells[_maxRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[_maxRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[range].Merge = true;
            }
            _maxRow += 5;

            workSheet.Cells[_maxRow, 4].Value = "XF :";
            workSheet.Cells[(_maxRow + 1), 4].Value = "ETD :";
            workSheet.Cells[(_maxRow + 2), 4].Value = "ETA :";
            workSheet.Cells[(_maxRow + 3), 4].Value = "PORT EXIT :";
            workSheet.Cells[_maxRow, 4, _maxRow + 3, 4].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, 4, _maxRow + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            _maxRow += 8;

            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]]
                .Value = packinglist.Company != null ? packinglist.Company.Name.Trim().ToUpper()
                         : "LEADING STAR VIETNAM GARMENT COMPANY LIMITED";
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Bold = true;
            workSheet.Cells[_maxRow, _dicPositionColumnPackingLine[TOTAL_UNITS]].Style.Font.Italic = true;
        }
        #endregion
        private void ResetData()
        {
            _itemStyle = new ItemStyle();
            _maxRow = 1;
            _maxColumn = 0;
            _countSize = 0;
            _isCanada = false;
            _isJapan = false;
            _isAruni = false;
            _isPanama = false;
            _isFitToPage = false;
            //_isAssortedSize_AssortedColor = false;
            _unitCarton = 0;
            _mergeColumn = 0;
            _LSStyle = string.Empty;

            _totalNoOfCarton = 0;
            _totalUnits = 0;
            _totalMeanM3 = 0;
            _totalNetWeight = 0;
            _totalGrossWeight = 0;
            _totalGrossWeightCarton = 0;
            _dicTotalQuantitySize = new Dictionary<string, decimal>();
            _dicTotalCartons = new Dictionary<string, int>();
            _dicTotalInnerCartons = new Dictionary<string, int>();
            _dicShipmentQuantity = new Dictionary<string, decimal>();
            _dicTotalQuantitySizeOverStyle = new Dictionary<string, decimal>();
        }
        private string CheckAssortedSize_AssortedColor(PackingList packingList)
        {
            var LSStyles = packingList?.LSStyles?.Split(";");
            var prePack = packingList.PackingLines?.FirstOrDefault()?.PrePack.Trim();

            if (LSStyles != null && LSStyles.Count() > 1)
            {
                _isAssortedSize_AssortedColor = true;
                foreach (var LSStyle in LSStyles)
                {
                    if (!String.IsNullOrEmpty(_LSStyle))
                    {
                        string lastCode = LSStyle.Substring(LSStyle.LastIndexOf('-') + 1,
                                        LSStyle.Length - LSStyle.LastIndexOf('-') - 1);
                        if (packingList.CustomerID == "PU")
                        {

                            _LSStyle += "/" + lastCode;
                        }
                        else if (packingList.CustomerID == "IFG")
                        {

                            _LSStyle += "+" + lastCode;
                        }
                        else if (packingList.CustomerID == "GA")
                        {

                            _LSStyle += " & " + LSStyle;
                        }
                        else if (packingList.CustomerID == "HM")
                        {

                            _LSStyle += "+" + lastCode;
                        }
                    }
                    else
                    {
                        _LSStyle = LSStyle;
                    }
                }

                /// Check packing type GA
                if (prePack == "Assorted Size - Solid Color")
                {
                    _packingType = "AssortedSize_AssortedColor";
                }
                else
                {
                    _packingType = "SolidSize_AssortedColor";
                }
            }
            else
            {
                _isAssortedSize_AssortedColor = false;
                if (packingList.CustomerID == "HA" &&
                    !string.IsNullOrEmpty((packingList?.BrandPurchaseOrder ?? string.Empty).Trim()))
                {
                    _LSStyle = _itemStyle?.LSStyle + "-" + packingList.BrandPurchaseOrder.Trim();
                }
                else
                {
                    _LSStyle = _itemStyle?.LSStyle;
                }

                /// Check packing type GA
                if (prePack == "Assorted Size - Solid Color")
                {
                    _packingType = "AssortedSize_SolidColor";
                }
                else
                {
                    _packingType = "SolidSize_SolidColor";
                }
            }

            return _LSStyle;
        }
        private void SetCountry(ref int posCustomer)
        {
            if (_itemStyle.CustomerCode.ToUpper().Trim().Equals(JAPAN))
            {
                _isJapan = true;
            }
            else
            {
                switch (_itemStyle.UCustomerCode?.ToUpper().Trim())
                {
                    case CANADA:
                        {
                            posCustomer = 14 + _countSize;
                            _isCanada = true;
                        }
                        break;
                    case ARUNI:
                        {
                            _isAruni = true;
                        }
                        break;
                    case PANAMA:
                        {
                            _isPanama = true;
                        }
                        break;
                }
            }
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

        public void GetSizeList(PackingList packingList)
        {
            _dicSizeList = new Dictionary<int, string>();
            var i = 1;

            foreach (var data in packingList.ItemStyles
                .OrderBy(x => x.LSStyle))
            {
                data.OrderDetails.Where(x => x.Quantity > 0)
                .OrderBy(o => o.SizeSortIndex).ToList().ForEach(o =>
                {
                    if (!_dicSizeList.ContainsValue(o.Size))
                    {
                        _dicSizeList.Add(i++, o.Size);
                    }
                });
            }
        }
    }
}
