using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class IFG_SeparatePackingListProcess
    {
        public static List<PackingLine> SeparatePacking(List<PackingLine> packingLines,
           List<ItemStyle> ifgStyles, List<StyleNetWeight> netWeights)
        {
            var newpackingLines = new List<PackingLine>();
            int sequenceNo = 0;

            var packingOverQuantities = new List<PackingOverQuantity>();
            if (packingLines?.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
            {
                foreach (var style in ifgStyles)
                {
                    var currentPackingLine = packingLines.Where(x => x.LSStyle == style.LSStyle).OrderBy(x => x.SequenceNo).ToList();
                    var totalRatio = currentPackingLine.Sum(x => x.Quantity);
                    var totalQuantityPerCarton = totalRatio * currentPackingLine.First()?.PackagesPerBox;

                    int localCartonNo = 0;
                    decimal netWeight = 0;
                    decimal innerWeight = currentPackingLine.First().InnerBoxDimension?.Weight ?? 0;
                    var totalQuantities = 0;

                    foreach (var sizePacking in style.OrderDetails.OrderBy(x => x.SizeSortIndex))
                    {
                        var sizeRatio = currentPackingLine
                            .FirstOrDefault(x => x.Size == sizePacking.Size)?.Quantity ?? 0;
                        var styleNetWeight = netWeights.FirstOrDefault(x =>
                                                    x.Size == sizePacking.Size &&
                                                    x.CustomerStyle == style.CustomerStyle);

                        var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;
                        totalQuantities = (int)(currentPackingLine
                                                .FirstOrDefault(x => x.Size == sizePacking.Size).TotalQuantity);
                        var totalCarton = (int)(totalQuantities /
                                                (totalQuantityPerCarton * sizeRatio /
                                                (totalRatio)));

                        netWeight += styleNetWeight.NetWeight
                            * totalCarton * quantitySize ?? 0;

                    }

                    foreach (var orderDetail in style.OrderDetails.OrderBy(x => x.SizeSortIndex))
                    {
                        var line = currentPackingLine?.FirstOrDefault(l => l.Size == orderDetail.Size);

                        var sizeRatio = currentPackingLine
                            .FirstOrDefault(x => x.Size == orderDetail.Size)?.Quantity ?? 0;
                        totalQuantities = (int)(currentPackingLine
                             .FirstOrDefault(x => x.Size == orderDetail.Size).TotalQuantity);

                        var packingLine = new PackingLine()
                        {
                            SequenceNo = line?.SequenceNo,
                            LSStyle = style.LSStyle,
                            PrePack = "Assorted Size - Solid Color",
                            BoxDimension = line?.BoxDimension,
                            BoxDimensionCode = line?.BoxDimensionCode,
                            InnerBoxDimension = line?.InnerBoxDimension,
                            InnerBoxDimensionCode = line?.InnerBoxDimensionCode,
                            Quantity = sizeRatio,
                            Size = orderDetail.Size,
                            QuantityPerCarton = totalQuantityPerCarton,
                            TotalCarton = (int)(totalQuantities /
                                                (totalQuantityPerCarton * sizeRatio /
                                                (totalRatio))),
                            QuantityPerPackage = totalRatio,
                            QuantitySize = totalQuantityPerCarton * sizeRatio / totalRatio,
                            PackagesPerBox = line?.PackagesPerBox,
                            Width = line?.Width,
                            Height = line?.Height,
                            Length = line?.Length,
                            Color = style.ColorCode,
                            InnerWidth = line?.InnerWidth,
                            InnerHeight = line?.InnerHeight,
                            InnerLength = line?.InnerLength,
                            FromNo = line?.FromNo,
                            ToNo = line.ToNo,
                            DeliveryPlace = style.DeliveryPlace
                        };

                        if (packingLine.TotalQuantity == null)
                            packingLine.TotalQuantity = 0;

                        packingLine.TotalQuantity = packingLine.TotalCarton * packingLine.QuantitySize;
                        packingLine.NetWeight = netWeight;
                        packingLine.GrossWeight = netWeight + line?.BoxDimension?.Weight * packingLine.TotalCarton +
                            (packingLine.InnerBoxDimension?.Weight * packingLine.PackagesPerBox *
                            packingLine.TotalCarton ?? 0);

                        localCartonNo = packingLine.TotalCarton ?? 0;

                        newpackingLines.Add(packingLine);

                        //sequenceNo += 1;
                    }

                    var total = newpackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                    foreach (var newpackingLine in newpackingLines)
                    {
                        newpackingLine.TotalQuantity = total;
                    }
                }
            }
            else
            {
                foreach (var style in ifgStyles)
                {
                    foreach (var orderDetail in style.OrderDetails.OrderBy(x => x.SizeSortIndex))
                    {
                        var currentPackingLine = packingLines
                            .FirstOrDefault(x => x.PrePack.Trim() == "Solid Size - Solid Color" &&
                                            x.Size == orderDetail.Size &&
                                            x.LSStyle == style.LSStyle);
                        if (currentPackingLine == null ||
                            (currentPackingLine?.QuantitySize ?? 0) == 0)
                        {
                            continue;
                        }
                        else
                        {
                            var totalQuantities = currentPackingLine?.TotalQuantity;
                            int totalCarton = (int)(totalQuantities / currentPackingLine?.QuantitySize);
                            var netWeight = netWeights.FirstOrDefault(x => x.Size == orderDetail.Size)?.NetWeight ?? 0;

                            if (totalCarton > 0)
                            {
                                var packingLine = new PackingLine()
                                {
                                    SequenceNo = sequenceNo.ToString("d3"),
                                    LSStyle = style.LSStyle,
                                    PrePack = "Solid Size - Solid Color",
                                    BoxDimension = currentPackingLine?.BoxDimension,
                                    BoxDimensionCode = currentPackingLine?.BoxDimensionCode,
                                    InnerBoxDimension = currentPackingLine?.InnerBoxDimension,
                                    InnerBoxDimensionCode = currentPackingLine?.InnerBoxDimensionCode,
                                    Quantity = currentPackingLine.QuantitySize,
                                    Size = orderDetail.Size,
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
                                    Color = orderDetail.ItemStyle.ColorCode,
                                    FromNo = currentPackingLine.FromNo,
                                    ToNo = currentPackingLine.ToNo,
                                    DeliveryPlace = style.DeliveryPlace
                                };

                                packingLine.TotalQuantity = packingLine.Quantity
                                    * packingLine.TotalCarton;
                                packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                packingLine.GrossWeight = packingLine.NetWeight + totalCarton * currentPackingLine?.BoxDimension.Weight;

                                newpackingLines.Add(packingLine);
                                sequenceNo += 1;
                            }
                        }
                    }
                }
            }

            return newpackingLines;
        }
    }
}
