using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class IFG_ClonePackingListProcess
    {
        public static List<PackingLine> GeneratePacking(List<PackingLine> packingLines,
           List<PackingOverQuantity> currentOverQuantities, List<StyleNetWeight> netWeights, bool isUpdate, bool isRevised, ref int SequenceNo, ref int CartonNo)
        {
            //var CartonNo = 1;
            //var SequenceNo = 0;
            var newpackingLines = new List<PackingLine>();

            var packingOverQuantities = new List<PackingOverQuantity>();
            if (packingLines?.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
            {
                var currentPackingLine = packingLines.OrderBy(x => x.SequenceNo).Take(currentOverQuantities.Count()).ToList();
                var totalRatio = currentPackingLine.Sum(x => x.Quantity);
                var totalQuantityPerCarton = totalRatio * currentPackingLine.First()?.PackagesPerBox;

                int localCartonNo = 0;
                decimal netWeight = 0;
                decimal innerWeight = currentPackingLine.First().InnerBoxDimension?.Weight ?? 0;
                var totalQuantities = 0;

                foreach (var sizePacking in currentOverQuantities.OrderBy(x => x.SizeSortIndex))
                {
                    var sizeRatio = currentPackingLine
                        .FirstOrDefault(x => x.Size == sizePacking.Size)?.Quantity ?? 0;
                    var styleNetWeight = netWeights
                        .FirstOrDefault(x => x.Size == sizePacking.Size);

                    var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;
                    totalQuantities = (int)(isUpdate ? currentPackingLine
                        .FirstOrDefault(x => x.Size == sizePacking.Size).TotalQuantity : sizePacking.Quantity);
                    var totalCarton = (int)(totalQuantities /
                                            (totalQuantityPerCarton * sizeRatio /
                                            (totalRatio)));

                    netWeight += styleNetWeight.NetWeight
                        * totalCarton * quantitySize ?? 0;

                }

                foreach (var currentOverQuantity in currentOverQuantities.OrderBy(x => x.SizeSortIndex))
                {
                    localCartonNo = CartonNo;

                    var sizeRatio = currentPackingLine
                        .FirstOrDefault(x => x.Size == currentOverQuantity.Size)?.Quantity ?? 0;
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
                        FromNo = isRevised ? currentPackingLine?.First()?.FromNo : localCartonNo,
                        ToNo = isRevised ? currentPackingLine?.First()?.ToNo : (localCartonNo + (int)(totalQuantities /
                                            (totalQuantityPerCarton * sizeRatio / (totalRatio))) - 1),
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

                    ///Phần dư
                    //var remainQuantity = (int)(totalQuantities %
                    //        (packingLine.PackagesPerBox * sizeRatio * packingLine.TotalCarton));
                    //if (remainQuantity > 0)
                    //{
                    //    var packingOverQuantity = new PackingOverQuantity()
                    //    {
                    //        Quantity = remainQuantity,
                    //        ItemStyleNumber = currentOverQuantity.ItemStyleNumber,
                    //        ColorCode = currentOverQuantity.ColorCode,
                    //        ColorName = currentOverQuantity.ColorName,
                    //        Size = currentOverQuantity.Size,
                    //        SizeSortIndex = currentOverQuantity.SizeSortIndex,
                    //        ItemStyle = currentOverQuantity.ItemStyle
                    //    };
                    //    packingOverQuantities.Add(packingOverQuantity);
                    //}
                }

                CartonNo += localCartonNo;

                var total = newpackingLines.Sum(x => x.TotalQuantity);
                foreach (var newpackingLine in newpackingLines)
                {
                    newpackingLine.TotalQuantity = total;
                }

                //if (packingOverQuantities.Any())
                //{
                //    var remainPackingLine = packingLines.OrderByDescending(x => x.SequenceNo).FirstOrDefault();
                //    totalQuantityPerCarton = totalRatio * packingOverQuantities.Min(x => x.Quantity);
                //    netWeight = 0;
                //    innerWeight = remainPackingLine?.InnerBoxDimension?.Weight ?? 0;

                //    foreach (var sizePacking in packingOverQuantities)
                //    {
                //        var sizeRatio = currentPackingLine
                //            .FirstOrDefault(x => x.Size == sizePacking.Size)?.Quantity ?? 0;
                //        var styleNetWeight = netWeights
                //            .FirstOrDefault(x => x.Size == sizePacking.Size);

                //        var totalCarton = (int)(sizePacking.Quantity /
                //                                (totalQuantityPerCarton * sizeRatio /
                //                                (totalRatio)));

                //        var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;

                //        netWeight += styleNetWeight.NetWeight
                //            * totalCarton * quantitySize ?? 0;
                //    }
                //    foreach (var overQuantity in packingOverQuantities.OrderBy(x => x.SizeSortIndex))
                //    {
                //        var sizeRatio = currentPackingLine
                //            .FirstOrDefault(x => x.Size == overQuantity.Size)?.Quantity ?? 0;

                //        var packingLine = new PackingLine()
                //        {
                //            SequenceNo = SequenceNo.ToString("d3"),
                //            LSStyle = overQuantity.ItemStyle?.LSStyle,
                //            PrePack = "Assorted Size - Solid Color",
                //            BoxDimension = remainPackingLine?.BoxDimension,
                //            BoxDimensionCode = remainPackingLine?.BoxDimensionCode,
                //            InnerBoxDimension = remainPackingLine?.InnerBoxDimension,
                //            InnerBoxDimensionCode = remainPackingLine?.InnerBoxDimensionCode,
                //            Quantity = sizeRatio,
                //            Size = overQuantity.Size,
                //            QuantityPerCarton = totalQuantityPerCarton,
                //            TotalCarton = (int)(overQuantity.Quantity * totalRatio /
                //                                (totalQuantityPerCarton * sizeRatio)),
                //            QuantityPerPackage = totalRatio,
                //            QuantitySize = totalQuantityPerCarton * sizeRatio / totalRatio,
                //            PackagesPerBox = packingOverQuantities.Min(x => x.Quantity),
                //            Width = remainPackingLine?.Width,
                //            Height = remainPackingLine?.Height,
                //            Length = remainPackingLine?.Length,
                //            Color = overQuantity.ColorCode,
                //            InnerWidth = remainPackingLine?.InnerWidth,
                //            InnerHeight = remainPackingLine?.InnerHeight,
                //            InnerLength = remainPackingLine?.InnerLength,
                //            FromNo = CartonNo,
                //            ToNo = CartonNo + (int)((overQuantity.Quantity * totalRatio) /
                //                                        (totalQuantityPerCarton * sizeRatio)) - 1,
                //            DeliveryPlace = overQuantity.ItemStyle?.DeliveryPlace
                //        };

                //        if (packingLine.TotalQuantity == null)
                //            packingLine.TotalQuantity = 0;

                //        //packingLine.TotalQuantity = packingLine.TotalCarton * packingLine.QuantitySize;
                //        packingLine.NetWeight = netWeight;
                //        packingLine.GrossWeight = netWeight + packingLine?.BoxDimension?.Weight * packingLine.TotalCarton +
                //            (packingLine.InnerBoxDimension?.Weight * packingLine.PackagesPerBox *
                //            packingLine.TotalCarton ?? 0);

                //        localCartonNo = packingLine.TotalCarton ?? 0;

                //        newpackingLines.Add(packingLine);

                //    }
                //    total = packingOverQuantities.Sum(x => x.Quantity);

                //    foreach (var newpackingLine in newpackingLines)
                //    {
                //        if (newpackingLine.TotalQuantity == 0)
                //        {
                //            newpackingLine.TotalQuantity = total;
                //        }
                //    }
                //}
            }
            else
            {
                foreach (var currentOverQuantity in currentOverQuantities.OrderBy(x => x.SizeSortIndex))
                {
                    var currentPackingLine = packingLines
                        .FirstOrDefault(x => x.PrePack.Trim() == "Solid Size - Solid Color" && x.Size == currentOverQuantity.Size);
                    if (currentOverQuantity.Quantity == 0)
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
                            Quantity = 0,
                            Size = currentOverQuantity.Size,
                            QuantityPerCarton = 0,
                            QuantitySize = currentPackingLine?.QuantitySize,
                            TotalCarton = 0,
                            QuantityPerPackage = 0,
                            PackagesPerBox = 0,
                            Width = currentPackingLine?.Width,
                            Height = currentPackingLine?.Height,
                            Length = currentPackingLine?.Length,
                            InnerWidth = currentPackingLine?.InnerWidth,
                            InnerHeight = currentPackingLine?.InnerHeight,
                            InnerLength = currentPackingLine?.InnerLength,
                            Color = currentOverQuantity.ItemStyle.ColorCode,
                            FromNo = 0,
                            ToNo = 0,
                            DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                        };

                        packingLine.TotalQuantity = 0;
                        packingLine.NetWeight = 0;
                        packingLine.GrossWeight = 0;

                        newpackingLines.Add(packingLine);
                        SequenceNo += 1;
                    }
                    else
                    {
                        var totalQuantities = isUpdate ? currentPackingLine?.TotalQuantity : currentOverQuantity.Quantity;
                        int totalCarton = (int)(totalQuantities / currentPackingLine?.QuantitySize);
                        var netWeight = netWeights.FirstOrDefault(x => x.Size == currentOverQuantity.Size)?.NetWeight ?? 0;

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
                                FromNo = isRevised ? currentPackingLine?.FromNo : CartonNo,
                                ToNo = isRevised ? currentPackingLine?.ToNo : (CartonNo + (int)(totalQuantities / currentPackingLine.QuantitySize) - 1),
                                DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                            };

                            packingLine.TotalQuantity = packingLine.Quantity
                                * packingLine.TotalCarton;
                            packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                            packingLine.GrossWeight = packingLine.NetWeight + totalCarton * currentPackingLine?.BoxDimension.Weight;

                            CartonNo += (int)(totalQuantities / currentPackingLine.QuantitySize);
                            newpackingLines.Add(packingLine);
                            SequenceNo += 1;

                            //if ((int)(totalQuantities % currentPackingLine.QuantitySize) > 0)
                            //{
                            //    var remainPackingLine = packingLines?
                            //        .FirstOrDefault(x => x.PrePack.Trim() == "R" && x.Size == currentOverQuantity.Size) ?? currentPackingLine;
                            //    packingLine = new PackingLine()
                            //    {
                            //        SequenceNo = SequenceNo.ToString("d3"),
                            //        LSStyle = currentOverQuantity.ItemStyle.LSStyle,
                            //        PrePack = "R",
                            //        BoxDimension = remainPackingLine?.BoxDimension,
                            //        BoxDimensionCode = remainPackingLine?.BoxDimensionCode,
                            //        InnerBoxDimension = remainPackingLine?.InnerBoxDimension,
                            //        InnerBoxDimensionCode = remainPackingLine?.InnerBoxDimensionCode,
                            //        Quantity = (int)(totalQuantities % currentPackingLine.QuantitySize),
                            //        Size = currentOverQuantity.Size,
                            //        QuantityPerCarton = (int)(totalQuantities % currentPackingLine.QuantitySize),
                            //        QuantitySize = (int)(totalQuantities % currentPackingLine.QuantitySize),
                            //        TotalCarton = 1,
                            //        QuantityPerPackage = 0,
                            //        PackagesPerBox = 0,
                            //        Width = remainPackingLine?.Width,
                            //        Height = remainPackingLine?.Height,
                            //        Length = remainPackingLine?.Length,
                            //        InnerWidth = remainPackingLine?.InnerWidth,
                            //        InnerHeight = remainPackingLine?.InnerHeight,
                            //        InnerLength = remainPackingLine?.InnerLength,
                            //        Color = currentOverQuantity.ItemStyle.ColorCode,
                            //        FromNo = CartonNo,
                            //        ToNo = CartonNo,
                            //        DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                            //    };

                            //    packingLine.TotalQuantity = packingLine.Quantity * packingLine.TotalCarton;
                            //    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                            //    packingLine.GrossWeight = packingLine.NetWeight + totalCarton * remainPackingLine?.BoxDimension.Weight;

                            //    CartonNo += 1;
                            //    newpackingLines.Add(packingLine);
                            //    SequenceNo += 1;

                            //}
                        }
                    }
                }
            }

            return newpackingLines;
        }
    }
}
