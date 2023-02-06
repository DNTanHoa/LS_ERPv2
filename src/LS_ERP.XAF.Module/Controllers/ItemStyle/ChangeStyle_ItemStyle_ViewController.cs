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
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ChangeStyle_ItemStyle_ViewController : ViewController
    {
        string contractNo = "";
        string styleList = "";
        int styleCount = 0;
        bool checkIFG = true;
        List<string> ifgStyles = new List<string>();
        Dictionary<string,string> styleMap = new Dictionary<string, string>();
        List<PackingLine> ifgPackingLines = new List<PackingLine>();
        int cartonNo = 1;
        int SequenceNo = 0;
        public ChangeStyle_ItemStyle_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupChangeItemStyleAction = new PopupWindowShowAction(this,
                "ChangeItemStyleAction", PredefinedCategory.Unspecified);
            popupChangeItemStyleAction.ImageName = "Actions_AddCircled";
            popupChangeItemStyleAction.Caption = "Add Style";
            popupChangeItemStyleAction.TargetObjectType = typeof(ItemStyle);
            popupChangeItemStyleAction.TargetViewType = ViewType.ListView;
            popupChangeItemStyleAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupChangeItemStyleAction.Shortcut = "CtrlShiftO";

            popupChangeItemStyleAction.CustomizePopupWindowParams += PopupChangeItemStyleAction_CustomizePopupWindowParams;
            popupChangeItemStyleAction.Execute += PopupChangeItemStyleAction_Execute;
        }

        private void PopupChangeItemStyleAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;
            
            if (viewObject.CustomerID == "IFG")
            {
                if (checkIFG)
                {
                    checkIFG = false;
                    ifgStyles = viewObject.PackingLines.OrderBy(x => x.SequenceNo).Select(x => x.LSStyle).Distinct().ToList();
                    ifgPackingLines = viewObject.PackingLines.ToList();
                    viewObject.PackingLines = new List<PackingLine>();
                }
                
                if (string.IsNullOrEmpty(contractNo.Trim()))
                {
                    contractNo = viewObject.LSStyles.Trim();
                }
            }
            else if (viewObject.CustomerID == "GA")
            {
                if(string.IsNullOrEmpty(styleList.Trim()))
                {
                    styleList = viewObject.LSStyles.Trim();
                    styleCount = viewObject.PackingLines.Select(x => x.LSStyle).Distinct().Count();
                }
            }

            var model = new ItemStyleSearchParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
                FromDate = DateTime.Today.AddMonths(-6),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }

        private void PopupChangeItemStyleAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var propertyCollectionSource = (View as ListView)?.CollectionSource as PropertyCollectionSource;
            var packingList = propertyCollectionSource?.MasterObject as PackingList;
            var errorMessage = "";

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
               .FindItem("Styles") as ListPropertyEditor;
            var sheetNameID = ObjectSpace.GetObjects<PackingSheetName>()
                    .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;

            if(packingList.CustomerID == "IFG")
            {
                var newItemStyles = packingList?.ItemStyles.ToList() ?? new List<ItemStyle>();
                newItemStyles.AddRange(listPropertyEditor.ListView
                    .SelectedObjects.Cast<ItemStyle>().ToList() ?? new List<ItemStyle>());

                if (newItemStyles.Any() && (newItemStyles.First()?.ContractNo ?? string.Empty).Trim() == contractNo)
                {
                    if(newItemStyles.Count <= ifgStyles.Count)
                    { 
                        var newStyles = new List<ItemStyle>();
                        var i = 0;
                        var fistStyle = true;
                        foreach(var data in newItemStyles.Select(x => x.LSStyle))
                        {
                            if(!styleMap.TryGetValue(data, out string value))
                            { 
                                styleMap.Add(newItemStyles[i].LSStyle, ifgStyles[i]);
                                if (i == 0)
                                    packingList.LSStyles = newItemStyles[i].LSStyle;
                                else
                                    packingList.LSStyles += ";" + newItemStyles[i].LSStyle;
                            }

                            newStyles.Add(newItemStyles[i]);
                            i++;
                        }
                        foreach(var itemStyle in newStyles)
                        {
                            var netWeights = objectSpace.GetObjects<StyleNetWeight>()
                                        .Where(x => x.CustomerStyle == itemStyle.CustomerStyle).ToList();
                            //var itemStyle = newItemStyles.First();

                            if (ifgPackingLines.Any())
                            {
                                var packingOverQuantities = new List<PackingOverQuantity>();

                                var mappingConfig = new MapperConfiguration(c =>
                                {
                                    c.CreateMap<OrderDetail, PackingOverQuantity>()
                                        .ForMember(x => x.ColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode))
                                        .ForMember(x => x.ColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                                        .ForMember(x => x.ItemStyleNumber, y => y.MapFrom(s => s.ItemStyleNumber))
                                        .ForMember(x => x.Size, y => y.MapFrom(s => s.Size))
                                        .ForMember(x => x.Quantity, y => y.MapFrom(s => s.Quantity))
                                        .ForMember(x => x.SizeSortIndex, y => y.MapFrom(s => s.SizeSortIndex))
                                        .ForMember(x => x.ID, y => y.Ignore());
                                });
                                var mapper = mappingConfig.CreateMapper();
                                packingOverQuantities = objectSpace.GetObjects<PackingOverQuantity>()
                                    .Where(x => x.ItemStyleNumber == itemStyle.Number).ToList();

                                var revisePackingList = objectSpace.GetObjects<PackingList>()
                                    .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle?.LSStyle) &&
                                                (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                                                (x.SheetNameID ?? 0) != sheetNameID &&
                                                (x.ParentPackingListID ?? 0) == 0)
                                    .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                                if (!packingOverQuantities.Any() || itemStyle.MultiShip != true)
                                {
                                    packingOverQuantities = itemStyle.OrderDetails
                                        .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                                }
                                else if (revisePackingList != null)
                                {
                                    packingOverQuantities.ForEach(x =>
                                    {
                                        var revisePackingLines = revisePackingList.PackingLines
                                                .Where(y => y.Size == x.Size && y.LSStyle == x.ItemStyle?.LSStyle).ToList();
                                        revisePackingLines.ForEach(y =>
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
                                    });
                                }

                                //var prePack = packingList?.PackingLines.ToList().FirstOrDefault()?.PrePack.Trim();
                                var prePack = ifgPackingLines.FirstOrDefault()?.PrePack.Trim();
                                if ((packingOverQuantities.Find(x => x.Quantity == 0) == null && prePack == "Assorted Size - Solid Color") ||
                                    (packingOverQuantities.Sum(x => x.Quantity) > 0 && prePack == "Solid Size - Solid Color"))
                                {
                                    var newPackingLines = IFG_ClonePackingListProcess
                                        .GeneratePacking(ifgPackingLines.Where(x => x.LSStyle == styleMap[itemStyle.LSStyle]).ToList(), 
                                                                            packingOverQuantities, netWeights, false, false, ref SequenceNo, ref cartonNo);

                                    if (itemStyle != null && newPackingLines.Any())
                                    {
                                        var newOrdinalShip = 0;
                                        if (itemStyle.MultiShip == true)
                                        {
                                            if(fistStyle)
                                            {
                                                fistStyle = false;
                                                if ((packingList.OrdinalShip ?? 0) == 0)
                                                {
                                                    var packingLists = objectSpace.GetObjects<PackingList>()
                                                        .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle.LSStyle) &&
                                                                    (x.SheetNameID ?? 0) != sheetNameID)
                                                        .OrderByDescending(x => x.OrdinalShip).ToList();
                                                    newOrdinalShip = 1 + (int)(packingLists.Any() ? packingLists.FirstOrDefault().OrdinalShip ?? 0 : 0);
                                                }
                                                else
                                                {
                                                    newOrdinalShip = packingList.OrdinalShip ?? 0;
                                                }
                                                packingList.OrdinalShip = newOrdinalShip;
                                            }
                                            
                                            if (!itemStyle.PackingOverQuantities.Any())
                                            {
                                                itemStyle.PackingOverQuantities = packingOverQuantities;
                                            }
                                        }
                                    }
                                    newPackingLines.ToList().ForEach(x =>
                                    {
                                        packingList.PackingLines.Add(x);
                                    });
                                }
                                else
                                {
                                    packingList.TotalQuantity = 0;
                                    var error = Message.GetMessageOptions("Out of remaining quantity", "Error",
                                        InformationType.Error, null, 5000);
                                    Application.ShowViewStrategy.ShowMessage(error);
                                    View.Refresh();
                                    return;
                                }
                            }
                        }

                        packingList.ItemStyles = newItemStyles;
                        packingList.TotalQuantity = packingList.PackingLines?.Sum(x => x.QuantitySize * x.TotalCarton) ?? 0;
                        packingList.Confirm = true;
                        var view = (DetailView)View.ObjectSpace.Owner;
                        view.Refresh();
                    }
                    else
                    {
                        var error = Message.GetMessageOptions("Number of item styles overtake current packing list", "Error",
                            InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(error);
                        View.Refresh();
                    }
                }
                else
                {
                    var error = Message.GetMessageOptions("Invalid customer style input", "Error",
                        InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                    View.Refresh();
                }
            }
            else if(packingList.CustomerID == "GA")
            {
                var view = (DetailView)View.ObjectSpace.Owner;
                var newItemStyles = ((ListView)View).CollectionSource.List
                                        .Cast<ItemStyle>().ToList();
                newItemStyles.AddRange(listPropertyEditor.ListView
                    .SelectedObjects.Cast<ItemStyle>().ToList() ?? new List<ItemStyle>());
                
                if (styleList.Contains(";"))
                    styleList = styleList.Replace(";","','");

                var criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                        "IN (" + string.Join(",", "'" + styleList + "'") + ")");
                var netWeights = objectSpace.GetObjects<StyleNetWeight>(criteria).ToList();

                if (newItemStyles.Any())
                {
                    foreach(var data in newItemStyles)
                    {
                        if(!styleList.Contains(data.CustomerStyle))
                        {
                            errorMessage = "Invalid customer style input";
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if(styleCount < newItemStyles.Count)
                        {
                            var error = Message.GetMessageOptions("Number of item styles overtake current packing list", "Error",
                            InformationType.Error, null, 5000);
                            Application.ShowViewStrategy.ShowMessage(error);
                            view.Refresh(true);
                        }
                        else if(styleCount == newItemStyles.Count)
                        {
                            packingList.ItemStyles = newItemStyles;
                            if (packingList.PackingLines.Any())
                            {
                                packingList.LSStyles = string.Join(";", newItemStyles.Select(x => x.LSStyle).Distinct());
                                Dictionary<string, string> itemStyles = new Dictionary<string, string>();
                                int i = 0;
                                var oldStyles = packingList.PackingLines.Select(x => x.LSStyle).Distinct().ToList();
                                var packingOverQuantities = new List<PackingOverQuantity>();

                                var mappingConfig = new MapperConfiguration(c =>
                                {
                                    c.CreateMap<OrderDetail, PackingOverQuantity>()
                                        .ForMember(x => x.ColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode))
                                        .ForMember(x => x.ColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                                        .ForMember(x => x.ItemStyleNumber, y => y.MapFrom(s => s.ItemStyleNumber))
                                        .ForMember(x => x.Size, y => y.MapFrom(s => s.Size))
                                        .ForMember(x => x.Quantity, y => y.MapFrom(s => s.Quantity))
                                        .ForMember(x => x.SizeSortIndex, y => y.MapFrom(s => s.SizeSortIndex))
                                        .ForMember(x => x.ID, y => y.Ignore());
                                });
                                var mapper = mappingConfig.CreateMapper();

                                newItemStyles.ForEach(x =>
                                {
                                    itemStyles.Add(oldStyles[i++], x.LSStyle);

                                    var overQuantities = objectSpace.GetObjects<PackingOverQuantity>()
                                        .Where(y => y.ItemStyleNumber == x.Number).ToList();

                                    var revisePackingList = objectSpace.GetObjects<PackingList>()
                                        .Where(y => (y.LSStyles ?? string.Empty).Contains(x?.LSStyle) && 
                                                    (y.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                                                    (y.SheetNameID ?? 0) != sheetNameID &&
                                                    (y.ParentPackingListID ?? 0) == 0)
                                        .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                                    if (!overQuantities.Any() || x.MultiShip != true)
                                    {
                                        overQuantities = x.OrderDetails
                                            .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                                        overQuantities.ForEach(x =>
                                        {
                                            packingOverQuantities.Add(x);
                                        });
                                    }
                                    else if (revisePackingList != null)
                                    {
                                        overQuantities.ForEach(y =>
                                        {
                                            var revisePackingLines = revisePackingList.PackingLines
                                                    .Where(z => z.Size == y.Size && z.LSStyle == y.ItemStyle?.LSStyle).ToList();
                                            revisePackingLines.ForEach(z =>
                                            {
                                                if (z.Size == y.Size && z.PrePack.Trim() == "Assorted Size - Solid Color")
                                                {
                                                    y.Quantity += (int)(z.QuantitySize * z.TotalCarton);
                                                }
                                                else if (z.Size == y.Size)
                                                {
                                                    y.Quantity += (int)z.TotalQuantity;
                                                }
                                            });
                                            packingOverQuantities.Add(y);
                                        });
                                    }
                                    else
                                    {
                                        overQuantities.ForEach(x =>
                                        {
                                            packingOverQuantities.Add(x);
                                        });
                                    }
                                });

                                //var prePack = packingList?.PackingLines.ToList().FirstOrDefault()?.PrePack.Trim();
                                //if ((packingOverQuantities.Find(x => x.Quantity == 0) == null && prePack == "Assorted Size - Solid Color") ||
                                if(packingOverQuantities.Sum(x => x.Quantity) > 0 ) //&& prePack == "Solid Size - Solid Color"))
                                {
                                    packingList.PackingLines = GA_ClonePackingListProcess
                                        .GeneratePacking(packingList.PackingLines.ToList(), itemStyles, packingOverQuantities, netWeights, false);

                                    if (packingList.PackingLines.Any())
                                    {
                                        var newOrdinalShip = 0;
                                        if (newItemStyles.ToList()?.FirstOrDefault()?.MultiShip == true)
                                        {
                                            if ((packingList.OrdinalShip ?? 0) == 0)
                                            {
                                                var packingLists = objectSpace.GetObjects<PackingList>()
                                                    .Where(x => (x.LSStyles ?? string.Empty).Contains(newItemStyles.ToList()?.FirstOrDefault()?.LSStyle) &&
                                                                (x.SheetNameID ?? 0) != sheetNameID).OrderByDescending(x => x.OrdinalShip).ToList();
                                                newOrdinalShip = 1 + (int)(packingLists.Any() ? packingLists.FirstOrDefault().OrdinalShip ?? 0 : 0);
                                            }
                                            else
                                            {
                                                newOrdinalShip = packingList.OrdinalShip ?? 0;
                                            }
                                            packingList.OrdinalShip = newOrdinalShip;
                                            newItemStyles.ToList().ForEach(x =>
                                            {
                                                x.PackingOverQuantities = packingOverQuantities.Where(y => y.ItemStyleNumber == x.Number).ToList();
                                            });
                                        }
                                    }

                                    packingList.TotalQuantity = packingList.PackingLines?.Sum(x => x.QuantitySize * x.TotalCarton) ?? 0;
                                    packingList.Confirm = true;

                                    view.Refresh();
                                }
                                else
                                {
                                    packingList.TotalQuantity = 0;
                                    var error = Message.GetMessageOptions("Out of remaining quantity", "Error",
                                        InformationType.Error, null, 5000);
                                    Application.ShowViewStrategy.ShowMessage(error);
                                    view.Refresh(true);
                                }
                            }
                        }
                        else
                        {
                            packingList.ItemStyles = newItemStyles;
                            view.Refresh(true);
                        }
                    }
                    else
                    {
                        var error = Message.GetMessageOptions(errorMessage, "Error",
                            InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(error);
                        view.Refresh(true);
                    }
                }
            }
            else if (packingList.CustomerID == "DE")
            {
                var newItemStyles = new List<ItemStyle>();
                newItemStyles.AddRange(listPropertyEditor.ListView
                    .SelectedObjects.Cast<ItemStyle>().ToList() ?? new List<ItemStyle>());

                if (newItemStyles.Any())
                {
                    packingList.ItemStyles = newItemStyles;
                    packingList.LSStyles = newItemStyles.First().LSStyle;
                    //packingList.TotalQuantity = newItemStyles.First().TotalQuantity;
                   
                    var itemStyle = newItemStyles.First();

                    //var netWeights = objectSpace.GetObjects<StyleNetWeight>()
                    //                .Where(x => x.CustomerStyle == newItemStyles.First().CustomerStyle).ToList();
                    var netWeights = objectSpace.GetObjects<StyleNetWeight>
                            (CriteriaOperator.Parse("[CustomerStyle] = ?", itemStyle?.CustomerStyle)).ToList();


                    //var creatia = CriteriaOperator.Parse("[LSStyles] = ?", itemStyle?.LSStyle);
                    var currentPackingLists = objectSpace.GetObjects<PackingList>
                            (CriteriaOperator.Parse("[LSStyles] = ?", itemStyle?.LSStyle))?.ToList();

                    var currentOverQuantities = objectSpace.GetObjects<PackingOverQuantity>
                            (CriteriaOperator.Parse("[ItemStyleNumber] = ?", itemStyle?.Number))?.ToList();

                    if (packingList.PackingLines.Any())
                    {
                        var packingOverQuantities = new List<PackingOverQuantity>();

                        var mappingConfig = new MapperConfiguration(c =>
                        {
                            c.CreateMap<OrderDetail, PackingOverQuantity>()
                                .ForMember(x => x.ColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode))
                                .ForMember(x => x.ColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                                .ForMember(x => x.ItemStyleNumber, y => y.MapFrom(s => s.ItemStyleNumber))
                                .ForMember(x => x.Size, y => y.MapFrom(s => s.Size))
                                .ForMember(x => x.Quantity, y => y.MapFrom(s => s.Quantity))
                                .ForMember(x => x.SizeSortIndex, y => y.MapFrom(s => s.SizeSortIndex))
                                .ForMember(x => x.ID, y => y.Ignore());
                        });
                        var mapper = mappingConfig.CreateMapper();
                        packingOverQuantities = currentOverQuantities           //objectSpace.GetObjects<PackingOverQuantity>()
                            .Where(x => x.ItemStyleNumber == itemStyle.Number).ToList();

                        var revisePackingList = currentPackingLists           //objectSpace.GetObjects<PackingList>()
                            .Where(x => (x.LSStyles ?? string.Empty) == itemStyle?.LSStyle && 
                                        (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                                        (x.SheetNameID ?? 0) != sheetNameID)
                            .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                        if (!packingOverQuantities.Any() || itemStyle.MultiShip != true)
                        {
                            packingOverQuantities = itemStyle.OrderDetails
                            .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                        }
                        else if (revisePackingList != null)
                        {
                            packingOverQuantities.ForEach(x =>
                            {
                                var revisePackingLines = revisePackingList.PackingLines
                                        .Where(y => y.Size == x.Size && y.LSStyle == x.ItemStyle?.LSStyle).ToList();
                                revisePackingLines.ForEach(y =>
                                {
                                    if (x.Size == y.Size)
                                    {
                                        x.Quantity += (int)y.TotalQuantity;
                                    }
                                });
                            });
                        }

                        var prePack = packingList?.PackingLines.ToList().FirstOrDefault()?.PrePack.Trim();
                        if (packingOverQuantities.Sum(x => x.Quantity) > 0)
                        {
                            packingList.PackingLines = DE_ClonePackingListProcess
                                .GeneratePacking(packingList.PackingLines.ToList(), packingOverQuantities, netWeights, false);

                            if (itemStyle != null && packingList.PackingLines.Any())
                            {
                                var newOrdinalShip = 0;
                                if (itemStyle.MultiShip == true)
                                {
                                    if ((packingList.OrdinalShip ?? 0) == 0)
                                    {
                                        var packingLists = currentPackingLists          //objectSpace.GetObjects<PackingList>()
                                            .Where(x => (x.LSStyles ?? string.Empty) == itemStyle.LSStyle &&
                                                        (x.SheetNameID ?? 0) != sheetNameID)
                                            .OrderByDescending(x => x.OrdinalShip).ToList();
                                        newOrdinalShip = 1 + (int)(packingLists.Any() ? packingLists.FirstOrDefault().OrdinalShip ?? 0 : 0);
                                    }
                                    else
                                    {
                                        newOrdinalShip = packingList.OrdinalShip ?? 0;
                                    }
                                    packingList.OrdinalShip = newOrdinalShip;
                                    if (!itemStyle.PackingOverQuantities.Any())
                                    {
                                        itemStyle.PackingOverQuantities = packingOverQuantities;
                                    }
                                    //itemStyle.PackingOverQuantities = packingOverQuantities;
                                }
                            }

                            packingList.TotalQuantity = packingList.PackingLines?.Sum(x => x.QuantitySize * x.TotalCarton) ?? 0;
                            packingList.Confirm = true;

                            var view = (DetailView)View.ObjectSpace.Owner;
                            view.Refresh();
                        }
                        else
                        {
                            packingList.TotalQuantity = 0;
                            var error = Message.GetMessageOptions("Out of remaining quantity", "Error",
                                InformationType.Error, null, 5000);
                            Application.ShowViewStrategy.ShowMessage(error);
                            View.Refresh();
                        }
                    }
                }
                else
                {
                    var error = Message.GetMessageOptions("Invalid customer style input", "Error",
                        InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                    View.Refresh();
                }
            }
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