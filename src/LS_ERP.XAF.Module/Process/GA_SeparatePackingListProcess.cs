using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class GA_SeparatePackingListProcess
    {
        public static List<PackingLine> SeparatePacking(List<PackingLine> packingLines, Dictionary<string, string> itemStyles,
                        List<PackingOverQuantity> currentOverQuantities, List<StyleNetWeight> netWeights, bool isUpdate)
        {
            var CartonNo = 1;
            var SequenceNo = 0;
            var newpackingLines = new List<PackingLine>();
            var packingOverQuantities = new List<PackingOverQuantity>();

            if (packingLines?.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
            {
                foreach (var style in itemStyles)
                {
                    var currentPackingLine = packingLines.Where(x => x.LSStyle == style.Key)
                            .OrderBy(x => x.SequenceNo).Take(currentOverQuantities.Where(x => x.ItemStyle.LSStyle == style.Value).Count()).ToList();
                    var totalRatio = currentPackingLine.Sum(x => x.Quantity);
                    var totalQuantityPerCarton = totalRatio * currentPackingLine.First()?.PackagesPerBox;

                    int localCartonNo = 0;
                    decimal netWeight = 0;
                    decimal innerWeight = currentPackingLine.First().InnerBoxDimension?.Weight ?? 0;
                    var totalQuantities = 0;

                    foreach (var sizePacking in currentOverQuantities
                        .Where(x => x.ItemStyle.LSStyle == style.Value).OrderBy(x => x.SizeSortIndex))
                    {
                        var sizeRatio = currentPackingLine
                            .FirstOrDefault(x => x.Size == sizePacking.Size)?.Quantity ?? 0;
                        if (sizeRatio == 0)
                        {
                            continue;
                        }
                        var styleNetWeight = netWeights
                            .FirstOrDefault(x => x.Size == sizePacking.Size &&
                                            x.CustomerStyle == sizePacking.ItemStyle.CustomerStyle);

                        var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;
                        totalQuantities = (int)(isUpdate ? currentPackingLine
                            .FirstOrDefault(x => x.Size == sizePacking.Size).TotalQuantity : sizePacking.Quantity);
                        var totalCarton = (int)(totalQuantities /
                                                (totalQuantityPerCarton * sizeRatio /
                                                (totalRatio)));

                        netWeight += styleNetWeight.NetWeight
                            * totalCarton * quantitySize ?? 0;

                    }

                    foreach (var currentOverQuantity in currentOverQuantities
                        .Where(x => x.ItemStyle.LSStyle == style.Value).OrderBy(x => x.SizeSortIndex))
                    {
                        localCartonNo = CartonNo;

                        var sizeRatio = currentPackingLine
                            .FirstOrDefault(x => x.Size == currentOverQuantity.Size)?.Quantity ?? 0;
                        if (sizeRatio == 0)
                        {
                            continue;
                        }

                        totalQuantities = (int)(isUpdate ? currentPackingLine
                             .FirstOrDefault(x => x.Size == currentOverQuantity.Size).TotalQuantity : currentOverQuantity.Quantity);

                        var packingLine = new PackingLine()
                        {
                            SequenceNo = SequenceNo.ToString("d3"),
                            LSStyle = currentOverQuantity.ItemStyle.LSStyle,
                            PrePack = "Assorted Size - Solid Color",
                            BoxDimension = currentPackingLine?.First()?.BoxDimension,
                            BoxDimensionCode = currentPackingLine?.First()?.BoxDimensionCode,
                            InnerBoxDimension = currentPackingLine?.First()?.InnerBoxDimension,
                            InnerBoxDimensionCode = currentPackingLine?.First()?.InnerBoxDimensionCode,
                            Quantity = sizeRatio,
                            Size = currentOverQuantity.Size,
                            QuantityPerCarton = totalQuantityPerCarton,
                            TotalCarton = (int)(totalQuantities /
                                                (totalQuantityPerCarton * sizeRatio /
                                                (totalRatio))),
                            QuantityPerPackage = totalRatio,
                            QuantitySize = totalQuantityPerCarton * sizeRatio / totalRatio,
                            PackagesPerBox = currentPackingLine.First()?.PackagesPerBox,
                            Width = currentPackingLine?.First()?.Width,
                            Height = currentPackingLine?.First()?.Height,
                            Length = currentPackingLine?.First()?.Length,
                            Color = currentOverQuantity.ItemStyle.ColorCode,
                            InnerWidth = currentPackingLine?.First()?.InnerWidth,
                            InnerHeight = currentPackingLine?.First()?.InnerHeight,
                            InnerLength = currentPackingLine?.First()?.InnerLength,
                            FromNo = localCartonNo,
                            ToNo = localCartonNo + (int)(totalQuantities /
                                                (totalQuantityPerCarton * sizeRatio / (totalRatio))) - 1,
                            DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                        };

                        if (packingLine.TotalQuantity == null)
                            packingLine.TotalQuantity = 0;

                        packingLine.TotalQuantity = packingLine.TotalCarton * packingLine.QuantitySize;
                        packingLine.NetWeight = netWeight;
                        packingLine.GrossWeight = netWeight + currentPackingLine?.First()?.BoxDimension?.Weight * packingLine.TotalCarton +
                            (packingLine.InnerBoxDimension?.Weight * packingLine.PackagesPerBox *
                            packingLine.TotalCarton ?? 0);

                        localCartonNo = packingLine.TotalCarton ?? 0;

                        newpackingLines.Add(packingLine);

                        SequenceNo += 1;

                        /// Remaining quantity - no handle
                    }

                    CartonNo += localCartonNo;

                    var total = newpackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                    foreach (var newpackingLine in newpackingLines)
                    {
                        newpackingLine.TotalQuantity = total;
                    }
                }
            }
            else
            {
                foreach (var style in itemStyles)
                {
                    foreach (var currentOverQuantity in currentOverQuantities
                                .Where(x => x.ItemStyle.LSStyle == style.Value).OrderBy(x => x.SizeSortIndex))
                    {
                        var currentPackingLine = packingLines
                            .FirstOrDefault(x => x.Size == currentOverQuantity.Size && x.LSStyle == style.Key);
                        if (currentPackingLine?.QuantityPerPackage > 0)
                        {
                            var totalQuantities = isUpdate ? currentPackingLine?.TotalQuantity : currentOverQuantity.Quantity;
                            int totalCarton = (int)(totalQuantities / (currentPackingLine?.QuantityPerPackage * currentPackingLine?.PackagesPerBox));
                            var netWeight = netWeights.FirstOrDefault(x => x.Size == currentOverQuantity.Size &&
                                                x.CustomerStyle == currentOverQuantity.ItemStyle.CustomerStyle)?.NetWeight ?? 0;

                            if (totalCarton > 0)
                            {
                                var packingLine = new PackingLine()
                                {
                                    SequenceNo = SequenceNo.ToString("d3"),
                                    LSStyle = currentOverQuantity.ItemStyle.LSStyle,
                                    PrePack = "Solid Size - Solid Color",
                                    BoxDimension = currentPackingLine?.BoxDimension,
                                    BoxDimensionCode = currentPackingLine?.BoxDimensionCode,
                                    InnerBoxDimension = currentPackingLine?.InnerBoxDimension,
                                    InnerBoxDimensionCode = currentPackingLine?.InnerBoxDimensionCode,
                                    Quantity = currentPackingLine.QuantityPerPackage,
                                    Size = currentOverQuantity.Size,
                                    QuantityPerCarton = currentPackingLine?.QuantityPerPackage * currentPackingLine?.PackagesPerBox,
                                    QuantitySize = currentPackingLine?.QuantityPerPackage * currentPackingLine?.PackagesPerBox,
                                    TotalCarton = (int)(totalQuantities / (currentPackingLine?.QuantityPerPackage * currentPackingLine?.PackagesPerBox)),
                                    QuantityPerPackage = currentPackingLine?.QuantityPerPackage,
                                    PackagesPerBox = currentPackingLine?.PackagesPerBox,
                                    Width = currentPackingLine?.Width,
                                    Height = currentPackingLine?.Height,
                                    Length = currentPackingLine?.Length,
                                    InnerWidth = currentPackingLine?.InnerWidth,
                                    InnerHeight = currentPackingLine?.InnerHeight,
                                    InnerLength = currentPackingLine?.InnerLength,
                                    Color = currentOverQuantity.ItemStyle.ColorCode,
                                    FromNo = CartonNo,
                                    ToNo = CartonNo + (int)(totalQuantities / (currentPackingLine?.QuantityPerPackage * currentPackingLine?.PackagesPerBox)) - 1,
                                    DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                                };

                                packingLine.TotalQuantity = packingLine.QuantitySize
                                    * packingLine.TotalCarton;
                                packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                packingLine.GrossWeight = packingLine.NetWeight + totalCarton * currentPackingLine?.BoxDimension.Weight;

                                CartonNo += (int)(totalQuantities / (currentPackingLine?.QuantityPerPackage * currentPackingLine?.PackagesPerBox));
                                newpackingLines.Add(packingLine);
                                SequenceNo += 1;

                                /// Remaining quantity - no handle
                            }
                        }
                        else
                        {
                            var totalQuantities = isUpdate ? currentPackingLine?.TotalQuantity : currentOverQuantity.Quantity;
                            int totalCarton = (int)(totalQuantities / currentPackingLine?.QuantitySize);
                            var netWeight = netWeights.FirstOrDefault(x => x.Size == currentOverQuantity.Size &&
                                                x.CustomerStyle == currentOverQuantity.ItemStyle.CustomerStyle)?.NetWeight ?? 0;

                            if (totalCarton > 0)
                            {
                                var packingLine = new PackingLine()
                                {
                                    SequenceNo = SequenceNo.ToString("d3"),
                                    LSStyle = currentOverQuantity.ItemStyle.LSStyle,
                                    PrePack = "Solid Size - Solid Color",
                                    BoxDimension = currentPackingLine?.BoxDimension,
                                    BoxDimensionCode = currentPackingLine?.BoxDimensionCode,
                                    InnerBoxDimension = currentPackingLine?.InnerBoxDimension,
                                    InnerBoxDimensionCode = currentPackingLine?.InnerBoxDimensionCode,
                                    Quantity = currentPackingLine.QuantitySize,
                                    Size = currentOverQuantity.Size,
                                    QuantityPerCarton = currentPackingLine.QuantitySize,
                                    QuantitySize = currentPackingLine.QuantitySize,
                                    TotalCarton = (int)(totalQuantities / currentPackingLine.QuantitySize),
                                    QuantityPerPackage = 0,
                                    PackagesPerBox = 0,
                                    Width = currentPackingLine?.Width,
                                    Height = currentPackingLine?.Height,
                                    Length = currentPackingLine?.Length,
                                    InnerWidth = currentPackingLine?.InnerWidth,
                                    InnerHeight = currentPackingLine?.InnerHeight,
                                    InnerLength = currentPackingLine?.InnerLength,
                                    Color = currentOverQuantity.ItemStyle.ColorCode,
                                    FromNo = CartonNo,
                                    ToNo = CartonNo + (int)(totalQuantities / currentPackingLine.QuantitySize) - 1,
                                    DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                                };

                                packingLine.TotalQuantity = packingLine.Quantity
                                    * packingLine.TotalCarton;
                                packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                packingLine.GrossWeight = packingLine.NetWeight + totalCarton * currentPackingLine?.BoxDimension.Weight;

                                CartonNo += (int)(totalQuantities / currentPackingLine.QuantitySize);
                                newpackingLines.Add(packingLine);
                                SequenceNo += 1;

                                /// Remaining quantity - no handle
                            }
                        }
                    }
                }
            }

            return newpackingLines;
        }
    }
}
