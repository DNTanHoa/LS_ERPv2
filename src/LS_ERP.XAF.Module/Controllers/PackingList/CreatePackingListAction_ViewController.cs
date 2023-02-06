using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using LS_ERP.XAF.Module.Validators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class CreatePackingListAction_ViewController : ViewController
    {
        private IObjectSpace StyleNetWeightObjectSpace;
        private IObjectSpace packingListObjectSpace;
        private IObjectSpace itemStyleObjectSpace;
        private string itemStyleNumber = "";
        private Dictionary<string, int> shipQuantity = new Dictionary<string, int>();
        public CreatePackingListAction_ViewController()
        {
            InitializeComponent();

            /// Select style to add
            PopupWindowShowAction popupPackingItemStyle = new PopupWindowShowAction(this,
                "PopupPackingItemStyle", PredefinedCategory.Unspecified);
            popupPackingItemStyle.ImageName = "Actions_AddCircled";
            popupPackingItemStyle.Caption = "Add Style";
            popupPackingItemStyle.TargetObjectType = typeof(PackingListCreateParam);
            popupPackingItemStyle.TargetViewType = ViewType.DetailView;
            popupPackingItemStyle.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupPackingItemStyle.Shortcut = "CtrlShiftO";

            popupPackingItemStyle.CustomizePopupWindowParams += 
                PopupPackingItemStyle_CustomizePopupWindowParams;
            popupPackingItemStyle.Execute += PopupPackingItemStyle_Execute;

            ///  Netweight
            PopupWindowShowAction popupStyleNetWeight = new PopupWindowShowAction(this,
                "PopupStyleNetWeight", PredefinedCategory.Unspecified);
            popupStyleNetWeight.ImageName = "ShowWeightedLegend";
            popupStyleNetWeight.Caption = "Net Weight";
            popupStyleNetWeight.TargetObjectType = typeof(PackingListCreateParam);
            popupStyleNetWeight.TargetViewType = ViewType.DetailView;
            popupStyleNetWeight.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupStyleNetWeight.Shortcut = "CtrlShiftN";

            popupStyleNetWeight.Execute += PopupStyleNetWeight_Execute;
            popupStyleNetWeight.CustomizePopupWindowParams += 
                PopupStyleNetWeight_CustomizePopupWindowParams;

            /// Generate
            PopupWindowShowAction generatePacking = new PopupWindowShowAction(this,
                "GeneratePacking", PredefinedCategory.Unspecified);
            generatePacking.ImageName = "ChangeLegendPosition";
            generatePacking.Caption = "Generate";
            generatePacking.TargetObjectType = typeof(PackingListCreateParam);
            generatePacking.TargetViewType = ViewType.DetailView;
            generatePacking.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            generatePacking.Shortcut = "CtrlShiftG";

            generatePacking.CustomizePopupWindowParams += GeneratePacking_CustomizePopupWindowParams;
            generatePacking.Execute += GeneratePacking_Execute;
        }

        private void GeneratePacking_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var param = e.PopupWindowViewCurrentObject as PackingLineCreateParam;
            var viewObject = View.CurrentObject as PackingListCreateParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)View)
               .FindItem("PackingStyles") as ListPropertyEditor;
            var styles = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>();
            var criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                    "IN (" + string.Join(",", styles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
            var objectSpace = Application.CreateObjectSpace(typeof(StyleNetWeight));
            var styleNetWeights = objectSpace.GetObjects<StyleNetWeight>(criteria).ToList();
            var customerID = styles.ToList().First().SalesOrder.CustomerID;

            if (PackingLineCreateParamValidator.IsValid(param, customerID, out string errorMessage))
            {
                int CartonNo = viewObject.CartonNo;
                int LastNoCartonShortShip = viewObject.LastNoCartonShortShip;

                if (customerID.Trim().ToUpper() == "PU" || customerID.Trim().ToUpper() == "IFG")
                {
                    var packingLines = PackingLineProccess.Generate(param.PackingType,
                    viewObject.Dimension, viewObject.InnerDimension, param.TotalQuantity,
                    param.QuantityPackagePerCarton, ref CartonNo, styleNetWeights, param.OrderDetails, param.PackingRatios,
                    param.PackingOverQuantities, param.RemainQuantity, out List<PackingOverQuantity> overQuantities, ref LastNoCartonShortShip);

                    if (viewObject.PackingLines == null)
                        viewObject.PackingLines = new List<PackingLine>();

                    viewObject.PackingLines.AddRange(packingLines);

                    var newPackingStyles = new List<ItemStyle>();

                    foreach (var packingStyle in viewObject.PackingStyles)
                    {
                        packingStyle.PackingOverQuantities = overQuantities
                            .Where(x => x.ItemStyleNumber == packingStyle.Number)
                            .ToList();

                        newPackingStyles.Add(packingStyle);
                    }

                    viewObject.CartonNo = CartonNo;
                    viewObject.LastNoCartonShortShip = LastNoCartonShortShip;
                    viewObject.PackingStyles = new List<ItemStyle>(newPackingStyles);
                }
                else if(customerID.Trim().ToUpper() == "HM")
                {
                    if (param.PackingType == PackingType.SolidSizeAssortedColor)
                    {
                        param.PackingRatios.ForEach(x =>
                        {
                            if (x.TotalQuantity > 0)
                            {
                                x.Ratio = param.QuantityPackagePerCarton / viewObject.PackingStyles.Count;
                            }
                        });
                    }

                    var packingLines = HM_PackingLineProcess.Generate(param.PackingType, viewObject.Dimension, 
                        viewObject.InnerDimension, ref CartonNo, styleNetWeights, param.OrderDetails,param.PackingRatios, 
                        param.PackingOverQuantities, param.RemainQuantity, out List<PackingOverQuantity> overQuantities, param.QuantityPackagePerCarton);

                    if (viewObject.PackingLines == null)
                        viewObject.PackingLines = new List<PackingLine>();

                    viewObject.PackingLines.AddRange(packingLines);

                    var newPackingStyles = new List<ItemStyle>();

                    foreach (var packingStyle in viewObject.PackingStyles)
                    {
                        packingStyle.PackingOverQuantities = overQuantities
                            .Where(x => x.ItemStyleNumber == packingStyle.Number)
                            .ToList();

                        newPackingStyles.Add(packingStyle);
                    }

                    viewObject.CartonNo = CartonNo;
                    viewObject.PackingStyles = new List<ItemStyle>(newPackingStyles);
                }
                else if (customerID.Trim().ToUpper() == "GA")
                {
                    var packingLines = GA_PackingLineProcess.Generate(param.PackingType,
                    viewObject.Dimension, viewObject.InnerDimension, param.TotalQuantity,
                    param.QuantityPackagePerCarton, ref CartonNo, styleNetWeights, param.OrderDetails, param.PackingRatios,
                    param.PackingOverQuantities, param.RemainQuantity, out List<PackingOverQuantity> overQuantities);

                    if (viewObject.PackingLines == null)
                        viewObject.PackingLines = new List<PackingLine>();

                    viewObject.PackingLines.AddRange(packingLines);

                    var newPackingStyles = new List<ItemStyle>();

                    foreach (var packingStyle in viewObject.PackingStyles)
                    {
                        var packingOverQuantity = overQuantities
                                .Where(x => x.ItemStyleNumber == packingStyle.Number).ToList();
                        if(packingOverQuantity.Any())
                        {
                            packingStyle.PackingOverQuantities = packingOverQuantity;
                        }

                        newPackingStyles.Add(packingStyle);
                    }

                    viewObject.CartonNo = CartonNo;
                    viewObject.LastNoCartonShortShip = LastNoCartonShortShip;
                    viewObject.PackingStyles = new List<ItemStyle>(newPackingStyles);
                }
                else if (customerID.Trim().ToUpper() == "HA")
                {
                    var packingLines = HA_PackingLineProcess.Generate(param.PackingType,
                    viewObject.Dimension, viewObject.InnerDimension, param.TotalQuantity,
                    param.QuantityPackagePerCarton, ref CartonNo, styleNetWeights, param.OrderDetails, param.PackingRatios,
                    param.PackingOverQuantities, param.RemainQuantity, out List<PackingOverQuantity> overQuantities);

                    if (viewObject.PackingLines == null)
                        viewObject.PackingLines = new List<PackingLine>();

                    viewObject.PackingLines.AddRange(packingLines);

                    var newPackingStyles = new List<ItemStyle>();

                    foreach (var packingStyle in viewObject.PackingStyles)
                    {
                        packingStyle.PackingOverQuantities = overQuantities
                            .Where(x => x.ItemStyleNumber == packingStyle.Number)
                            .ToList();

                        newPackingStyles.Add(packingStyle);
                    }

                    viewObject.CartonNo = CartonNo;
                    viewObject.PackingStyles = new List<ItemStyle>(newPackingStyles);
                }
                else if (customerID.Trim().ToUpper() == "DE")
                {
                    var packingLines = DE_PackingLineProcess.GenerateManual(viewObject.Dimension, param.TotalQuantity,
                            styleNetWeights, param.OrderDetails, param.PackingRatios, out List < PackingOverQuantity > overQuantities);

                    if (viewObject.PackingLines == null)
                        viewObject.PackingLines = new List<PackingLine>();

                    viewObject.PackingLines.AddRange(packingLines);

                    var newPackingStyles = new List<ItemStyle>();

                    foreach (var packingStyle in viewObject.PackingStyles)
                    {
                        packingStyle.PackingOverQuantities = overQuantities
                            .Where(x => x.ItemStyleNumber == packingStyle.Number)
                            .ToList();

                        newPackingStyles.Add(packingStyle);
                    }

                    viewObject.CartonNo = CartonNo;
                    viewObject.LastNoCartonShortShip = LastNoCartonShortShip;
                    viewObject.PackingStyles = new List<ItemStyle>(newPackingStyles);
                }

                listPropertyEditor.ListView.Refresh();
                listPropertyEditor.ListView.EditView?.Refresh();
                View.Refresh();
            }
            else
            {
                var error = Message.GetMessageOptions(errorMessage, "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
            }
        }

        private void GeneratePacking_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as PackingListCreateParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)View)
               .FindItem("PackingStyles") as ListPropertyEditor;
            var itemStyle = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>().FirstOrDefault();
            var multiShip = itemStyle?.MultiShip ?? false;
            var customerID = itemStyle?.SalesOrder?.CustomerID ?? string.Empty;
            var sheetNameID = ObjectSpace.GetObjects<PackingSheetName>()
                    .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;

            var mappingConfig = new MapperConfiguration(c =>
            {
                c.CreateMap<OrderDetail, OrderDetailForPacking>()
                    .ForMember(x => x.ColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode))
                    .ForMember(x => x.ColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                    .ForMember(x => x.LSStyle, y => y.MapFrom(s => s.ItemStyle.LSStyle))
                    .ForMember(x => x.DeliveryPlace, y => y.MapFrom(s => s.ItemStyle.DeliveryPlace))
                    .ForMember(x => x.ItemStyleNumber, y => y.MapFrom(s => s.ItemStyle.Number))
                    .ForMember(x => x.SizeSortIndex, y => y.MapFrom(s => s.SizeSortIndex))
                    .ForMember(x => x.OrderDetailID, y => y.MapFrom(s => s.ID))
                    .ForMember(x => x.BalanceQuantity, y => y.MapFrom(s => s.ShipQuantity))
                    .ForMember(x => x.MultiShip, y => y.MapFrom(s => s.ItemStyle.MultiShip))
                    .ForMember(x => x.CustomerStyle, y => y.MapFrom(s => s.ItemStyle.CustomerStyle));
                c.CreateMap<OrderDetail, PackingRatio>()
                    .ForMember(x => x.Color, y => y.MapFrom(s => s.ItemStyle.ColorCode));
                c.CreateMap<OrderDetailForPacking, PackingOverQuantity>()
                    .ForMember(x => x.ColorCode, y => y.MapFrom(s => s.ColorCode))
                    .ForMember(x => x.ColorName, y => y.MapFrom(s => s.ColorName))
                    .ForMember(x => x.ItemStyleNumber, y => y.MapFrom(s => s.ItemStyleNumber))
                    .ForMember(x => x.Size, y => y.MapFrom(s => s.Size))
                    .ForMember(x => x.Quantity, y => y.MapFrom(s => s.Quantity))
                    .ForMember(x => x.SizeSortIndex, y => y.MapFrom(s => s.SizeSortIndex));
            });

            var mapper = mappingConfig.CreateMapper();
            var model = new PackingLineCreateParam();
            if (customerID.Trim().ToUpper() == "PU" || customerID.Trim().ToUpper() == "IFG")
            {
                model.OrderDetails = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>()
                            .SelectMany(x => x.OrderDetails).OrderBy(x => x.SizeSortIndex)
                            .Select(x => mapper.Map<OrderDetailForPacking>(x)).ToList();
                model.PackingOverQuantities = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.PackingOverQuantities)
                        .OrderBy(x => x.SizeSortIndex).ToList();
                model.PackingRatios = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.OrderDetails)
                        .Select(x => new PackingRatio()
                        {
                            Size = x.Size,
                            Color = x.ItemStyle,
                            Ratio = 0,
                            TotalQuantity = 0,
                            SizeSortIndex = x.SizeSortIndex
                        }).Distinct().ToList();
                
                if (multiShip)
                {
                    if (listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().FirstOrDefault().Number != itemStyleNumber)
                    {
                        var packingList = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>()
                                .SelectMany(x => x.PackingLists)?.ToList()
                                .Where(x => x.OrdinalShip == viewObject.OrdinalShip && (x.ParentPackingListID ?? 0) == 0)
                                .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                        var currentOverQuantities = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>().SelectMany(x => x.PackingOverQuantities).ToList() ?? new List<PackingOverQuantity>();

                        if (!currentOverQuantities.Any())
                        {
                            currentOverQuantities = model.OrderDetails
                                .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                            model.PackingOverQuantities = currentOverQuantities;
                            currentOverQuantities.ForEach(x =>
                            {
                                shipQuantity.Add(x.ItemStyleNumber + ";" + x.Size, (int)x.Quantity);
                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.BalanceQuantity = (int)x.Quantity;
                                    }
                                });
                            });
                        }
                        else if (packingList != null)
                        {
                            currentOverQuantities.ForEach(x =>
                            {
                                var packingLines = packingList.PackingLines.Where(y => y.Size == x.Size && y.LSStyle == x.ItemStyle?.LSStyle).ToList();
                                packingLines.ForEach(y =>
                                {
                                    if (x.Size == y.Size && y.PrePack.Trim() == "Assorted Size - Solid Color")
                                    {
                                        x.Quantity += (int)(y.QuantitySize * y.TotalCarton);
                                    }
                                    else if (x.Size == y.Size)
                                    {
                                        x.Quantity += (int)y.TotalQuantity;
                                    }
                                });

                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.ShipQuantity = (int)x.Quantity;
                                        d.BalanceQuantity = (int)x.Quantity;
                                        shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                    }
                                });
                            });
                            model.PackingOverQuantities = currentOverQuantities;
                        }
                        else
                        {
                            currentOverQuantities.ForEach(x =>
                            {
                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.ShipQuantity = (int)x.Quantity;
                                        d.BalanceQuantity = (int)x.Quantity;
                                        shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                    }
                                });
                            });
                            model.PackingOverQuantities = currentOverQuantities;
                        }
                        viewObject.RemainQuantity = shipQuantity;
                        itemStyleNumber = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().FirstOrDefault().Number;
                    }
                    else
                    {
                        model.OrderDetails.ForEach(x =>
                        {
                            x.ShipQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                            x.BalanceQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                        });
                    }
                }
            }
            else if(customerID.Trim().ToUpper() == "HM")
            {
                var colors = new ItemStyle();
                var itemStyles = listPropertyEditor.ListView?
                            .CollectionSource.List.Cast<ItemStyle>().ToList();
                var orderDetails = new List<OrderDetail>();
                var overQuantities = new List<PackingOverQuantity>();
                itemStyles.ForEach(x =>
                {
                    x.OrderDetails.ToList().ForEach(x =>
                    {
                        orderDetails.Add(x);
                    });

                    x.PackingOverQuantities.ToList().ForEach(x =>
                    {
                        overQuantities.Add(x);
                    });

                    colors.ColorName += string.IsNullOrEmpty(colors.ColorName) ? x.ColorName : " / " + x.ColorName;
                });
                model.OrderDetails = orderDetails
                    .Select(x => mapper.Map<OrderDetailForPacking>(x)).ToList();
                model.PackingOverQuantities = overQuantities;
                model.PackingRatios = itemStyles.FirstOrDefault()?.OrderDetails
                    .OrderBy(x => x.SizeSortIndex)
                    .Select(x => new PackingRatio()
                    {
                        Size = x.Size,
                        Color = colors,
                        Ratio = 0,
                        TotalQuantity = 0,
                        SizeSortIndex = x.SizeSortIndex
                    }).ToList();

                if (multiShip)
                {
                    if (!shipQuantity.Any())
                    {
                        packingListObjectSpace = Application.CreateObjectSpace(typeof(PackingList));
                        var packingList = packingListObjectSpace.GetObjects<PackingList>()
                            .Where(x => x.OrdinalShip == viewObject.OrdinalShip && x.LSStyles.Contains(itemStyle?.LSStyle))?
                            .OrderByDescending(x => x.PackingListDate)?.FirstOrDefault();

                        var currentOverQuantities = new List<PackingOverQuantity>();    
                        foreach (var data in itemStyles.ToList())
                        {
                            data.PackingOverQuantities.ToList()
                                .ForEach(x =>
                                {
                                    currentOverQuantities.Add(x);
                                });
                        }

                        if (!currentOverQuantities.Any())
                        {
                            currentOverQuantities = model.OrderDetails
                                .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                            model.PackingOverQuantities = currentOverQuantities;
                            currentOverQuantities.ForEach(x =>
                            {
                                shipQuantity.Add(x.ItemStyleNumber + ";" + x.Size, (int)x.Quantity);
                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.BalanceQuantity = (int)x.Quantity;
                                    }
                                });
                            });
                        }
                        else if (packingList != null)
                        {
                            currentOverQuantities.ForEach(x =>
                            {
                                var packingLines = packingList.PackingLines.Where(y => y.Size == x.Size && y.LSStyle == x.ItemStyle?.LSStyle).ToList();
                                packingLines.ForEach(y =>
                                {
                                    x.Quantity += (int)(y.QuantitySize * y.TotalCarton);
                                });

                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size && d.ItemStyleNumber == x.ItemStyleNumber)
                                    {
                                        d.ShipQuantity = (int)x.Quantity;
                                        d.BalanceQuantity = (int)x.Quantity;
                                        shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                    }
                                });
                            });
                            model.PackingOverQuantities = currentOverQuantities;
                        }
                        else
                        {
                            currentOverQuantities.ForEach(x =>
                            {
                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size && d.ItemStyleNumber == x.ItemStyleNumber)
                                    {
                                        d.ShipQuantity = (int)x.Quantity;
                                        d.BalanceQuantity = (int)x.Quantity;
                                        shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                    }
                                });
                            });
                            model.PackingOverQuantities = currentOverQuantities;
                        }
                        viewObject.RemainQuantity = shipQuantity;
                    }
                    else
                    {
                        model.OrderDetails.ForEach(x =>
                        {
                            x.ShipQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                            x.BalanceQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                        });
                    }
                }
            }
            else if (customerID.Trim().ToUpper() == "GA")
            {
                model.OrderDetails = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>()
                            .SelectMany(x => x.OrderDetails).OrderBy(x => x.SizeSortIndex)
                            .Select(x => mapper.Map<OrderDetailForPacking>(x)).ToList();
                model.PackingOverQuantities = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.PackingOverQuantities)
                        .OrderBy(x => x.SizeSortIndex).ToList();
                model.PackingRatios = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.OrderDetails)
                        .Where(x => x.ItemStyleNumber == itemStyle.Number)
                        .Select(x => new PackingRatio()
                        {
                            Size = x.Size,
                            Color = x.ItemStyle,
                            Ratio = 0,
                            TotalQuantity = 0,
                            SizeSortIndex = x.SizeSortIndex
                        }).Distinct().ToList();

                if (multiShip)
                {
                    var styles = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().ToList();
                    var list = shipQuantity.Select(x => x.Key.Split(";")[0]).Distinct().ToList();

                    foreach (var data in styles.OrderBy(x => x.LSStyle))
                    {
                        if (!list.Contains(data.Number))
                        {
                            var packingList = data.PackingLists?.ToList()
                                    .Where(x => x.OrdinalShip == viewObject.OrdinalShip &&
                                                (x.SheetNameID ?? 0) != sheetNameID &&
                                                (x.ParentPackingListID ?? 0) == 0)
                                    .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                            var currentOverQuantities = data.PackingOverQuantities
                                .Where(x => x.ItemStyleNumber == data.Number).ToList() ?? new List<PackingOverQuantity>();

                            if (!currentOverQuantities.Any() ||
                                itemStyle?.MultiShip != true)
                            {
                                currentOverQuantities = model.OrderDetails.Where(x => x.ItemStyleNumber == data.Number)
                                    .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                                model.PackingOverQuantities = currentOverQuantities;
                                currentOverQuantities.ForEach(x =>
                                {
                                    shipQuantity.Add(x.ItemStyleNumber + ";" + x.Size, (int)x.Quantity);
                                    model.OrderDetails.Where(x => x.ItemStyleNumber == data.Number).ToList()
                                    .ForEach(d =>
                                    {
                                        if (d.Size == x.Size && d.ItemStyleNumber == data.Number)
                                        {
                                            d.BalanceQuantity = (int)x.Quantity;
                                        }
                                    });
                                });
                            }
                            else if (packingList != null)
                            {
                                currentOverQuantities.ForEach(x =>
                                {
                                    var packingLines = packingList.PackingLines.Where(y => y.Size == x.Size && y.LSStyle == x.ItemStyle?.LSStyle).ToList();
                                    packingLines.ForEach(y =>
                                    {
                                        if (x.Size == y.Size && y.PrePack.Trim() == "Assorted Size - Solid Color")
                                        {
                                            x.Quantity += (int)(y.QuantitySize * y.TotalCarton);
                                        }
                                        else if (x.Size == y.Size)
                                        {
                                            x.Quantity += (int)y.TotalQuantity;
                                        }
                                    });

                                    model.OrderDetails.Where(x => x.ItemStyleNumber == data.Number).ToList()
                                    .ForEach(d =>
                                    {
                                        if (d.Size == x.Size && d.ItemStyleNumber == data.Number)
                                        {
                                            d.ShipQuantity = (int)x.Quantity;
                                            d.BalanceQuantity = (int)x.Quantity;
                                            shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                        }
                                    });
                                });
                                model.PackingOverQuantities = currentOverQuantities;
                            }
                            else
                            {
                                currentOverQuantities.ForEach(x =>
                                {
                                    model.OrderDetails.Where(x => x.ItemStyleNumber == data.Number).ToList()
                                    .ForEach(d =>
                                    {
                                        if (d.Size == x.Size && d.ItemStyleNumber == data.Number)
                                        {
                                            d.ShipQuantity = (int)x.Quantity;
                                            d.BalanceQuantity = (int)x.Quantity;
                                            shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                        }
                                    });
                                });
                                currentOverQuantities.ForEach(x =>
                                {
                                    model.PackingOverQuantities.Add(x);
                                });
                            }
                            viewObject.RemainQuantity = shipQuantity;
                            itemStyleNumber = data.Number;
                        }
                        else
                        {
                            model.OrderDetails.Where(x => x.ItemStyleNumber == data.Number).ToList()
                            .ForEach(x =>
                            {
                                x.ShipQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                                x.BalanceQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                            });
                        }
                    }
                }
            }
            else if (customerID.Trim().ToUpper() == "HA")
            {
                model.OrderDetails = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>()
                            .SelectMany(x => x.OrderDetails).OrderBy(x => x.SizeSortIndex)
                            .Select(x => mapper.Map<OrderDetailForPacking>(x)).ToList();
                model.PackingOverQuantities = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.PackingOverQuantities)
                        .OrderBy(x => x.SizeSortIndex).ToList();
                model.PackingRatios = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.OrderDetails)
                        .Select(x => new PackingRatio()
                        {
                            Size = x.Size,
                            Color = x.ItemStyle,
                            Ratio = 0,
                            TotalQuantity = 0,
                            SizeSortIndex = x.SizeSortIndex
                        }).Distinct().ToList();
            }
            else if (customerID.Trim().ToUpper() == "DE")
            {
                model.OrderDetails = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>()
                            .SelectMany(x => x.OrderDetails).OrderBy(x => x.SizeSortIndex)
                            .Select(x => mapper.Map<OrderDetailForPacking>(x)).ToList();
                model.PackingOverQuantities = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.PackingOverQuantities)
                        .OrderBy(x => x.SizeSortIndex).ToList();
                model.PackingRatios = listPropertyEditor.ListView?
                        .SelectedObjects.Cast<ItemStyle>()
                        .SelectMany(x => x.OrderDetails)
                        .Select(x => new PackingRatio()
                        {
                            Size = x.Size,
                            Color = x.ItemStyle,
                            Ratio = int.Parse(itemStyle?.Barcodes.Where(y => y.Size.Trim().ToUpper().Replace(" ","") 
                                                    == x.Size.Trim().ToUpper().Replace(" ", "")).FirstOrDefault().PCB),
                            TotalQuantity = 0,
                            SizeSortIndex = x.SizeSortIndex
                        }).Distinct().ToList();

                if (multiShip)
                {
                    if (listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().FirstOrDefault().Number != itemStyleNumber)
                    {
                        var packingList = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>()
                                .SelectMany(x => x.PackingLists)?.ToList()
                                .Where(x => x.OrdinalShip == viewObject.OrdinalShip &&
                                            (x.SheetNameID ?? 0) != sheetNameID)
                                .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                        var currentOverQuantities = listPropertyEditor.ListView?
                            .SelectedObjects.Cast<ItemStyle>().SelectMany(x => x.PackingOverQuantities).ToList() ?? new List<PackingOverQuantity>();

                        if (!currentOverQuantities.Any())
                        {
                            currentOverQuantities = model.OrderDetails
                                .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                            model.PackingOverQuantities = currentOverQuantities;
                            currentOverQuantities.ForEach(x =>
                            {
                                shipQuantity.Add(x.ItemStyleNumber + ";" + x.Size, (int)x.Quantity);
                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.BalanceQuantity = (int)x.Quantity;
                                    }
                                });
                            });
                        }
                        else if (packingList != null)
                        {
                            currentOverQuantities.ForEach(x =>
                            {
                                var packingLines = packingList.PackingLines.Where(y => y.Size == x.Size && y.LSStyle == x.ItemStyle?.LSStyle).ToList();
                                packingLines.ForEach(y =>
                                {
                                    if (x.Size == y.Size && y.PrePack.Trim() == "Assorted Size - Solid Color")
                                    {
                                        x.Quantity += (int)(y.QuantitySize * y.TotalCarton);
                                    }
                                    else if (x.Size == y.Size)
                                    {
                                        x.Quantity += (int)y.TotalQuantity;
                                    }
                                });

                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.ShipQuantity = (int)x.Quantity;
                                        d.BalanceQuantity = (int)x.Quantity;
                                        shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                    }
                                });
                            });
                            model.PackingOverQuantities = currentOverQuantities;
                        }
                        else
                        {
                            currentOverQuantities.ForEach(x =>
                            {
                                model.OrderDetails.ForEach(d =>
                                {
                                    if (d.Size == x.Size)
                                    {
                                        d.ShipQuantity = (int)x.Quantity;
                                        d.BalanceQuantity = (int)x.Quantity;
                                        shipQuantity.Add(x.ItemStyleNumber + ";" + d.Size, d.ShipQuantity);
                                    }
                                });
                            });
                            model.PackingOverQuantities = currentOverQuantities;
                        }
                        viewObject.RemainQuantity = shipQuantity;
                        itemStyleNumber = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().FirstOrDefault().Number;
                    }
                    else
                    {
                        model.OrderDetails.ForEach(x =>
                        {
                            x.ShipQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                            x.BalanceQuantity = shipQuantity[x.ItemStyleNumber + ";" + x.Size];
                        });
                    }
                }
            }

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        private void PopupStyleNetWeight_CustomizePopupWindowParams(object sender, 
            CustomizePopupWindowParamsEventArgs e)
        {
            StyleNetWeightObjectSpace = Application.CreateObjectSpace(typeof(StyleNetWeight));
            var viewObject = View.CurrentObject as PackingListCreateParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)View)
               .FindItem("PackingStyles") as ListPropertyEditor;

            if(viewObject != null &&
               listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().Any() == true)
            {
                var styles = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>();
                var criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                    "IN (" + string.Join(",", styles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                
                var model = new ViewStyleNetWeightParam();
                model.Weights = StyleNetWeightObjectSpace.GetObjects<StyleNetWeight>(criteria).ToList();

                var orderDetails = styles.FirstOrDefault().OrderDetails
                    .Where(x => !model.Weights.Select(w => w.Size).Contains(x.Size))
                    .OrderBy(x =>x.SizeSortIndex).ToList();

                orderDetails.ForEach(x =>
                {
                    var netWeight = StyleNetWeightObjectSpace.CreateObject<StyleNetWeight>();
                    netWeight.CustomerStyle = styles.First().CustomerStyle;
                    netWeight.Size = x.Size;
                    netWeight.CustomerID = styles.First().SalesOrder.CustomerID;
                    
                    model.Weights.Add(netWeight);
                });

                var view = Application.CreateDetailView(StyleNetWeightObjectSpace, model, false);
                e.DialogController.SaveOnAccept = false;
                e.View = view;
            }
        }

        private void PopupStyleNetWeight_Execute(object sender, 
            PopupWindowShowActionExecuteEventArgs e)
        {
            ListPropertyEditor listPropertyEditor = ((DetailView)View)
               .FindItem("PackingStyles") as ListPropertyEditor;

            if (listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().Any() == true)
            {
                var style = listPropertyEditor.ListView?.SelectedObjects.Cast<ItemStyle>().FirstOrDefault();
                var param = e.PopupWindowViewCurrentObject as ViewStyleNetWeightParam;
                var customer = style.SalesOrder.Customer;
                if(customer.ID == "IFG")
                {
                    itemStyleObjectSpace = Application.CreateObjectSpace(typeof(ItemStyle));
                    var criteria = CriteriaOperator.Parse("[ContractNo] " +
                                                "IN (" + string.Join(",", "'" + style.ContractNo + "'") + ")");
                    var customerStyles = itemStyleObjectSpace.GetObjects<ItemStyle>(criteria)
                                                .Select(x => x.CustomerStyle).Distinct().ToList();

                    foreach (var data in customerStyles)
                    { 
                        criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                                                "IN (" + string.Join(",", "'" + data + "'") + ")");
                        var netWeights = StyleNetWeightObjectSpace.GetObjects<StyleNetWeight>(criteria) ?? new List<StyleNetWeight>();

                        foreach (var weight in param.Weights)
                        {
                            if (string.IsNullOrEmpty(weight.CustomerStyle) 
                                    && netWeights.ToList().Find(y => y.Size == weight.Size) == null)
                            {
                                var netWeight = StyleNetWeightObjectSpace.CreateObject<StyleNetWeight>();
                                netWeight.CustomerStyle = data;
                                netWeight.CustomerID = customer.ID;
                                netWeight.Size = weight.Size;
                                netWeight.NetWeight = weight.NetWeight;
                            }

                            if (weight.ID == 0)
                            {
                                weight.SetCreateAudit(SecuritySystem.CurrentUserName);
                            }
                            else
                            {
                                weight.SetUpdateAudit(SecuritySystem.CurrentUserName);
                            }
                        }
                    }
                }
                else
                {
                    param.Weights.ForEach(x =>
                    {
                        if (string.IsNullOrEmpty(x.CustomerStyle))
                        {
                            x.CustomerStyle = style?.CustomerStyle;
                            x.CustomerID = customer.ID;
                        }

                        if (x.ID == 0)
                        {
                            x.SetCreateAudit(SecuritySystem.CurrentUserName);
                        }
                        else
                        {
                            x.SetUpdateAudit(SecuritySystem.CurrentUserName);
                        }
                    });
                }
                
                StyleNetWeightObjectSpace.CommitChanges();
                View.Refresh();
            }
        }

        private void PopupPackingItemStyle_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as PackingListCreateParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
               .FindItem("Styles") as ListPropertyEditor;
            if (viewObject.PackingStyles == null)
                viewObject.PackingStyles = new List<ItemStyle>();

            var newPackingStyles = viewObject.PackingStyles.ToList();
            newPackingStyles.AddRange(listPropertyEditor.ListView
                .SelectedObjects.Cast<ItemStyle>().ToList());
            viewObject.PackingStyles = newPackingStyles.ToList();
            View.Refresh(false);

            /// Warning style net weight 
            var newStyleValidators = listPropertyEditor.ListView
                .SelectedObjects.Cast<ItemStyle>().ToList();
            if(newStyleValidators.Any())
            {
                if (!PackingListCreateParamValidator.IsValid(newStyleValidators, objectSpace, out string errorMessage))
                {
                    var error = Message.GetMessageOptions(errorMessage, "Warning",
                        InformationType.Warning, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }
            }
        }

        private void PopupPackingItemStyle_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ItemStyleSearchParam()
            {
                Customer = ObjectSpace.GetObjects<Customer>().FirstOrDefault(),
                FromDate = DateTime.Today.AddMonths(-6),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}


