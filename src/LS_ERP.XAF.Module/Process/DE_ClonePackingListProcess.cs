using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Process
{
    public class DE_ClonePackingListProcess
    {
        public static List<PackingLine> GeneratePacking(List<PackingLine> packingLines,
            List<PackingOverQuantity> currentOverQuantities, List<StyleNetWeight> netWeights, bool isUpdate)
        {
            
            var SequenceNo = 0;
            var newpackingLines = new List<PackingLine>();

            var packingOverQuantities = new List<PackingOverQuantity>();
            foreach (var currentOverQuantity in currentOverQuantities.OrderBy(x => x.SizeSortIndex))
            {
                var CartonNo = 1;
                var currentPackingLine = packingLines
                        .Where(x => x.Size == currentOverQuantity.Size).OrderBy(x => x.SequenceNo).FirstOrDefault();
                //var currentPackingLine = packingLines
                //    .FirstOrDefault(x => x.PrePack.Trim() == "Solid Size - Solid Color" && x.Size == currentOverQuantity.Size);
                if (currentPackingLine == null)
                {
                    continue;
                }
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

                    var styleNetWeight = netWeights.Where(x => x.Size == currentOverQuantity.Size).ToList();

                    decimal netWeight = 0;
                    if (styleNetWeight.Find(x => x.GarmentColorCode == currentOverQuantity.ItemStyle.ColorCode) != null)
                    {
                        netWeight = (decimal)(styleNetWeight?.
                            FirstOrDefault(x => x.GarmentColorCode == currentOverQuantity.ItemStyle.ColorCode).NetWeight ?? 0);
                    }
                    else
                    {
                        netWeight = styleNetWeight?.FirstOrDefault()?.NetWeight ?? 0;
                    }

                    decimal grossWeight = 0;
                    if (styleNetWeight?.Sum(x => x.BoxWeight ?? 0) == 0)
                        grossWeight = currentPackingLine?.BoxDimension?.Weight ?? 0;
                    else
                    {
                        styleNetWeight?.ForEach(w =>
                        {
                            if (!string.IsNullOrEmpty(w?.BoxDimensionCode))
                            {
                                if (w?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                    == currentPackingLine?.BoxDimension?.Code?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", ""))
                                    grossWeight = w?.BoxWeight ?? 0;
                            }
                        });

                        if (grossWeight == 0)
                            grossWeight = currentPackingLine?.BoxDimension?.Weight ?? 0;
                    }

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
                        packingLine.GrossWeight = packingLine.NetWeight + totalCarton * grossWeight;

                        CartonNo += (int)(totalQuantities / currentPackingLine.QuantitySize);
                        newpackingLines.Add(packingLine);
                        SequenceNo += 1;

                    }

                    if ((int)(totalQuantities % currentPackingLine.QuantitySize) > 0)
                    {
                        var remainPackingLine = packingLines?
                            .FirstOrDefault(x => x.PrePack.Trim() == "R" && x.Size == currentOverQuantity.Size) ?? currentPackingLine;
                        //grossWeight = styleNetWeight?.BoxWeight ?? remainPackingLine?.BoxDimension?.Weight ?? 0;
                        grossWeight = 0;
                        if (styleNetWeight?.Sum(x => x.BoxWeight ?? 0) == 0)
                            grossWeight = remainPackingLine?.BoxDimension?.Weight ?? 0;
                        else
                        {
                            styleNetWeight?.ForEach(w =>
                            {
                                if (!string.IsNullOrEmpty(w?.BoxDimensionCode))
                                {
                                    if (w?.BoxDimensionCode?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", "")
                                        == remainPackingLine?.BoxDimension?.Code?.Trim()?.ToLower().Replace("  ", "").Replace(" ", "").Replace("*", "").Replace("x", ""))
                                        grossWeight = w?.BoxWeight ?? 0;
                                }
                            });

                            if (grossWeight == 0)
                                grossWeight = remainPackingLine?.BoxDimension?.Weight ?? 0;
                        }

                        var packingLine = new PackingLine()
                        {
                            SequenceNo = SequenceNo.ToString("d3"),
                            LSStyle = currentOverQuantity.ItemStyle.LSStyle,
                            PrePack = "R",
                            BoxDimension = remainPackingLine?.BoxDimension,
                            BoxDimensionCode = remainPackingLine?.BoxDimensionCode,
                            InnerBoxDimension = remainPackingLine?.InnerBoxDimension,
                            InnerBoxDimensionCode = remainPackingLine?.InnerBoxDimensionCode,
                            Quantity = (int)(totalQuantities % currentPackingLine.QuantitySize),
                            Size = currentOverQuantity.Size,
                            QuantityPerCarton = (int)(totalQuantities % currentPackingLine.QuantitySize),
                            QuantitySize = (int)(totalQuantities % currentPackingLine.QuantitySize),
                            TotalCarton = 1,
                            QuantityPerPackage = 0,
                            PackagesPerBox = 0,
                            Width = remainPackingLine?.Width,
                            Height = remainPackingLine?.Height,
                            Length = remainPackingLine?.Length,
                            InnerWidth = remainPackingLine?.InnerWidth,
                            InnerHeight = remainPackingLine?.InnerHeight,
                            InnerLength = remainPackingLine?.InnerLength,
                            Color = currentOverQuantity.ItemStyle.ColorCode,
                            FromNo = CartonNo,
                            ToNo = CartonNo,
                            DeliveryPlace = currentOverQuantity.ItemStyle.DeliveryPlace
                        };

                        packingLine.TotalQuantity = packingLine.Quantity * packingLine.TotalCarton;
                        packingLine.NetWeight = packingLine.TotalQuantity * netWeight;
                        packingLine.GrossWeight = packingLine.NetWeight + packingLine.TotalCarton * grossWeight;

                        CartonNo += 1;
                        newpackingLines.Add(packingLine);
                        SequenceNo += 1;

                    }
                }
            }

            return newpackingLines;
        }
    }
}
