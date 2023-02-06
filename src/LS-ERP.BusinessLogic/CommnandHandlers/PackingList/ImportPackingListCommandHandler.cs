using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Dtos.PackingLine;
using LS_ERP.BusinessLogic.Dtos.PackingList;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Global;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportPackingListCommandHandler
        : IRequestHandler<ImportPackingListCommand, ImportPackingListResult>
    {
        private ILogger<ImportPackingListCommandHandler> logger;
        private SqlServerAppDbContext context;
        private PackingListValidator packingListValidator;
        private IMediator mediator;

        public ImportPackingListCommandHandler(ILogger<ImportPackingListCommandHandler> logger,
            SqlServerAppDbContext context,
            PackingListValidator packingListValidator,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.packingListValidator = packingListValidator;
            this.mediator = mediator;
        }

        public async Task<ImportPackingListResult> Handle(ImportPackingListCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Excute import packing list command", DateTime.Now.ToString());
            var result = new ImportPackingListResult();
            string fileName = request.File.FileName;
            string errorMessage = string.Empty;

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var packingListDtos = new List<PackingListImportDto>();
            if (request.CustomerID.Trim().ToUpper() == "HA")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(fullPath);

                if (fileInfo.Exists && (fileInfo.Extension.Equals(".xlsx") || fileInfo.Extension.Equals(".XLSX")))
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            Dictionary<int, int> columnIndex = new Dictionary<int, int>();
                            var workSheet = package.Workbook.Worksheets.First();
                            var packingTable = workSheet.ToDataTable(startHeader: 1, startColumn: 1,
                                                                        endColumn: 30, startRow: 1);
                            int idx = 1;
                            var JDPONo = "";
                            var checkJDPO = false;
                            for (var i = 0; i < packingTable.Rows.Count; i++)
                            {
                                if (i < 3)
                                {
                                    for (var j = 0; j < packingTable.Columns.Count; j++)
                                    {
                                        var columnValue = packingTable.Rows[i][j].ToString().Trim();
                                        if (!string.IsNullOrEmpty(columnValue))
                                        {
                                            if (columnValue == "HADDAD PO#")
                                            {
                                                columnIndex.Add(0, j);
                                            }
                                            else if (columnValue == "JD PO#")
                                            {
                                                columnIndex.Add(1, j);
                                            }
                                            else if (columnValue == "TOTAL NB of carton")
                                            {
                                                columnIndex.Add(2, j);
                                            }
                                            else if (columnValue == "SIZE ORDER")
                                            {
                                                columnIndex.Add(3, j);
                                            }
                                            else if (columnValue == "PO ORDER")
                                            {
                                                columnIndex.Add(4, j);
                                            }
                                            else if (columnValue == "QTY per carton")
                                            {
                                                columnIndex.Add(5, j);
                                            }
                                            else if (columnValue == "TOTAL QTY")
                                            {
                                                columnIndex.Add(6, j);
                                            }
                                            else if (columnValue == "LENGTH")
                                            {
                                                columnIndex.Add(7, j);
                                            }
                                            else if (columnValue == "WIDTH")
                                            {
                                                columnIndex.Add(8, j);
                                            }
                                            else if (columnValue == "HEIGHT")
                                            {
                                                columnIndex.Add(9, j);
                                            }
                                            else if (columnValue == "NW")
                                            {
                                                columnIndex.Add(10, j);
                                            }
                                            else if (columnValue == "GW")
                                            {
                                                columnIndex.Add(11, j);
                                            }
                                        }
                                    }
                                    if (columnIndex.TryGetValue(1, out int JDPOIndex))
                                    {
                                        checkJDPO = true;
                                    }
                                }
                                else
                                {
                                    var list = packingTable.Rows[i].ItemArray.ToList();
                                    if(!checkJDPO)
                                    {
                                        if (string.IsNullOrEmpty(list[0].ToString().Trim()))
                                        {
                                            idx++;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(list[0].ToString().Trim()))
                                        {
                                            continue;
                                        }
                                        else if(JDPONo != list[columnIndex[1]].ToString().Trim() &&
                                                JDPONo.Trim().Length > 0)
                                        {
                                            idx++;
                                        }
                                    }
                                    
                                    var packingListDto = new PackingListImportDto()
                                    {
                                        PONumber = list[columnIndex[0]].ToString().Trim(),
                                        JDPONumber = (checkJDPO ? list[columnIndex[1]] : string.Empty).ToString().Trim(),
                                        TotalCarton = string.IsNullOrEmpty(list[columnIndex[2]].ToString().Trim())
                                                             ? 0 : int.Parse(list[columnIndex[2]].ToString().Trim()),
                                        POSize = list[columnIndex[3]].ToString().Trim(),
                                        POOrder = string.IsNullOrEmpty(list[columnIndex[4]].ToString().Trim())
                                                             ? 0 : int.Parse(list[columnIndex[4]].ToString().Trim().Replace(",", "")),
                                        QuantityPerCarton = string.IsNullOrEmpty(list[columnIndex[5]].ToString().Trim())
                                                             ? 0 : int.Parse(list[columnIndex[5]].ToString().Trim().Replace(",", "")),
                                        TotalQuantity = string.IsNullOrEmpty(list[columnIndex[6]].ToString().Trim())
                                                             ? 0 : int.Parse(list[columnIndex[6]].ToString().Trim().Replace(",", "")),
                                        CartonLength = string.IsNullOrEmpty(list[columnIndex[7]].ToString().Trim())
                                                             ? 0 : decimal.Parse(list[columnIndex[7]].ToString().Trim().Replace(",", "")),
                                        CartonWidth = string.IsNullOrEmpty(list[columnIndex[8]].ToString().Trim())
                                                         ? 0 : decimal.Parse(list[columnIndex[8]].ToString().Trim().Replace(",", "")),
                                        CartonHeight = string.IsNullOrEmpty(list[columnIndex[9]].ToString().Trim())
                                                             ? 0 : decimal.Parse(list[columnIndex[9]].ToString().Trim().Replace(",", "")),
                                        NetWeight = list[columnIndex[10]].ToString().Trim(),
                                        GrossWeight = list[columnIndex[11]].ToString().Trim(),
                                        Index = idx,
                                    };
                                    if(checkJDPO)
                                    {
                                        JDPONo = packingListDto.JDPONumber;
                                    }

                                    packingListDtos.Add(packingListDto);
                                }
                            }
                        }
                    }
                }

                if (!packingListValidator.IsValid(packingListDtos, out errorMessage))
                {
                    result.Message = errorMessage;
                    return result;
                }
                
                try
                {
                    /// Check & create style net weight - box demension - packing unit 
                    foreach (var dto in packingListDtos)
                    {
                        dto.ItemStyle = context.ItemStyle.Include(x => x.OrderDetails)
                            .Where(x => x.PurchaseOrderNumber == dto.PONumber)
                            .OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                        decimal netWeight = 0;
                        if (decimal.TryParse(dto.NetWeight.Trim().Split(" ")[0], out netWeight))
                        {
                            var styleNetWeight = context.StyleNetWeight
                                .Where(x => x.CustomerStyle == dto.ItemStyle.CustomerStyle &&
                                       x.Size.Trim().ToUpper() == dto.POSize.Trim().ToUpper()).FirstOrDefault();
                            if (styleNetWeight == null)
                            {
                                styleNetWeight = new StyleNetWeight();
                                styleNetWeight.CustomerStyle = dto.ItemStyle.CustomerStyle;
                                styleNetWeight.NetWeight = (decimal)(netWeight / dto.QuantityPerCarton);
                                styleNetWeight.Size = dto.POSize.Trim();
                                styleNetWeight.CustomerID = request.CustomerID.Trim();
                                styleNetWeight.SetCreateAudit(request.GetUser());

                                context.StyleNetWeight.Add(styleNetWeight);
                                context.SaveChanges();
                            }
                        }

                        decimal grossWeight = 0;
                        if (decimal.TryParse(dto.GrossWeight.Trim().Split(" ")[0], out grossWeight))
                        {
                            var boxDemension = context.BoxDimension
                                .Where(x => x.Length == dto.CartonLength && x.Width == dto.CartonWidth &&
                                            x.Height == dto.CartonHeight).FirstOrDefault();
                            if (boxDemension == null)
                            {
                                boxDemension = new BoxDimension();
                                boxDemension.Code = dto.CartonLength + "x" + dto.CartonWidth + "x" + dto.CartonHeight;
                                boxDemension.Width = dto.CartonWidth;
                                boxDemension.Length = dto.CartonLength;
                                boxDemension.Height = dto.CartonHeight;
                                boxDemension.Weight = grossWeight - netWeight;
                                boxDemension.SetCreateAudit(request.GetUser());

                                context.BoxDimension.Add(boxDemension);
                                context.SaveChanges();

                            }
                            dto.BoxDemensionCode = boxDemension.Code;
                        }

                        dto.TotalNetWeight = (decimal)(netWeight * dto.TotalCarton);
                        dto.TotalGrossWeight = (decimal)(grossWeight * dto.TotalCarton);

                        var weightUnit = "cm";
                        var lengthUnit = "KGS";
                        if (dto.NetWeight.Trim().Contains(" "))
                        {
                            weightUnit = dto.NetWeight.Substring(dto.NetWeight.LastIndexOf(' ') + 1,
                                            dto.NetWeight.Length - dto.NetWeight.LastIndexOf(' ') - 1);
                            if (string.IsNullOrEmpty(weightUnit.Trim()))
                            {
                                weightUnit = dto.GrossWeight.Substring(dto.GrossWeight.LastIndexOf(' ') + 1,
                                                dto.GrossWeight.Length - dto.GrossWeight.LastIndexOf(' ') - 1);
                            }

                            if (!string.IsNullOrEmpty(weightUnit.Trim()))
                            {
                                lengthUnit = weightUnit.Trim().ToUpper() == "LBS" ? "inch" : "cm";
                                var packingUnit = context.PackingUnit
                                    .Where(x => x.LengthUnit.Trim().ToUpper() == lengthUnit.Trim().ToUpper() &&
                                           x.WeightUnit.Trim().ToUpper() == weightUnit.Trim().ToUpper()).FirstOrDefault();
                                if (packingUnit == null)
                                {
                                    packingUnit = new PackingUnit();
                                    packingUnit.ID = lengthUnit.Trim().ToUpper() + "/" + weightUnit.Trim().ToUpper();
                                    packingUnit.LengthUnit = lengthUnit.Trim();
                                    packingUnit.WeightUnit = weightUnit.Trim();
                                    packingUnit.Description = lengthUnit.Trim() + "/" + weightUnit.Trim();
                                    packingUnit.SetCreateAudit(request.GetUser());

                                    context.PackingUnit.Add(packingUnit);
                                    context.SaveChanges();
                                }
                                dto.PackingUnit = packingUnit;
                            }
                        }
                    }

                    /// Create packing list
                    var packingLists = new List<PackingList>();
                    var maxIndex = packingListDtos.Max(x => x.Index);
                    for (int i = 1; i <= maxIndex; i++)
                    {
                        var indexPackings = packingListDtos.Where(x => x.Index == i).ToList();
                        var sumFullCarton = indexPackings.Where(x => x.POOrder > 0).Sum(x => x.TotalCarton);
                        var cartonNo = 1;
                        
                        foreach (var packing in indexPackings)
                        {
                            if(packing.POOrder > 0)
                            {
                                var sequenceNo = 0;
                                var packingList = new PackingList()
                                {
                                    CustomerID = request.CustomerID.Trim(),
                                    PackingListDate = DateTime.Now,
                                    CompanyCode = "LS",
                                    PackingListCode = "PK-" + Nanoid.Nanoid
                                            .Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6),
                                    BrandPurchaseOrder = packing.JDPONumber,
                                    PackingUnit = packing.PackingUnit,
                                };
                               
                                /// Create packing line - đóng thùng chẵn
                                var packingLine = new PackingLine()
                                {
                                    SequenceNo = sequenceNo.ToString("d3"),
                                    LSStyle = packing.ItemStyle.LSStyle,
                                    PrePack = "Solid Size - Solid Color",
                                    BoxDimensionCode = packing.BoxDemensionCode,
                                    Quantity = packing.QuantityPerCarton,
                                    Size = packing.POSize,
                                    QuantityPerCarton = packing.QuantityPerCarton,
                                    QuantitySize = packing.QuantityPerCarton,
                                    TotalQuantity = packing.TotalQuantity,
                                    TotalCarton = packing.TotalCarton,
                                    QuantityPerPackage = 0,
                                    PackagesPerBox = 0,
                                    Width = packing.CartonWidth,
                                    Height = packing.CartonHeight,
                                    Length = packing.CartonLength,
                                    Color = packing.ItemStyle.ColorCode,
                                    FromNo = cartonNo,
                                    ToNo = cartonNo + packing.TotalCarton - 1,
                                    DeliveryPlace = packing.ItemStyle.DeliveryPlace,
                                    NetWeight = packing.TotalNetWeight,
                                    GrossWeight = packing.TotalGrossWeight,
                                };

                                packingList.PackingLines.Add(packingLine);
                                sequenceNo++;

                                /// Đóng thùng lẽ
                                var remainPacking = indexPackings
                                    .Where(x => x.PONumber == packing.PONumber &&
                                           x.POSize == packing.POSize && x.POOrder == 0).FirstOrDefault();
                                if(remainPacking != null)
                                {
                                    sumFullCarton += 1;
                                    var remainPackingLine = new PackingLine()
                                    {
                                        SequenceNo = sequenceNo.ToString("d3"),
                                        LSStyle = remainPacking.ItemStyle.LSStyle,
                                        PrePack = "R",
                                        BoxDimensionCode = remainPacking.BoxDemensionCode,
                                        Quantity = remainPacking.QuantityPerCarton,
                                        Size = remainPacking.POSize,
                                        QuantityPerCarton = remainPacking.QuantityPerCarton,
                                        QuantitySize = remainPacking.QuantityPerCarton,
                                        TotalQuantity = remainPacking.TotalQuantity,
                                        TotalCarton = remainPacking.TotalCarton,
                                        QuantityPerPackage = 0,
                                        PackagesPerBox = 0,
                                        Width = remainPacking.CartonWidth,
                                        Height = remainPacking.CartonHeight,
                                        Length = remainPacking.CartonLength,
                                        Color = remainPacking.ItemStyle.ColorCode,
                                        FromNo = sumFullCarton,
                                        ToNo = sumFullCarton,
                                        DeliveryPlace = remainPacking.ItemStyle.DeliveryPlace,
                                        NetWeight = remainPacking.TotalNetWeight,
                                        GrossWeight = remainPacking.TotalGrossWeight,
                                    };
                                    packingList.PackingLines.Add(remainPackingLine);
                                }

                                packingList.SetCreateAudit(request.GetUser());
                                packingList.ItemStyles.Add(packing.ItemStyle);
                                packingList.TotalQuantity = packingList.PackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                                packingList.LSStyles = string.Join(";", packingList.ItemStyles.Select(x => x.LSStyle).Distinct());
                                //packingList.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                                // {
                                //     ImageUrl = AppGlobal.DontShortShip,
                                //     SortIndex = 1
                                // });
                                packingLists.Add(packingList);

                                cartonNo += (int)packing.TotalCarton;
                            }
                        }
                    }
                   
                    context.PackingList.AddRange(packingLists);

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;

                    result.Data = packingLists;
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.InnerException.Message;
                }
            }
            else if (request.CustomerID.Trim().ToUpper() == "DE")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(fullPath);

                var packingLineDtos = new List<PackingLineImportDto>();

                if (fileInfo.Exists && (fileInfo.Extension.Equals(".xlsx") || fileInfo.Extension.Equals(".XLSX")))
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            var workSheet = package.Workbook.Worksheets.First();
                            var packingTable = workSheet.ToDataTable(startHeader: 1, startColumn: 1,
                                                                        endColumn: 19, startRow: 1);

                            foreach (DataRow row in packingTable.Rows)
                            {
                                var line = new PackingLineImportDto();
                                line.FromNo = int.Parse(row["FROM NO"]?.ToString());
                                line.ToNo = int.Parse(row["TO NO"]?.ToString());
                                line.LSStyle = row["STYLE  NO  BY LS"]?.ToString();
                                line.Size = row["SIZE"]?.ToString();
                                line.QuantityPerCarton = decimal.Parse(row["PCB"]?.ToString());
                                line.TotalCarton = int.Parse(row["NO. CARTON"]?.ToString());
                                line.Quantity = decimal.Parse(row["PCB"]?.ToString());
                                line.TotalQuantity = decimal.Parse(row["TOTAL QTY/SIZE"]?.ToString());
                                line.QuantitySize = line.TotalQuantity / line.TotalCarton;
                                line.PackagesPerBox = decimal.Parse(row["UE"]?.ToString());
                                line.QuantityPerPackage = line.QuantityPerCarton / line.PackagesPerBox;
                                line.NetWeight = decimal.Parse(row["TOTAL NW"]?.ToString());
                                line.GrossWeight = decimal.Parse(row["TOTAL GW"]?.ToString());
                                line.Length = decimal.Parse(row["LENGTH"]?.ToString());
                                line.Width = decimal.Parse(row["WIDTH"]?.ToString());
                                line.Height = decimal.Parse(row["HEIGHT"]?.ToString());
                                line.BoxDimensionCode = row["LENGTH"]?.ToString() + "*" + row["WIDTH"]?.ToString() + "*" + row["HEIGHT"]?.ToString();
                                line.SequenceNo = int.Parse(row["SEQUENCE"]?.ToString()).ToString("d3");
                                line.PrePack = row["PREPACK"]?.ToString();
                                line.Note = row["NOTE"]?.ToString();
                                if (decimal.TryParse(row["TOTAL QTY"]?.ToString(), out decimal outQuantity))
                                    line.SummaryQuantity = outQuantity;
                                else
                                    line.SummaryQuantity = 0;

                                packingLineDtos.Add(line);
                            }
                        }
                    }
                }

                try
                {
                    /// Check & insert box dimension from file Import
                    var importBoxDimesions = new List<BoxDimension>();

                    var boxDimensions = context.BoxDimension
                        .Where(b => packingLineDtos.Select(x => x.BoxDimensionCode).Contains(b.Code)).ToList();

                    var sheetNames = context.PackingSheetName.ToList();

                    foreach (var box in packingLineDtos
                        .Select(x => x.BoxDimensionCode).Distinct())
                    {
                        if (!boxDimensions.Select(x => x.Code).Contains(box))
                        {
                            var import = packingLineDtos.FirstOrDefault(p => p.BoxDimensionCode == box);
                            var boxDimension = new BoxDimension()
                            {
                                Code = import.BoxDimensionCode,
                                Description = "DE",
                                Length = import.Length,
                                Width = import.Width,
                                Height = import.Height,
                                Weight = (import.GrossWeight - import.NetWeight) / import.TotalCarton,
                            };
                            boxDimension.SetCreateAudit(request.GetUser());

                            importBoxDimesions.Add(boxDimension);
                        }
                    }

                    /// Get Item Style from file Import
                    var itemStyles = context.ItemStyle
                        .Where(i => packingLineDtos.Select(x => x.LSStyle).Contains(i.LSStyle)).ToList();

                    //var withOutStyle = new List<string>();
                    //foreach (var style in packingLineDtos
                    //    .Select(x => x.LSStyle).Distinct())
                    //{
                    //    if (!itemStyles.Select(x => x.LSStyle).Contains(style))
                    //    {
                    //        withOutStyle.Add(style);
                    //    }
                    //}

                    var importPackingLists = new List<PackingList>();
                    
                    var mappingConfig = new MapperConfiguration(c =>
                    {
                        c.CreateMap<PackingLineImportDto, PackingLine>();
                    });
                    var mapper = mappingConfig.CreateMapper();
                    /// Create packing list
                    foreach (var itemStyle in itemStyles)
                    {
                        var importLines = packingLineDtos.Where(x => x.LSStyle == itemStyle.LSStyle)
                                                            .OrderBy(x => x.SequenceNo).ToList();
                        if(importLines.Select(o => o.Note).Distinct().Count() > 1)
                        {
                            foreach (var ordinal in importLines.Select(o => o.Note).Distinct())
                            {
                                var newPackingLines = importLines
                                        .Where(x => x.Note?.Trim()?.ToUpper() == ordinal?.Trim().ToUpper())
                                        .Select(x => mapper.Map<PackingLine>(x)).ToList();
                                
                                var sheet = string.IsNullOrEmpty(ordinal) ? "TOTAL" : ordinal?.Trim().ToUpper();

                                var sheetName = sheetNames.FirstOrDefault(s => s.SheetName.ToUpper() == sheet);

                                newPackingLines.ForEach(x =>
                                {
                                    x.DeliveryPlace = itemStyle.DeliveryPlace;
                                    x.Color = itemStyle.ColorCode;
                                    x.SetCreateAudit(request.GetUser());
                                });

                                var newStyles = new List<ItemStyle>();
                                newStyles.Add(itemStyle);

                                var packingList = new PackingList()
                                {
                                    PackingListCode = "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6),
                                    PackingListDate = DateTime.Now,
                                    CompanyCode = "LS",
                                    LSStyles = itemStyle.LSStyle,
                                    ItemStyles = newStyles,
                                    CustomerID = "DE",
                                    Confirm = true,
                                    TotalQuantity = newPackingLines.Sum(x => x.TotalQuantity),
                                    BrandPurchaseOrder = "Import",
                                    PackingLines = newPackingLines,
                                    SheetName = sheetName,
                                };
                                packingList.SetCreateAudit(request.GetUser());

                                importPackingLists.Add(packingList);
                            }
                        }
                        else
                        {
                            var sheet = string.IsNullOrEmpty(importLines?.FirstOrDefault()?.Note)
                                        ? "TOTAL" : importLines?.FirstOrDefault()?.Note?.Trim().ToUpper();

                            var sheetName = sheetNames.FirstOrDefault(s => s.SheetName.ToUpper() == sheet);

                            var newPackingLines = importLines.Select(x => mapper.Map<PackingLine>(x)).ToList();
                            newPackingLines.ForEach(x =>
                            {
                                x.DeliveryPlace = itemStyle.DeliveryPlace;
                                x.Color = itemStyle.ColorCode;
                                x.SetCreateAudit(request.GetUser());
                            });

                            var newStyles = new List<ItemStyle>();
                            newStyles.Add(itemStyle);

                            var packingList = new PackingList()
                            {
                                PackingListCode = "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6),
                                PackingListDate = DateTime.Now,
                                CompanyCode = "LS",
                                LSStyles = itemStyle.LSStyle,
                                ItemStyles = newStyles,
                                CustomerID = "DE",
                                Confirm = true,
                                TotalQuantity = importLines.Sum(x => x.SummaryQuantity),
                                BrandPurchaseOrder = "Import",
                                PackingLines = newPackingLines,
                                SheetName = sheetName,
                            };
                            packingList.SetCreateAudit(request.GetUser());

                            importPackingLists.Add(packingList);
                        }

                        
                    }

                    context.BoxDimension.AddRange(importBoxDimesions);
                    context.PackingList.AddRange(importPackingLists);

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;

                    result.Data = importPackingLists;
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.InnerException.Message;
                }

            }

            return result;
        }
    }
}
