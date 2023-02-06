using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class DE_PackingLineProcess
    {
        public static List<PackingLine> Generate(List<StyleNetWeight> styleNetWeights,
            Dictionary<string, BoxDimension> boxDimensions,
            List<OrderDetail> orderDetails,
            List<ItemStyleBarCode> itemStyleBarCodes)
        {
            var packingLines = new List<PackingLine>();
            int SequenceNo = 0;

            foreach (var orderDetail in orderDetails.OrderBy(x => x.SizeSortIndex))
            {
                int CartonNo = 1;
                var barCode = itemStyleBarCodes.FirstOrDefault(x => x.Size.Trim().ToUpper().Replace(" ", "") == orderDetail.Size.Trim().ToUpper().Replace(" ", ""));
                decimal quantityCarton = 1;
                int totalCarton = 0;
                if (decimal.TryParse(barCode.PCB, out quantityCarton))
                {
                    totalCarton = (int)(barCode.Quantity / quantityCarton);
                }

                var styleNetWeight = styleNetWeights
                        .Where(x => x.Size.Trim().ToUpper().Replace(" ", "")
                                == barCode.Size.Trim().ToUpper().Replace(" ", "")).ToList();

                var dimension = boxDimensions[barCode.Packing.Replace(" ", "").Replace(".0", "")];

                decimal netWeight = 0;
                if(styleNetWeight.Find(x => x.GarmentColorCode == orderDetail.ItemStyle.ColorCode) != null)
                {
                    netWeight = (decimal)(styleNetWeight?.
                        FirstOrDefault(x => x.GarmentColorCode == orderDetail.ItemStyle.ColorCode).NetWeight ?? 0);
                }
                else
                {
                    netWeight = styleNetWeight?.FirstOrDefault()?.NetWeight ?? 0;
                }
                
                decimal grossWeight = 0;
                if (styleNetWeight?.Sum(x => x.BoxWeight ?? 0) == 0)
                    grossWeight =dimension?.Weight ?? 0;
                else
                {
                    styleNetWeight?.ForEach(w =>
                    {
                        if (!string.IsNullOrEmpty(w?.BoxDimensionCode))
                        {
                            if (w?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                == dimension?.Code?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", ""))
                                grossWeight = w?.BoxWeight ?? 0;
                        }
                    });

                    if (grossWeight == 0)
                        grossWeight = dimension?.Weight ?? 0;
                }

                if (totalCarton <= 0)
                {
                    var packingLine = new PackingLine()
                    {
                        SequenceNo = SequenceNo.ToString("d3"),
                        LSStyle = orderDetail.ItemStyle.LSStyle,
                        PrePack = "R",
                        BoxDimensionCode = dimension.Code,
                        Quantity = barCode.Quantity,
                        Size = orderDetail.Size,
                        QuantityPerCarton = barCode.Quantity,
                        QuantitySize = barCode.Quantity,
                        TotalCarton = 1,
                        QuantityPerPackage = barCode.Quantity / int.Parse(barCode.UE),
                        PackagesPerBox = int.Parse(barCode.UE),
                        Width = dimension?.Width,
                        Height = dimension?.Height,
                        Length = dimension?.Length,
                        Color = orderDetail.ItemStyle.ColorCode,
                        FromNo = CartonNo,
                        ToNo = 1,
                        DeliveryPlace = orderDetail.ItemStyle.DeliveryPlace,
                        TotalQuantity = 0,
                        NetWeight = 0,
                        GrossWeight = 0,
                    };
                    packingLine.TotalQuantity = packingLine.Quantity
                            * packingLine.TotalCarton;
                    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                    packingLine.GrossWeight = packingLine.NetWeight + totalCarton * grossWeight;

                    packingLines.Add(packingLine);
                    SequenceNo += 1;
                }
                else
                {
                    var packingLine = new PackingLine()
                    {
                        SequenceNo = SequenceNo.ToString("d3"),
                        LSStyle = orderDetail.ItemStyle.LSStyle,
                        PrePack = "Solid Size - Solid Color",
                        BoxDimensionCode = dimension.Code,
                        Quantity = quantityCarton,
                        Size = orderDetail.Size,
                        QuantityPerCarton = quantityCarton,
                        QuantitySize = quantityCarton,
                        TotalCarton = totalCarton,
                        QuantityPerPackage = quantityCarton / int.Parse(barCode.UE),
                        PackagesPerBox = int.Parse(barCode.UE),
                        Width = dimension?.Width,
                        Height = dimension?.Height,
                        Length = dimension?.Length,
                        Color = orderDetail.ItemStyle.ColorCode,
                        FromNo = CartonNo,
                        ToNo = CartonNo + totalCarton - 1,
                        DeliveryPlace = orderDetail.ItemStyle.DeliveryPlace
                    };

                    packingLine.TotalQuantity = packingLine.Quantity
                        * packingLine.TotalCarton;
                    packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                    packingLine.GrossWeight = packingLine.NetWeight + totalCarton * grossWeight;
                    packingLines.Add(packingLine);

                    CartonNo += totalCarton;
                    SequenceNo += 1;

                    ///Phần dư
                    var remainQuantity = (int)(barCode.Quantity % quantityCarton);
                    if (remainQuantity > 0)
                    {
                        packingLine = new PackingLine()
                        {
                            SequenceNo = SequenceNo.ToString("d3"),
                            LSStyle = orderDetail.ItemStyle.LSStyle,
                            PrePack = "R",
                            BoxDimensionCode = dimension.Code,
                            Quantity = remainQuantity,
                            Size = orderDetail.Size,
                            QuantityPerCarton = remainQuantity,
                            QuantitySize = remainQuantity,
                            TotalCarton = 1,
                            QuantityPerPackage = remainQuantity,
                            PackagesPerBox = 1,
                            Width = dimension?.Width,
                            Height = dimension?.Height,
                            Length = dimension?.Length,
                            Color = orderDetail.ItemStyle.ColorCode,
                            FromNo = CartonNo,
                            ToNo = CartonNo,
                            DeliveryPlace = orderDetail.ItemStyle.DeliveryPlace
                        };
                        packingLine.TotalQuantity = packingLine.Quantity
                            * packingLine.TotalCarton;
                        packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                        packingLine.GrossWeight = packingLine.NetWeight + packingLine.TotalCarton * grossWeight;
                        packingLines.Add(packingLine);

                        CartonNo += 1;
                        SequenceNo += 1;
                    }
                }
            }

            return packingLines;
        }

        public static List<PackingLine> GenerateManual(BoxDimension dimension,
            int totalQuantity, List<StyleNetWeight> styleNetWeights,
            List<OrderDetailForPacking> orderDetailForPackings,
            List<PackingRatio> ratios, out List<PackingOverQuantity> outOverQuantities)
        {
            var packingLines = new List<PackingLine>();
            outOverQuantities = new List<PackingOverQuantity>();
            int SequenceNo = 0;

            foreach (var orderDetail in orderDetailForPackings.Where(x => x.ShipQuantity > 0).OrderBy(x => x.SizeSortIndex))
            {
                int CartonNo = 1;
                int totalCarton = 0;
                var ratioQuantity = 1;
                if (totalQuantity > 0)
                {
                    ratioQuantity = totalQuantity;
                }
                else
                {
                    ratioQuantity = ratios.
                       FirstOrDefault(x => x.Size.Trim().ToUpper() == orderDetail.Size.Trim().ToUpper()).Ratio;
                }
                totalCarton = (int)(orderDetail.ShipQuantity / ratioQuantity);

                //var styleNetWeight = styleNetWeights
                //        .Where(x => x.Size.Trim().ToUpper().Replace(" ", "")
                //                == barCode.Size.Trim().ToUpper().Replace(" ", "")).ToList();
                //var dimension = boxDimensions[barCode.Packing.Replace(" ", "").Replace(".0", "")];
                //var netWeight = styleNetWeight?.FirstOrDefault()?.NetWeight ?? 0;

                var styleNetWeight = styleNetWeights
                        .Where(x => x.Size.Trim().ToUpper() == orderDetail.Size.Trim().ToUpper()).ToList();

                decimal netWeight = 0;
                if (styleNetWeight.Find(x => x.GarmentColorCode == orderDetail.ColorCode) != null)
                {
                    netWeight = (decimal)(styleNetWeight?.
                        FirstOrDefault(x => x.GarmentColorCode == orderDetail.ColorCode).NetWeight ?? 0);
                }
                else
                {
                    netWeight = styleNetWeight?.FirstOrDefault()?.NetWeight ?? 0;
                }

                decimal grossWeight = 0;
                if (styleNetWeight?.Sum(x => x.BoxWeight ?? 0) == 0)
                    grossWeight = dimension?.Weight ?? 0;
                else
                {
                    styleNetWeight?.ForEach(w =>
                    {
                        if (!string.IsNullOrEmpty(w?.BoxDimensionCode))
                        {
                            if (w?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                == dimension?.Code?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", ""))
                                grossWeight = w?.BoxWeight ?? 0;
                        }
                    });

                    if (grossWeight == 0)
                        grossWeight = dimension?.Weight ?? 0;
                }

                if (totalCarton > 0)
                    {
                        var packingLine = new PackingLine()
                        {
                            SequenceNo = SequenceNo.ToString("d3"),
                            LSStyle = orderDetail?.LSStyle,
                            PrePack = "Solid Size - Solid Color",
                            BoxDimensionCode = dimension.Code,
                            BoxDimension = dimension,
                            Quantity = ratioQuantity,
                            Size = orderDetail.Size,
                            QuantityPerCarton = ratioQuantity,
                            QuantitySize = ratioQuantity,
                            TotalCarton = totalCarton,
                            QuantityPerPackage = 0,
                            PackagesPerBox = 0,
                            Width = dimension?.Width,
                            Height = dimension?.Height,
                            Length = dimension?.Length,
                            Color = orderDetail?.ColorCode,
                            FromNo = CartonNo,
                            ToNo = CartonNo + totalCarton - 1,
                            DeliveryPlace = orderDetail?.DeliveryPlace
                        };

                        packingLine.TotalQuantity = packingLine.Quantity
                            * packingLine.TotalCarton;
                        packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                        packingLine.GrossWeight = packingLine.NetWeight + totalCarton * grossWeight;
                        packingLines.Add(packingLine);

                        CartonNo += totalCarton;
                        SequenceNo += 1;
                    }
                    ///Phần dư
                    var remainQuantity = (int)(orderDetail.ShipQuantity % ratioQuantity);
                    if (remainQuantity > 0)
                    {
                        var packingLine = new PackingLine()
                        {
                            SequenceNo = SequenceNo.ToString("d3"),
                            LSStyle = orderDetail?.LSStyle,
                            PrePack = "R",
                            BoxDimensionCode = dimension.Code,
                            BoxDimension = dimension,
                            Quantity = remainQuantity,
                            Size = orderDetail.Size,
                            QuantityPerCarton = remainQuantity,
                            QuantitySize = remainQuantity,
                            TotalCarton = 1,
                            QuantityPerPackage = 0,
                            PackagesPerBox = 0,
                            Width = dimension?.Width,
                            Height = dimension?.Height,
                            Length = dimension?.Length,
                            Color = orderDetail?.ColorCode,
                            FromNo = CartonNo,
                            ToNo = CartonNo,
                            DeliveryPlace = orderDetail?.DeliveryPlace
                        };
                        packingLine.TotalQuantity = packingLine.Quantity
                            * packingLine.TotalCarton;
                        packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                        packingLine.GrossWeight = packingLine.NetWeight + packingLine.TotalCarton * grossWeight;
                        packingLines.Add(packingLine);

                        CartonNo += 1;
                        SequenceNo += 1;
                    }

                    var packingOverQuantity = new PackingOverQuantity()
                    {
                        Quantity = 0,
                        ItemStyleNumber = orderDetail?.ItemStyleNumber,
                        ColorCode = orderDetail?.ColorCode,
                        ColorName = orderDetail?.ColorName,
                        Size = orderDetail?.Size,
                        SizeSortIndex = orderDetail?.SizeSortIndex
                    };

                    outOverQuantities.Add(packingOverQuantity);
                }

            return packingLines;
        }
    }
}
