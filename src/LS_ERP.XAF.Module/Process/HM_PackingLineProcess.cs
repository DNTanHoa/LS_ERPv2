using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class HM_PackingLineProcess
    {
        public static List<PackingLine> Generate(PackingType type, BoxDimension dimension, BoxDimension innerDimension,
            ref int CartonNo, List<StyleNetWeight> styleNetWeights, List<OrderDetailForPacking> orderDetailForPackings,
            List<PackingRatio> ratios, List<PackingOverQuantity> overQuantities, bool IsRemain, out List<PackingOverQuantity> outOverQuantities, int quantityPackagePercarton)
        {
            var packingLines = new List<PackingLine>();
            int SequenceNo = 0;
            int totalCarton = 0;
            decimal totalNetWeight = 0;

            if(type == PackingType.AssortedSizeSolidColor)
            {
                outOverQuantities = new List<PackingOverQuantity>();
            }
            else
            {
                outOverQuantities = overQuantities ?? new List<PackingOverQuantity>();
            }
          
            switch (type)
            {
                case PackingType.SolidSizeAssortedColor:
                    if(orderDetailForPackings.FirstOrDefault().MultiShip != true)
                    {
                        if (!IsRemain) /// Đóng thùng chẵn
                        {
                            foreach (var ratio in ratios.OrderBy(x => x.SizeSortIndex))
                            {
                                totalNetWeight = 0;
                                var orderDetails = orderDetailForPackings.Where(x => x.Size == ratio.Size).ToList();
                                var checkQuantity = orderDetails.Find(x => x.ShipQuantity < (ratio.TotalQuantity / orderDetails.Count()));
                                if (checkQuantity != null || ratio.Ratio == 0)
                                {
                                    foreach (var orderDetail in orderDetails)
                                    {
                                        var packingOverQuantity = overQuantities
                                            .FirstOrDefault(x => x.ItemStyleNumber == orderDetail.ItemStyleNumber && x.Size == orderDetail.Size);

                                        if (packingOverQuantity != null)
                                        {
                                            packingOverQuantity.Quantity = orderDetail.ShipQuantity;
                                        }
                                        else
                                        {
                                            packingOverQuantity = new PackingOverQuantity();
                                            packingOverQuantity.Quantity = orderDetail.ShipQuantity;
                                            packingOverQuantity.ItemStyleNumber = orderDetail.ItemStyleNumber;
                                            packingOverQuantity.ColorCode = orderDetail.ColorCode;
                                            packingOverQuantity.ColorName = orderDetail.ColorName;
                                            packingOverQuantity.Size = orderDetail.Size;
                                            packingOverQuantity.SizeSortIndex = orderDetail.SizeSortIndex;

                                            outOverQuantities.Add(packingOverQuantity);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var orderDetail in orderDetails)
                                    {
                                        totalCarton = (int)(orderDetails.Sum(x => x.ShipQuantity) / ratio.TotalQuantity);
                                        var netWeight = styleNetWeights
                                                .FirstOrDefault(x => x.Size == orderDetail.Size)?.NetWeight ?? 0;
                                        totalNetWeight += (decimal)(totalCarton * (ratio.TotalQuantity / orderDetails.Count) * netWeight);
                                    }

                                    foreach (var orderDetail in orderDetails)
                                    {
                                        if (totalCarton > 0)
                                        {
                                            var packingLine = new PackingLine()
                                            {
                                                SequenceNo = SequenceNo.ToString("d3"),
                                                LSStyle = orderDetail.LSStyle,
                                                PrePack = "Solid Size - Assorted Color",
                                                BoxDimension = dimension,
                                                BoxDimensionCode = dimension.Code,
                                                InnerBoxDimension = innerDimension,
                                                InnerBoxDimensionCode = innerDimension?.Code,
                                                Quantity = ratio.Ratio,
                                                Size = orderDetail.Size,
                                                QuantityPerCarton = ratio.TotalQuantity,
                                                QuantitySize = (int)(ratio.TotalQuantity / orderDetails.Count),
                                                TotalCarton = totalCarton,
                                                QuantityPerPackage = ratio.Ratio * orderDetails.Count,
                                                PackagesPerBox = (int)(ratio.TotalQuantity / (ratio.Ratio * orderDetails.Count())),
                                                Width = dimension?.Width,
                                                Height = dimension?.Height,
                                                Length = dimension?.Length,
                                                InnerWidth = innerDimension?.Width,
                                                InnerHeight = innerDimension?.Height,
                                                InnerLength = innerDimension?.Length,
                                                Color = orderDetail.ColorCode,
                                                FromNo = CartonNo,
                                                ToNo = CartonNo + totalCarton - 1,
                                                DeliveryPlace = orderDetail.DeliveryPlace
                                            };

                                            packingLine.TotalQuantity = packingLine.QuantityPerCarton
                                                * packingLine.TotalCarton;
                                            packingLine.NetWeight = totalNetWeight;
                                            packingLine.GrossWeight = totalNetWeight + totalCarton * dimension.Weight;
                                            packingLines.Add(packingLine);

                                            /// Phần dư
                                            var packingOverQuantity = overQuantities
                                                .FirstOrDefault(x => x.ItemStyleNumber == orderDetail.ItemStyleNumber && x.Size == orderDetail.Size);

                                            if (packingOverQuantity != null)
                                            {
                                                packingOverQuantity.Quantity = (int)(orderDetail.ShipQuantity - (ratio.TotalQuantity
                                                                                    / orderDetails.Count()) * totalCarton);
                                            }
                                            else
                                            {
                                                packingOverQuantity = new PackingOverQuantity();
                                                packingOverQuantity.Quantity = (int)(orderDetail.ShipQuantity - (ratio.TotalQuantity
                                                                                    / orderDetails.Count()) * totalCarton);
                                                packingOverQuantity.ItemStyleNumber = orderDetail.ItemStyleNumber;
                                                packingOverQuantity.ColorCode = orderDetail.ColorCode;
                                                packingOverQuantity.ColorName = orderDetail.ColorName;
                                                packingOverQuantity.Size = orderDetail.Size;
                                                packingOverQuantity.SizeSortIndex = orderDetail.SizeSortIndex;

                                                outOverQuantities.Add(packingOverQuantity);
                                            }
                                        }
                                        SequenceNo += 1;
                                    }
                                    CartonNo += totalCarton;
                                }
                            }
                        }
                        else  /// Đóng phần dư 
                        {
                            totalNetWeight = 0;
                            foreach (var ratio in ratios.Where(x => x.Ratio > 0).OrderBy(x => x.SizeSortIndex))
                            {
                                var remainOverQuantities = overQuantities.Where(x => x.Size == ratio.Size).ToList();
                                var styleCount = remainOverQuantities.Count(x => x.Quantity > 0);

                                if (styleCount == remainOverQuantities.Count())
                                {
                                    foreach (var remainOverQuantity in remainOverQuantities)
                                    {
                                        var netWeight = styleNetWeights
                                                .FirstOrDefault(x => x.Size == remainOverQuantity.Size)?.NetWeight ?? 0;
                                        totalNetWeight += (decimal)((ratio.TotalQuantity / styleCount) * netWeight);

                                        var packingLine = new PackingLine()
                                        {
                                            SequenceNo = SequenceNo.ToString("d3"),
                                            LSStyle = remainOverQuantity.ItemStyle.LSStyle,
                                            PrePack = "Solid Size - Assorted Color",
                                            BoxDimension = dimension,
                                            BoxDimensionCode = dimension.Code,
                                            InnerBoxDimension = innerDimension,
                                            InnerBoxDimensionCode = innerDimension?.Code,
                                            Quantity = ratio.Ratio,
                                            Size = remainOverQuantity.Size,
                                            QuantityPerCarton = ratio.TotalQuantity,
                                            QuantitySize = (int)(ratio.TotalQuantity / remainOverQuantities.Count),
                                            TotalCarton = 1,
                                            QuantityPerPackage = ratio.Ratio * remainOverQuantities.Count,
                                            PackagesPerBox = (int)(ratio.TotalQuantity / (ratio.Ratio * remainOverQuantities.Count)),
                                            Width = dimension?.Width,
                                            Height = dimension?.Height,
                                            Length = dimension?.Length,
                                            InnerWidth = innerDimension?.Width,
                                            InnerHeight = innerDimension?.Height,
                                            InnerLength = innerDimension?.Length,
                                            Color = remainOverQuantity.ColorCode,
                                            FromNo = CartonNo,
                                            ToNo = CartonNo,
                                            DeliveryPlace = remainOverQuantity.ItemStyle.DeliveryPlace
                                        };

                                        //packingLine.TotalQuantity = packingLine.QuantityPerCarton
                                        //    * packingLine.TotalCarton; 
                                        packingLines.Add(packingLine);

                                        /// Phần dư
                                        var packingOverQuantity = overQuantities
                                            .First(x => x.ItemStyleNumber == remainOverQuantity.ItemStyleNumber && x.Size == remainOverQuantity.Size);

                                        packingOverQuantity.Quantity = (int)(remainOverQuantity.Quantity - (ratio.TotalQuantity / styleCount));
                                        SequenceNo += 1;
                                    }
                                }
                                //else
                                //{
                                //    foreach (var remainOverQuantity in remainOverQuantities.Where(x => x.Quantity > 0))
                                //    {
                                //        var netWeight = styleNetWeights
                                //                .FirstOrDefault(x => x.Size == remainOverQuantity.Size)?.NetWeight ?? 0;
                                //        totalNetWeight += (decimal)(remainOverQuantity.Quantity * netWeight);

                                //        var packingLine = new PackingLine()
                                //        {
                                //            SequenceNo = SequenceNo.ToString("d3"),
                                //            LSStyle = remainOverQuantity.ItemStyle.LSStyle,
                                //            PrePack = "Solid Size - Assorted Color",
                                //            BoxDimension = dimension,
                                //            BoxDimensionCode = dimension.Code,
                                //            InnerBoxDimension = innerDimension,
                                //            InnerBoxDimensionCode = innerDimension?.Code,
                                //            Quantity = ratio.Ratio,
                                //            Size = remainOverQuantity.Size,
                                //            QuantityPerCarton = remainOverQuantity.Quantity,
                                //            QuantitySize = remainOverQuantity.Quantity,
                                //            TotalCarton = 1,
                                //            QuantityPerPackage = remainOverQuantity.Quantity,
                                //            PackagesPerBox = 1,
                                //            Width = dimension?.Width,
                                //            Height = dimension?.Height,
                                //            Length = dimension?.Length,
                                //            InnerWidth = innerDimension?.Width,
                                //            InnerHeight = innerDimension?.Height,
                                //            InnerLength = innerDimension?.Length,
                                //            Color = remainOverQuantity.ColorCode,
                                //            FromNo = CartonNo,
                                //            ToNo = CartonNo,
                                //            DeliveryPlace = remainOverQuantity.ItemStyle.DeliveryPlace
                                //        };

                                //        //packingLine.TotalQuantity = remainOverQuantities.Sum(x => x.Quantity);
                                //        //packingLine.NetWeight = totalNetWeight;
                                //        //packingLine.GrossWeight = totalNetWeight + dimension.Weight;
                                //        packingLines.Add(packingLine);

                                //        /// Phần dư
                                //        var packingOverQuantity = overQuantities
                                //            .First(x => x.ItemStyleNumber == remainOverQuantity.ItemStyleNumber && x.Size == remainOverQuantity.Size);

                                //        packingOverQuantity.Quantity = 0;

                                //        SequenceNo += 1;
                                //    }
                                //}
                            }

                            /// Update net weight & gross weight & total quantity
                            foreach (var packingLine in packingLines)
                            {
                                packingLine.NetWeight = totalNetWeight;
                                packingLine.GrossWeight = totalNetWeight + dimension.Weight;
                                packingLine.TotalQuantity = packingLines.Sum(x => x.QuantitySize);
                            }
                            CartonNo += 1;
                        }
                    }
                    else
                    {
                        if (!IsRemain) /// Đóng thùng chẵn
                        {
                            foreach (var ratio in ratios.OrderBy(x => x.SizeSortIndex))
                            {
                                totalNetWeight = 0;
                                var orderDetails = orderDetailForPackings.Where(x => x.Size == ratio.Size).ToList();
                                var checkQuantity = orderDetails.Find(x => x.ShipQuantity < (ratio.TotalQuantity / orderDetails.Count()));
                                if (checkQuantity != null || ratio.Ratio == 0)
                                {
                                    foreach (var orderDetail in orderDetails.Where(o => o.ShipQuantity > 0))
                                    {
                                        var packingOverQuantity = overQuantities
                                            .FirstOrDefault(x => x.ItemStyleNumber == orderDetail.ItemStyleNumber && x.Size == orderDetail.Size);

                                        if (packingOverQuantity != null)
                                        {
                                            packingOverQuantity.Quantity = orderDetail.ShipQuantity;
                                        }
                                        else
                                        {
                                            packingOverQuantity = new PackingOverQuantity();
                                            packingOverQuantity.Quantity = orderDetail.ShipQuantity;
                                            packingOverQuantity.ItemStyleNumber = orderDetail.ItemStyleNumber;
                                            packingOverQuantity.ColorCode = orderDetail.ColorCode;
                                            packingOverQuantity.ColorName = orderDetail.ColorName;
                                            packingOverQuantity.Size = orderDetail.Size;
                                            packingOverQuantity.SizeSortIndex = orderDetail.SizeSortIndex;

                                            outOverQuantities.Add(packingOverQuantity);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var orderDetail in orderDetails)
                                    {
                                        totalCarton = (int)(orderDetails.Sum(x => x.ShipQuantity) / ratio.TotalQuantity);
                                        var netWeight = styleNetWeights
                                                .FirstOrDefault(x => x.Size == orderDetail.Size)?.NetWeight ?? 0;
                                        totalNetWeight += (decimal)(totalCarton * (ratio.TotalQuantity / orderDetails.Count) * netWeight);
                                    }

                                    foreach (var orderDetail in orderDetails)
                                    {
                                        if (totalCarton > 0)
                                        {
                                            var packingLine = new PackingLine()
                                            {
                                                SequenceNo = SequenceNo.ToString("d3"),
                                                LSStyle = orderDetail.LSStyle,
                                                PrePack = "Solid Size - Assorted Color",
                                                BoxDimension = dimension,
                                                BoxDimensionCode = dimension.Code,
                                                InnerBoxDimension = innerDimension,
                                                InnerBoxDimensionCode = innerDimension?.Code,
                                                Quantity = ratio.Ratio,
                                                Size = orderDetail.Size,
                                                QuantityPerCarton = ratio.TotalQuantity,
                                                QuantitySize = (int)(ratio.TotalQuantity / orderDetails.Count),
                                                TotalCarton = totalCarton,
                                                QuantityPerPackage = ratio.Ratio * orderDetails.Count,
                                                PackagesPerBox = (int)(ratio.TotalQuantity / (ratio.Ratio * orderDetails.Count())),
                                                Width = dimension?.Width,
                                                Height = dimension?.Height,
                                                Length = dimension?.Length,
                                                InnerWidth = innerDimension?.Width,
                                                InnerHeight = innerDimension?.Height,
                                                InnerLength = innerDimension?.Length,
                                                Color = orderDetail.ColorCode,
                                                FromNo = CartonNo,
                                                ToNo = CartonNo + totalCarton - 1,
                                                DeliveryPlace = orderDetail.DeliveryPlace
                                            };

                                            packingLine.TotalQuantity = packingLine.QuantityPerCarton
                                                * packingLine.TotalCarton;
                                            packingLine.NetWeight = totalNetWeight;
                                            packingLine.GrossWeight = totalNetWeight + totalCarton * dimension.Weight;
                                            packingLines.Add(packingLine);

                                            /// Phần dư
                                            var packingOverQuantity = overQuantities
                                                .FirstOrDefault(x => x.ItemStyleNumber == orderDetail.ItemStyleNumber && x.Size == orderDetail.Size);

                                            if (packingOverQuantity != null)
                                            {
                                                packingOverQuantity.Quantity = (int)(orderDetail.ShipQuantity - (ratio.TotalQuantity
                                                                                    / orderDetails.Count()) * totalCarton);
                                            }
                                            else
                                            {
                                                packingOverQuantity = new PackingOverQuantity();
                                                packingOverQuantity.Quantity = (int)(orderDetail.ShipQuantity - (ratio.TotalQuantity
                                                                                    / orderDetails.Count()) * totalCarton);
                                                packingOverQuantity.ItemStyleNumber = orderDetail.ItemStyleNumber;
                                                packingOverQuantity.ColorCode = orderDetail.ColorCode;
                                                packingOverQuantity.ColorName = orderDetail.ColorName;
                                                packingOverQuantity.Size = orderDetail.Size;
                                                packingOverQuantity.SizeSortIndex = orderDetail.SizeSortIndex;

                                                outOverQuantities.Add(packingOverQuantity);
                                            }
                                        }
                                        SequenceNo += 1;
                                    }
                                    CartonNo += totalCarton;
                                }
                            }
                        }
                        else  /// Đóng phần dư 
                        {
                            totalNetWeight = 0;
                            foreach (var ratio in ratios.Where(x => x.Ratio > 0).OrderBy(x => x.SizeSortIndex))
                            {
                                var remainOverQuantities = overQuantities.Where(x => x.Size == ratio.Size).ToList();
                                var styleCount = remainOverQuantities.Count(x => x.Quantity > 0);

                                if (styleCount == remainOverQuantities.Count())
                                {
                                    foreach (var remainOverQuantity in remainOverQuantities)
                                    {
                                        var netWeight = styleNetWeights
                                                .FirstOrDefault(x => x.Size == remainOverQuantity.Size)?.NetWeight ?? 0;
                                        totalNetWeight += (decimal)((ratio.TotalQuantity / styleCount) * netWeight);

                                        var packingLine = new PackingLine()
                                        {
                                            SequenceNo = SequenceNo.ToString("d3"),
                                            LSStyle = remainOverQuantity.ItemStyle.LSStyle,
                                            PrePack = "Solid Size - Assorted Color",
                                            BoxDimension = dimension,
                                            BoxDimensionCode = dimension.Code,
                                            InnerBoxDimension = innerDimension,
                                            InnerBoxDimensionCode = innerDimension?.Code,
                                            Quantity = ratio.Ratio,
                                            Size = remainOverQuantity.Size,
                                            QuantityPerCarton = ratio.TotalQuantity,
                                            QuantitySize = (int)(ratio.TotalQuantity / remainOverQuantities.Count),
                                            TotalCarton = 1,
                                            QuantityPerPackage = ratio.Ratio * remainOverQuantities.Count,
                                            PackagesPerBox = (int)(ratio.TotalQuantity / (ratio.Ratio * remainOverQuantities.Count)),
                                            Width = dimension?.Width,
                                            Height = dimension?.Height,
                                            Length = dimension?.Length,
                                            InnerWidth = innerDimension?.Width,
                                            InnerHeight = innerDimension?.Height,
                                            InnerLength = innerDimension?.Length,
                                            Color = remainOverQuantity.ColorCode,
                                            FromNo = CartonNo,
                                            ToNo = CartonNo,
                                            DeliveryPlace = remainOverQuantity.ItemStyle.DeliveryPlace
                                        };

                                        //packingLine.TotalQuantity = packingLine.QuantityPerCarton
                                        //    * packingLine.TotalCarton; 
                                        packingLines.Add(packingLine);

                                        /// Phần dư
                                        var packingOverQuantity = overQuantities
                                            .First(x => x.ItemStyleNumber == remainOverQuantity.ItemStyleNumber && x.Size == remainOverQuantity.Size);

                                        packingOverQuantity.Quantity = (int)(remainOverQuantity.Quantity - (ratio.TotalQuantity / styleCount));
                                        SequenceNo += 1;
                                    }
                                }
                                //else
                                //{
                                //    foreach (var remainOverQuantity in remainOverQuantities.Where(x => x.Quantity > 0))
                                //    {
                                //        var netWeight = styleNetWeights
                                //                .FirstOrDefault(x => x.Size == remainOverQuantity.Size)?.NetWeight ?? 0;
                                //        totalNetWeight += (decimal)(remainOverQuantity.Quantity * netWeight);

                                //        var packingLine = new PackingLine()
                                //        {
                                //            SequenceNo = SequenceNo.ToString("d3"),
                                //            LSStyle = remainOverQuantity.ItemStyle.LSStyle,
                                //            PrePack = "Solid Size - Assorted Color",
                                //            BoxDimension = dimension,
                                //            BoxDimensionCode = dimension.Code,
                                //            InnerBoxDimension = innerDimension,
                                //            InnerBoxDimensionCode = innerDimension?.Code,
                                //            Quantity = ratio.Ratio,
                                //            Size = remainOverQuantity.Size,
                                //            QuantityPerCarton = remainOverQuantity.Quantity,
                                //            QuantitySize = remainOverQuantity.Quantity,
                                //            TotalCarton = 1,
                                //            QuantityPerPackage = remainOverQuantity.Quantity,
                                //            PackagesPerBox = 1,
                                //            Width = dimension?.Width,
                                //            Height = dimension?.Height,
                                //            Length = dimension?.Length,
                                //            InnerWidth = innerDimension?.Width,
                                //            InnerHeight = innerDimension?.Height,
                                //            InnerLength = innerDimension?.Length,
                                //            Color = remainOverQuantity.ColorCode,
                                //            FromNo = CartonNo,
                                //            ToNo = CartonNo,
                                //            DeliveryPlace = remainOverQuantity.ItemStyle.DeliveryPlace
                                //        };

                                //        //packingLine.TotalQuantity = remainOverQuantities.Sum(x => x.Quantity);
                                //        //packingLine.NetWeight = totalNetWeight;
                                //        //packingLine.GrossWeight = totalNetWeight + dimension.Weight;
                                //        packingLines.Add(packingLine);

                                //        /// Phần dư
                                //        var packingOverQuantity = overQuantities
                                //            .First(x => x.ItemStyleNumber == remainOverQuantity.ItemStyleNumber && x.Size == remainOverQuantity.Size);

                                //        packingOverQuantity.Quantity = 0;

                                //        SequenceNo += 1;
                                //    }
                                //}
                            }

                            /// Update net weight & gross weight & total quantity
                            foreach (var packingLine in packingLines)
                            {
                                packingLine.NetWeight = totalNetWeight;
                                packingLine.GrossWeight = totalNetWeight + dimension.Weight;
                                packingLine.TotalQuantity = packingLines.Sum(x => x.QuantitySize);
                            }
                            CartonNo += 1;
                        }
                    }
                    
                    break;

                case PackingType.AssortedSizeSolidColor:
                    if (!IsRemain)
                    {
                        var totalRatio = ratios.Where(x => x.Ratio > 0).Sum(x => x.Ratio);
                        var totalQuantityPerCarton = totalRatio * quantityPackagePercarton;

                        int localCartonNo = 0;
                        decimal netWeight = 0;
                        decimal innerWeight = innerDimension?.Weight ?? 0;

                        foreach (var sizePacking in orderDetailForPackings)
                        {
                            var sizeRatio = ratios
                                .FirstOrDefault(x => x.Size == sizePacking.Size)?.Ratio ?? 0;
                            if (sizeRatio <= 0)
                            {
                                continue;
                            }

                            var styleNetWeight = styleNetWeights
                                .FirstOrDefault(x => x.Size == sizePacking.Size);

                            var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;

                            totalCarton = (int)(sizePacking.ShipQuantity /
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
                            if (sizeRatio <= 0)
                            {
                                continue;
                            }

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

                            outOverQuantities.Add(packingOverQuantity);
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
                            if (sizeRatio <= 0)
                            {
                                continue;
                            }

                            var styleNetWeight = styleNetWeights
                                .FirstOrDefault(x => x.Size == sizePacking.Size);

                            totalCarton = (int)(sizePacking.Quantity /
                                                    (totalQuantityPerCarton * sizeRatio /
                                                    (totalRatio)));

                            var quantitySize = totalQuantityPerCarton * sizeRatio / totalRatio;

                            netWeight += styleNetWeight.NetWeight
                                * totalCarton * quantitySize ?? 0;
                        }

                        foreach (var overQuantity in overQuantities
                            .OrderBy(x => x.SizeSortIndex))
                        {
                            localCartonNo = CartonNo;

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

                            outOverQuantities.Add(packingOverQuantity);
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

