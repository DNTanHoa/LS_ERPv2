using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class HA_PackingLineProcess
    {
        public static List<PackingLine> Generate(PackingType type, BoxDimension dimension, BoxDimension innerDimension,
            int totalQuantity, int quantityPackagePercarton, ref int CartonNo, List<StyleNetWeight> styleNetWeights,
            List<OrderDetailForPacking> orderDetailForPackings, List<PackingRatio> ratios,
            List<PackingOverQuantity> overQuantities, bool IsRemain, out List<PackingOverQuantity> outOverQuantites)
        {
            var packingLines = new List<PackingLine>();
            var totalQuantitySize = 0;
            int SequenceNo = 0;

            if (type == PackingType.SolidSizeSolidColor && overQuantities?.Find(x => x.Quantity > 0) != null)
            {
                outOverQuantites = overQuantities ?? new List<PackingOverQuantity>();
            }
            else
            {
                outOverQuantites = new List<PackingOverQuantity>();
            }

            switch (type)
            {
                case PackingType.SolidSizeSolidColor:
                    if (!IsRemain)
                    {
                        if (totalQuantity > 0)
                        {
                            foreach (var orderDetailForPacking in orderDetailForPackings
                                .OrderBy(x => x.SizeSortIndex))
                            {
                                SequenceNo += 1;

                                int totalCarton = (int)(orderDetailForPacking.ShipQuantity / totalQuantity);
                                var netWeight = styleNetWeights
                                        .FirstOrDefault(x => x.Size == orderDetailForPacking.Size)?.NetWeight ?? 0;

                                if (totalCarton > 0)
                                {
                                    var packingLine = new PackingLine()
                                    {
                                        SequenceNo = SequenceNo.ToString("d3"),
                                        LSStyle = orderDetailForPacking.LSStyle,
                                        PrePack = "Solid Size - Solid Color",
                                        BoxDimension = dimension,
                                        BoxDimensionCode = dimension.Code,
                                        InnerBoxDimension = innerDimension,
                                        InnerBoxDimensionCode = innerDimension?.Code,
                                        Quantity = totalQuantity,
                                        Size = orderDetailForPacking.Size,
                                        QuantityPerCarton = totalQuantity,
                                        QuantitySize = totalQuantity,
                                        TotalCarton = (int)(orderDetailForPacking.ShipQuantity / totalQuantity),
                                        QuantityPerPackage = 0,
                                        PackagesPerBox = 0,
                                        Width = dimension?.Width,
                                        Height = dimension?.Height,
                                        Length = dimension?.Length,
                                        InnerWidth = innerDimension?.Width,
                                        InnerHeight = innerDimension?.Height,
                                        InnerLength = innerDimension?.Length,
                                        Color = orderDetailForPacking.ColorCode,
                                        FromNo = CartonNo,
                                        ToNo = CartonNo + (int)(orderDetailForPacking.ShipQuantity / totalQuantity) - 1,
                                        DeliveryPlace = orderDetailForPacking.DeliveryPlace
                                    };

                                    packingLine.TotalQuantity = packingLine.Quantity
                                        * packingLine.TotalCarton;
                                    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                    packingLine.GrossWeight = packingLine.NetWeight + totalCarton * dimension.Weight;

                                    CartonNo += (int)(orderDetailForPacking.ShipQuantity / totalQuantity);

                                    packingLines.Add(packingLine);
                                }

                                ///Phần dư
                                var packingOverQuantity = outOverQuantites
                                    .FirstOrDefault(x => x.ItemStyleNumber == orderDetailForPacking.ItemStyleNumber &&
                                                    x.Size == orderDetailForPacking.Size);
                                if (packingOverQuantity != null)
                                {
                                    packingOverQuantity.Quantity = orderDetailForPacking.ShipQuantity % totalQuantity;
                                }
                                else
                                {
                                    packingOverQuantity = new PackingOverQuantity()
                                    {
                                        Quantity = orderDetailForPacking.ShipQuantity % totalQuantity,
                                        ItemStyleNumber = orderDetailForPacking.ItemStyleNumber,
                                        ColorCode = orderDetailForPacking.ColorCode,
                                        ColorName = orderDetailForPacking.ColorName,
                                        Size = orderDetailForPacking.Size,
                                        SizeSortIndex = orderDetailForPacking.SizeSortIndex
                                    };

                                    outOverQuantites.Add(packingOverQuantity);
                                }
                            }
                        }
                        else
                        {
                            foreach (var orderDetailForPacking in orderDetailForPackings
                                .OrderBy(x => x.SizeSortIndex))
                            {
                                var packingOverQuantity = outOverQuantites
                                    .FirstOrDefault(x => x.ItemStyleNumber == orderDetailForPacking.ItemStyleNumber &&
                                                    x.Size == orderDetailForPacking.Size);

                                /// Get Ratio Size
                                totalQuantitySize = 0;
                                ratios.ForEach(x =>
                                {
                                    if (x.Size == orderDetailForPacking.Size && x.Ratio > 0)
                                    {
                                        totalQuantitySize = x.Ratio;
                                    }
                                });
                                if (totalQuantitySize == 0)
                                {
                                    if (packingOverQuantity == null)
                                    {
                                        packingOverQuantity = new PackingOverQuantity()
                                        {
                                            Quantity = orderDetailForPacking.ShipQuantity,
                                            ItemStyleNumber = orderDetailForPacking.ItemStyleNumber,
                                            ColorCode = orderDetailForPacking.ColorCode,
                                            ColorName = orderDetailForPacking.ColorName,
                                            Size = orderDetailForPacking.Size,
                                            SizeSortIndex = orderDetailForPacking.SizeSortIndex
                                        };

                                        outOverQuantites.Add(packingOverQuantity);
                                    }

                                    continue;
                                }

                                SequenceNo += 1;

                                int totalCarton = (int)(orderDetailForPacking.ShipQuantity / totalQuantitySize);
                                var netWeight = styleNetWeights
                                        .FirstOrDefault(x => x.Size == orderDetailForPacking.Size)?.NetWeight ?? 0;
                                
                                if (totalCarton > 0)
                                {
                                    var packingLine = new PackingLine()
                                    {
                                        SequenceNo = SequenceNo.ToString("d3"),
                                        LSStyle = orderDetailForPacking.LSStyle,
                                        PrePack = "Solid Size - Solid Color",
                                        BoxDimension = dimension,
                                        BoxDimensionCode = dimension.Code,
                                        InnerBoxDimension = innerDimension,
                                        InnerBoxDimensionCode = innerDimension?.Code,
                                        Quantity = totalQuantitySize,
                                        Size = orderDetailForPacking.Size,
                                        QuantityPerCarton = totalQuantitySize,
                                        QuantitySize = totalQuantitySize,
                                        TotalCarton = (int)(orderDetailForPacking.ShipQuantity / totalQuantitySize),
                                        QuantityPerPackage = 0,
                                        PackagesPerBox = 0,
                                        Width = dimension?.Width,
                                        Height = dimension?.Height,
                                        Length = dimension?.Length,
                                        InnerWidth = innerDimension?.Width,
                                        InnerHeight = innerDimension?.Height,
                                        InnerLength = innerDimension?.Length,
                                        Color = orderDetailForPacking.ColorCode,
                                        FromNo = CartonNo,
                                        ToNo = CartonNo + (int)(orderDetailForPacking.ShipQuantity / totalQuantitySize) - 1,
                                        DeliveryPlace = orderDetailForPacking.DeliveryPlace
                                    };

                                    packingLine.TotalQuantity = packingLine.Quantity
                                        * packingLine.TotalCarton;
                                    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                    packingLine.GrossWeight = packingLine.NetWeight + totalCarton * dimension.Weight;

                                    CartonNo += (int)(orderDetailForPacking.ShipQuantity / totalQuantitySize);

                                    packingLines.Add(packingLine);
                                }

                                ///Phần dư
                                if (packingOverQuantity != null)
                                {
                                    packingOverQuantity.Quantity = orderDetailForPacking.ShipQuantity % totalQuantitySize;
                                }
                                else
                                {
                                    packingOverQuantity = new PackingOverQuantity()
                                    {
                                        Quantity = orderDetailForPacking.ShipQuantity % totalQuantitySize,
                                        ItemStyleNumber = orderDetailForPacking.ItemStyleNumber,
                                        ColorCode = orderDetailForPacking.ColorCode,
                                        ColorName = orderDetailForPacking.ColorName,
                                        Size = orderDetailForPacking.Size,
                                        SizeSortIndex = orderDetailForPacking.SizeSortIndex
                                    };

                                    outOverQuantites.Add(packingOverQuantity);
                                }
                            }
                        }
                    }
                    else
                    {
                        bool firstSize = true;
                        if (ratios.Find(x => x.Ratio > 0) != null)
                        {
                            foreach (var overQuantity in overQuantities
                                .OrderBy(x => x.SizeSortIndex))
                            {
                                var netWeight = styleNetWeights
                                        .FirstOrDefault(x => x.Size == overQuantity.Size)?.NetWeight ?? 0;

                                totalQuantitySize = 0;
                                ratios.ForEach(x =>
                                {
                                    if (x.Size == overQuantity.Size && x.Ratio > 0)
                                    {
                                        totalQuantitySize = x.Ratio;
                                    }
                                });
                                if (totalQuantitySize == 0)
                                {
                                    continue;
                                }

                                if (overQuantity.Quantity > 0)
                                {
                                    var packingLine = new PackingLine()
                                    {
                                        SequenceNo = SequenceNo.ToString("d3"),
                                        LSStyle = overQuantity.ItemStyle?.LSStyle,
                                        PrePack = "R",
                                        BoxDimension = dimension,
                                        BoxDimensionCode = dimension.Code,
                                        InnerBoxDimension = innerDimension,
                                        InnerBoxDimensionCode = innerDimension?.Code,
                                        Quantity = overQuantity.Quantity,
                                        QuantitySize = overQuantity.Quantity,
                                        Size = overQuantity.Size,
                                        QuantityPerCarton = overQuantity.Quantity,
                                        TotalCarton = 1,
                                        QuantityPerPackage = 0,
                                        PackagesPerBox = 0,
                                        Width = dimension?.Width,
                                        Height = dimension?.Height,
                                        Length = dimension?.Length,
                                        InnerWidth = innerDimension?.Width,
                                        InnerHeight = innerDimension?.Height,
                                        InnerLength = innerDimension?.Length,
                                        Color = overQuantity.ColorCode,
                                        FromNo = CartonNo,
                                        ToNo = CartonNo,
                                        DeliveryPlace = overQuantity.ItemStyle?.DeliveryPlace
                                    };

                                    packingLine.TotalQuantity = overQuantity.Quantity;
                                    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                    packingLine.GrossWeight = packingLine.NetWeight + dimension.Weight;

                                    packingLines.Add(packingLine);

                                    CartonNo += 1;
                                }

                                overQuantity.Quantity = 0;
                            }

                            outOverQuantites = overQuantities;
                        }
                        else
                        {
                            foreach (var overQuantity in overQuantities
                                .OrderBy(x => x.SizeSortIndex))
                            {
                                var netWeight = styleNetWeights
                                        .FirstOrDefault(x => x.Size == overQuantity.Size)?.NetWeight ?? 0;

                                if (overQuantity.Quantity > 0)
                                {
                                    var packingLine = new PackingLine()
                                    {
                                        SequenceNo = SequenceNo.ToString("d3"),
                                        LSStyle = overQuantity.ItemStyle?.LSStyle,
                                        PrePack = "R",
                                        BoxDimension = dimension,
                                        BoxDimensionCode = dimension.Code,
                                        InnerBoxDimension = innerDimension,
                                        InnerBoxDimensionCode = innerDimension?.Code,
                                        Quantity = overQuantity.Quantity,
                                        QuantitySize = overQuantity.Quantity,
                                        Size = overQuantity.Size,
                                        QuantityPerCarton = overQuantity.Quantity,
                                        TotalCarton = 1,
                                        QuantityPerPackage = 0,
                                        PackagesPerBox = 0,
                                        Width = dimension?.Width,
                                        Height = dimension?.Height,
                                        Length = dimension?.Length,
                                        InnerWidth = innerDimension?.Width,
                                        InnerHeight = innerDimension?.Height,
                                        InnerLength = innerDimension?.Length,
                                        Color = overQuantity.ColorCode,
                                        FromNo = CartonNo,
                                        ToNo = CartonNo,
                                        DeliveryPlace = overQuantity.ItemStyle?.DeliveryPlace
                                    };

                                    packingLine.TotalQuantity = overQuantity.Quantity;
                                    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                                    packingLine.GrossWeight = packingLine.NetWeight + dimension.Weight;

                                    packingLines.Add(packingLine);

                                    CartonNo += 1;
                                }

                                overQuantity.Quantity = 0;
                            }

                            outOverQuantites = overQuantities;
                        }
                    }
                    break;
                case PackingType.AssortedSizeSolidColor:
                    if (!IsRemain)
                    {
                        var totalRatio = ratios.Sum(x => x.Ratio);
                        var totalQuantityPerCarton = totalRatio * quantityPackagePercarton;

                        int localCartonNo = 0;
                        decimal netWeight = 0;
                        decimal innerWeight = innerDimension?.Weight ?? 0;

                        foreach (var sizePacking in orderDetailForPackings)
                        {
                            var sizeRatio = ratios
                                .FirstOrDefault(x => x.Size == sizePacking.Size)?.Ratio ?? 0;
                            var styleNetWeight = styleNetWeights
                                .FirstOrDefault(x => x.Size == sizePacking.Size);

                            var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;

                            var totalCarton = (int)(sizePacking.ShipQuantity /
                                                    (totalQuantityPerCarton * sizeRatio /
                                                    (totalRatio)));

                            netWeight += styleNetWeight.NetWeight
                                * totalCarton * quantitySize ?? 0;
                        }

                        foreach (var orderDetailForPacking in orderDetailForPackings
                            .OrderBy(x => x.SizeSortIndex))
                        {
                            localCartonNo = CartonNo;

                            var sizeRatio = ratios
                                .FirstOrDefault(x => x.Size == orderDetailForPacking.Size)?.Ratio ?? 0;

                            var packingLine = new PackingLine()
                            {
                                SequenceNo = SequenceNo.ToString("d3"),
                                LSStyle = orderDetailForPacking.LSStyle,
                                PrePack = "Assorted Size - Solid Color",
                                BoxDimension = dimension,
                                BoxDimensionCode = dimension.Code,
                                InnerBoxDimension = innerDimension,
                                InnerBoxDimensionCode = innerDimension?.Code,
                                Quantity = sizeRatio,
                                Size = orderDetailForPacking.Size,
                                QuantityPerCarton = totalQuantityPerCarton,
                                TotalCarton = (int)(orderDetailForPacking.ShipQuantity /
                                                    (totalQuantityPerCarton * sizeRatio /
                                                    (totalRatio))),
                                QuantityPerPackage = totalRatio,
                                QuantitySize = totalQuantityPerCarton * sizeRatio / totalRatio,
                                PackagesPerBox = quantityPackagePercarton,
                                Width = dimension?.Width,
                                Height = dimension?.Height,
                                Length = dimension?.Length,
                                Color = orderDetailForPacking.ColorCode,
                                InnerWidth = innerDimension?.Width,
                                InnerHeight = innerDimension?.Height,
                                InnerLength = innerDimension?.Length,
                                FromNo = localCartonNo,
                                ToNo = localCartonNo + (int)(orderDetailForPacking.ShipQuantity /
                                                    (totalQuantityPerCarton * sizeRatio / (totalRatio))) - 1,
                                DeliveryPlace = orderDetailForPacking.DeliveryPlace
                            };

                            if (packingLine.TotalQuantity == null)
                                packingLine.TotalQuantity = 0;

                            packingLine.TotalQuantity = packingLine.TotalCarton * packingLine.QuantitySize;
                            packingLine.NetWeight = netWeight;
                            packingLine.GrossWeight = netWeight + dimension.Weight * packingLine.TotalCarton +
                                (packingLine.InnerBoxDimension?.Weight * packingLine.PackagesPerBox *
                                packingLine.TotalCarton ?? 0);

                            localCartonNo = packingLine.TotalCarton ?? 0;

                            packingLines.Add(packingLine);

                            ///Phần dư
                            var packingOverQuantity = new PackingOverQuantity()
                            {
                                Quantity = orderDetailForPacking.ShipQuantity %
                                    (quantityPackagePercarton * sizeRatio * packingLine.TotalCarton),
                                ItemStyleNumber = orderDetailForPacking.ItemStyleNumber,
                                ColorCode = orderDetailForPacking.ColorCode,
                                ColorName = orderDetailForPacking.ColorName,
                                Size = orderDetailForPacking.Size,
                                SizeSortIndex = orderDetailForPacking.SizeSortIndex
                            };

                            outOverQuantites.Add(packingOverQuantity);
                        }

                        CartonNo += localCartonNo;
                    }
                    else
                    {
                        var totalRatio = ratios.Sum(x => x.Ratio);
                        var totalQuantityPerCarton = totalRatio * quantityPackagePercarton;
                        int localCartonNo = 0;
                        decimal netWeight = 0;
                        decimal innerWeight = innerDimension?.Weight ?? 0;

                        foreach (var sizePacking in overQuantities)
                        {
                            var sizeRatio = ratios
                                .FirstOrDefault(x => x.Size == sizePacking.Size)?.Ratio ?? 0;
                            var styleNetWeight = styleNetWeights
                                .FirstOrDefault(x => x.Size == sizePacking.Size);

                            var totalCarton = (int)(sizePacking.Quantity /
                                                    (totalQuantityPerCarton * sizeRatio /
                                                    (totalRatio)));

                            var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;

                            netWeight += styleNetWeight.NetWeight
                                * totalCarton * quantitySize ?? 0;
                        }

                        foreach (var overQuantity in overQuantities
                            .OrderBy(x => x.SizeSortIndex))
                        {
                            var sizeRatio = ratios
                                .FirstOrDefault(x => x.Size == overQuantity.Size)?.Ratio ?? 0;

                            var packingLine = new PackingLine()
                            {
                                SequenceNo = SequenceNo.ToString("d3"),
                                LSStyle = overQuantity.ItemStyle?.LSStyle,
                                PrePack = "Assorted Size - Solid Color",
                                BoxDimension = dimension,
                                BoxDimensionCode = dimension.Code,
                                InnerBoxDimension = innerDimension,
                                InnerBoxDimensionCode = innerDimension?.Code,
                                Quantity = sizeRatio,
                                Size = overQuantity.Size,
                                QuantityPerCarton = totalQuantityPerCarton,
                                TotalCarton = (int)(overQuantity.Quantity * totalRatio /
                                                    (totalQuantityPerCarton * sizeRatio)),
                                QuantityPerPackage = totalRatio,
                                QuantitySize = totalQuantityPerCarton * sizeRatio / totalRatio,
                                PackagesPerBox = quantityPackagePercarton,
                                Width = dimension?.Width,
                                Height = dimension?.Height,
                                Length = dimension?.Length,
                                Color = overQuantity.ColorCode,
                                InnerWidth = innerDimension?.Width,
                                InnerHeight = innerDimension?.Height,
                                InnerLength = innerDimension?.Length,
                                FromNo = localCartonNo,
                                ToNo = localCartonNo + (int)((overQuantity.Quantity * totalRatio) /
                                                            (totalQuantityPerCarton * sizeRatio)) - 1,
                                DeliveryPlace = overQuantity.ItemStyle?.DeliveryPlace
                            };

                            if (packingLine.TotalQuantity == null)
                                packingLine.TotalQuantity = 0;

                            packingLine.TotalQuantity = packingLine.TotalCarton * packingLine.QuantitySize;
                            packingLine.NetWeight = netWeight;
                            packingLine.GrossWeight = netWeight + dimension.Weight * packingLine.TotalCarton +
                                (packingLine.InnerBoxDimension?.Weight * packingLine.PackagesPerBox *
                                packingLine.TotalCarton ?? 0);

                            localCartonNo = packingLine.TotalCarton ?? 0;

                            packingLines.Add(packingLine);

                            ///Phần dư
                            var packingOverQuantity = new PackingOverQuantity()
                            {
                                Quantity = 0,
                                ItemStyleNumber = overQuantity.ItemStyleNumber,
                                ColorCode = overQuantity.ColorCode,
                                ColorName = overQuantity.ColorName,
                                Size = overQuantity.Size,
                                SizeSortIndex = overQuantity.SizeSortIndex
                            };

                            outOverQuantites.Add(packingOverQuantity);
                        }

                        CartonNo += localCartonNo;
                    }

                    var total = packingLines.Sum(x => x.TotalQuantity);

                    foreach (var packingLine in packingLines)
                    {
                        packingLine.TotalQuantity = total;
                    }

                    break;
            }

            return packingLines;
        }
    }
}
