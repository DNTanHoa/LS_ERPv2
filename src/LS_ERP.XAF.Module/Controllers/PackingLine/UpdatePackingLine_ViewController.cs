using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdatePackingLine_ViewController : ObjectViewController<ListView, PackingLine>
    {
        private IObjectSpace ObjectSpaceOverQuantity;
        int cartonNo = 1;
        public UpdatePackingLine_ViewController()
        {
            InitializeComponent();

            SimpleAction updatePackingLineAction = new SimpleAction(this, "UpdatePackingLineAction", PredefinedCategory.Unspecified);
            updatePackingLineAction.ImageName = "Update";
            updatePackingLineAction.Caption = "Update (Ctrl + U)";
            updatePackingLineAction.TargetObjectType = typeof(PackingLine);
            updatePackingLineAction.TargetViewType = ViewType.ListView;
            updatePackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updatePackingLineAction.Shortcut = "CtrlU";
            updatePackingLineAction.Execute += UpdatePackingLineAction_Execute;
        }
        private void UpdatePackingLineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var objectSpacePackingList = Application.CreateObjectSpace(typeof(PackingList));
            ObjectSpaceOverQuantity = Application.CreateObjectSpace(typeof(PackingOverQuantity));
            var packingList = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;
            var sheetNameID = ObjectSpace.GetObjects<PackingSheetName>()
                    .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;

            if(packingList.ItemStyles.ToList().Any())
            {
                if(packingList.CustomerID =="IFG")
                {
                    var isRevised = false;
                    if (packingList.ID != 0)
                        isRevised = true;

                    var packingLines = (View as ListView).CollectionSource.List
                        .Cast<PackingLine>().ToList();
                    if (packingLines.Any())
                    {
                        var fullPackingLine = new List<PackingLine>();
                        var SequenceNo = 0;
                        foreach (var itemStyle in packingList.ItemStyles.OrderBy(x => x.LSStyle))
                        {
                            //var netWeights = objectSpace.GetObjects<StyleNetWeight>()
                            //.Where(x => x.CustomerStyle == packingList.ItemStyles.First().CustomerStyle).ToList();
                            //var itemStyle = packingList.ItemStyles.First();

                            var netWeights = objectSpace.GetObjects<StyleNetWeight>()
                            .Where(x => x.CustomerStyle == itemStyle.CustomerStyle).ToList();
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
                            packingOverQuantities = ObjectSpaceOverQuantity.GetObjects<PackingOverQuantity>()
                                                        .Where(x => x.ItemStyleNumber == itemStyle.Number).ToList();

                            //var revisePackingList = objectSpace.GetObjects<PackingList>()
                            //        .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle?.LSStyle) &&
                            //                    (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                            //                    (x.SheetNameID ?? 0) != sheetNameID)
                            //        .OrderByDescending(x => x.PackingListDate).FirstOrDefault();

                            var revisePackingList = new PackingList();
                            if (isRevised)
                            {
                                revisePackingList = objectSpacePackingList.GetObjectByKey<PackingList>(packingList.ID);
                            }
                            else
                            {
                                revisePackingList = objectSpace.GetObjects<PackingList>()
                                    .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle?.LSStyle) &&
                                                (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                                                (x.SheetNameID ?? 0) != sheetNameID &&
                                                (x.ParentPackingListID ?? 0) == 0)
                                    .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                            }

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

                            var newPackingLines = new List<PackingLine>();
                            var prePack = packingLines.FirstOrDefault().PrePack.Trim();
                            var errorMessage = "";
                            if (prePack == "Assorted Size - Solid Color")
                            {
                                //var assortedPackingLines = packingLines.OrderBy(x => x.SequenceNo).Take(packingOverQuantities.Count()).ToList();
                                var assortedPackingLines = packingLines.OrderBy(x => x.SequenceNo).ToList();
                                var totalCarton = assortedPackingLines.FirstOrDefault().TotalCarton;
                                foreach (var overQuantity in packingOverQuantities)
                                {
                                    var assortedPackingLine = assortedPackingLines.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle && x.Size == overQuantity.Size);
                                    if (totalCarton * assortedPackingLine.QuantitySize > overQuantity.Quantity)
                                    {
                                        errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                        break;
                                    }
                                    else
                                    {
                                        assortedPackingLine.TotalQuantity = totalCarton * assortedPackingLine.QuantitySize;
                                    }
                                }
                            }
                            else
                            {
                                var solidPackingLines = packingLines.Where(x => x.PrePack.Trim() == "Solid Size - Solid Color").ToList();
                                foreach (var overQuantity in packingOverQuantities)
                                {
                                    var solidPackingLine = solidPackingLines.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle && x.Size == overQuantity.Size);
                                    if (solidPackingLine.TotalCarton * solidPackingLine.QuantitySize > overQuantity.Quantity)
                                    {
                                        errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                        break;
                                    }
                                    else
                                    {
                                        solidPackingLine.TotalQuantity = solidPackingLine.TotalCarton * solidPackingLine.QuantitySize;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(errorMessage.Trim()))
                            {
                                var error = Message.GetMessageOptions(errorMessage, "Error",
                                                InformationType.Error, null, 5000);
                                Application.ShowViewStrategy.ShowMessage(error);
                                View.Refresh();
                            }
                            else
                            {
                                //newPackingLines = ClonePackingListProcess
                                //    .GeneratePacking(packingLines.Where(x => x.LSStyle == itemStyle.LSStyle).ToList(), packingOverQuantities, netWeights, true, isRevised);
                                
                                newPackingLines = IFG_ClonePackingListProcess
                                    .GeneratePacking(packingLines.Where(x => x.LSStyle == itemStyle.LSStyle).ToList(), packingOverQuantities, netWeights, true, isRevised, ref SequenceNo, ref cartonNo);

                                if (isRevised)
                                {
                                    itemStyle.PackingOverQuantities = new List<PackingOverQuantity>(packingOverQuantities);
                                }
                                //packingList.PackingLines = newPackingLines;
                                //packingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                                fullPackingLine.AddRange(newPackingLines);

                                //var view = (DetailView)View.ObjectSpace.Owner;
                                //view.Refresh();
                            }
                        }
                        packingList.PackingLines = fullPackingLine;
                        packingList.TotalQuantity = fullPackingLine.Sum(x => x.QuantitySize * x.TotalCarton);

                        var view = (DetailView)View.ObjectSpace.Owner;
                        view.Refresh();
                    }
                    else
                    {
                        View.Refresh();
                    }
                }
                else if (packingList.CustomerID == "GA")
                {
                    var isRevised = false;
                    //var isSeparated = false;

                    if (packingList.ID != 0)
                        isRevised = true;
                    //else if(packingList.ParentPackingListID > 0)
                    //    isSeparated = true;

                    var criteria = CriteriaOperator.Parse("[CustomerStyle] " +
                                "IN (" + string.Join(",", packingList.ItemStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                    var netWeights = objectSpace.GetObjects<StyleNetWeight>(criteria).ToList();

                    var packingLines = (View as ListView).CollectionSource.List
                            .Cast<PackingLine>().OrderBy(x => x.SequenceNo).ToList();
                    if (packingLines.Any())
                    {
                        Dictionary<string, string> itemStyles = new Dictionary<string, string>();
                        var packingOverQuantities = new List<PackingOverQuantity>();
                        var newPackingLines = new List<PackingLine>();
                        var prePack = packingLines.FirstOrDefault().PrePack.Trim();
                        var errorMessage = "";

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

                        var totalCarton = packingLines.FirstOrDefault().TotalCarton;
                        var defaultPackingLines = new List<PackingLine>(); 
                        if(prePack == "Solid Size - Solid Color")
                        {
                            defaultPackingLines = packingLines.Where(x => x.LSStyle == packingLines.OrderBy(x => x.SequenceNo).FirstOrDefault().LSStyle).ToList();
                        }

                        foreach (var itemStyle in packingList.ItemStyles.OrderBy(x => x.LSStyle))
                        {
                            itemStyles.Add(itemStyle.LSStyle, itemStyle.LSStyle);

                            var overQuantities = ObjectSpaceOverQuantity.GetObjects<PackingOverQuantity>()
                                                    .Where(x => x.ItemStyleNumber == itemStyle.Number).ToList();

                            var revisePackingList = new PackingList();
                            if (isRevised)
                            {
                                revisePackingList = objectSpacePackingList.GetObjectByKey<PackingList>(packingList.ID);
                            }
                            //else if(isSeparated)
                            //{
                            //    revisePackingList = objectSpacePackingList.GetObjectByKey<PackingList>(packingList.ParentPackingListID);
                            //}
                            else
                            {
                                revisePackingList = objectSpace.GetObjects<PackingList>()
                                    .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle?.LSStyle) &&
                                                (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                                                (x.SheetNameID ?? 0) != sheetNameID &&
                                                (x.ParentPackingListID ?? 0) == 0)
                                    .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                            }
                            if (!overQuantities.Any() || itemStyle.MultiShip != true)
                            {
                                overQuantities = itemStyle.OrderDetails
                                    .Select(x => mapper.Map<PackingOverQuantity>(x)).ToList();
                                overQuantities.ForEach(x =>
                                {
                                    packingOverQuantities.Add(x);
                                });
                            }
                            else if (revisePackingList != null)
                            {
                                overQuantities.ForEach(x =>
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
                                    packingOverQuantities.Add(x);
                                });
                            }
                            else
                            {
                                overQuantities.ForEach(x =>
                                {
                                    packingOverQuantities.Add(x);
                                });
                            }

                            if (isRevised)
                            {
                                itemStyle.PackingOverQuantities = new List<PackingOverQuantity>(overQuantities);
                            }

                            if (prePack == "Assorted Size - Solid Color")
                            {
                                foreach (var overQuantity in overQuantities)
                                {
                                    var assortedPackingLine = packingLines?.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle && x.Size == overQuantity.Size);
                                    if(assortedPackingLine != null)
                                    {
                                        if (totalCarton * assortedPackingLine.QuantitySize > overQuantity.Quantity)
                                        {
                                            errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                            break;
                                        }
                                        else
                                        {
                                            assortedPackingLine.TotalQuantity = totalCarton * assortedPackingLine?.QuantitySize;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var overQuantity in packingOverQuantities)
                                {
                                    var solidPackingLine = packingLines?.FirstOrDefault(x => x.LSStyle == itemStyle.LSStyle && x.Size == overQuantity.Size);
                                    if(solidPackingLine != null)
                                    {
                                        var solidTotalCarton = defaultPackingLines?.FirstOrDefault(x => x.Size == overQuantity.Size)?.TotalCarton ?? 0;
                                        if (solidTotalCarton * solidPackingLine?.QuantitySize > overQuantity.Quantity)
                                        {
                                            errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                            break;
                                        }
                                        else
                                        {
                                            solidPackingLine.TotalQuantity = solidTotalCarton * solidPackingLine?.QuantitySize;
                                        }
                                    }
                                }
                            }
                        }  

                        if (!string.IsNullOrEmpty(errorMessage.Trim()))
                        {
                            var error = Message.GetMessageOptions(errorMessage, "Error",
                                            InformationType.Error, null, 5000);
                            Application.ShowViewStrategy.ShowMessage(error);
                            View.Refresh();
                        }
                        else
                        {
                            newPackingLines = GA_ClonePackingListProcess
                                .GeneratePacking(packingLines, itemStyles, packingOverQuantities, netWeights, true);

                            packingList.PackingLines = newPackingLines;
                            packingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);

                            var view = (DetailView)View.ObjectSpace.Owner;
                            view.Refresh();
                        }
                    }
                    else
                    {
                        View.Refresh();
                    }
                }
                else if (packingList.CustomerID == "DE")
                {
                    var isRevised = false;
                    if (packingList.ID != 0)
                        isRevised = true;

                    var itemStyle = packingList.ItemStyles.First();

                    //var netWeights = objectSpace.GetObjects<StyleNetWeight>()
                    //    .Where(x => x.CustomerStyle == packingList.ItemStyles.First().CustomerStyle).ToList();
                    var netWeights = objectSpace.GetObjects<StyleNetWeight>
                            (CriteriaOperator.Parse("[CustomerStyle] = ?", itemStyle?.CustomerStyle)).ToList();

                    var currentPackingLists = objectSpace.GetObjects<PackingList>
                            (CriteriaOperator.Parse("[LSStyles] = ?", itemStyle?.LSStyle))?.ToList();

                    //var currentOverQuantities = objectSpace.GetObjects<PackingOverQuantity>
                    //        (CriteriaOperator.Parse("[ItemStyleNumber] = ?", itemStyle?.Number))?.ToList();

                    var packingLines = (View as ListView).CollectionSource.List
                        .Cast<PackingLine>().ToList();
                    if (packingLines.Any())
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
                        packingOverQuantities = ObjectSpaceOverQuantity.GetObjects<PackingOverQuantity>
                                (CriteriaOperator.Parse("[ItemStyleNumber] = ?", itemStyle?.Number))?.ToList();

                        var revisePackingList = new PackingList();
                        if (isRevised)
                        {
                            revisePackingList = objectSpacePackingList.GetObjectByKey<PackingList>(packingList.ID);
                        }
                        else
                        {
                            revisePackingList = currentPackingLists      //objectSpace.GetObjects<PackingList>()
                                .Where(x => (x.LSStyles ?? string.Empty) == itemStyle?.LSStyle &&
                                            (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                                            (x.SheetNameID ?? 0) != sheetNameID)
                                .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                        }

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

                        var newPackingLines = new List<PackingLine>();
                        var prePack = packingLines.FirstOrDefault().PrePack.Trim();
                        var errorMessage = "";
                            
                        //var solidPackingLines = packingLines.Where(x => x.PrePack.Trim() == "Solid Size - Solid Color").ToList();
                        foreach (var overQuantity in packingOverQuantities)
                        {
                            var solidpackingLines = packingLines.Where(x => x.Size == overQuantity.Size);
                            if(solidpackingLines.Any())
                            {
                                if (solidpackingLines.OrderBy(x => x.SequenceNo).FirstOrDefault().TotalQuantity > overQuantity.Quantity)
                                {
                                    errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(errorMessage.Trim()))
                        {
                            var error = Message.GetMessageOptions(errorMessage, "Error",
                                            InformationType.Error, null, 5000);
                            Application.ShowViewStrategy.ShowMessage(error);
                            View.Refresh();
                        }
                        else
                        {
                            newPackingLines = DE_ClonePackingListProcess
                                .GeneratePacking(packingLines, packingOverQuantities, netWeights, true);

                            if (isRevised)
                            {
                                itemStyle.PackingOverQuantities = new List<PackingOverQuantity>(packingOverQuantities);
                            }
                           
                            packingList.PackingLines = newPackingLines;
                            packingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);

                            var view = (DetailView)View.ObjectSpace.Owner;
                            view.Refresh();
                        }
                    }
                    else
                    {
                        View.Refresh();
                    }
                }
                else if (packingList.CustomerID == "PU")
                {
                    var netWeights = objectSpace.GetObjects<StyleNetWeight>()
                        .Where(x => x.CustomerStyle == packingList.ItemStyles.First().CustomerStyle).ToList();
                    var itemStyle = packingList.ItemStyles.First();

                    var packingLines = (View as ListView).CollectionSource.List
                        .Cast<PackingLine>().ToList();
                    if (packingLines.Any())
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
                        packingOverQuantities = ObjectSpaceOverQuantity.GetObjects<PackingOverQuantity>()
                                                    .Where(x => x.ItemStyleNumber == itemStyle.Number).ToList();

                        //var revisePackingList =  objectSpace.GetObjects<PackingList>()
                        //    .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle?.LSStyle) &&
                        //                (x.OrdinalShip ?? 0) == packingList?.OrdinalShip &&
                        //                (x.SheetNameID ?? 0) != sheetNameID)
                        //    .OrderByDescending(x => x.PackingListDate).FirstOrDefault();
                        var revisePackingList = objectSpacePackingList.GetObjectByKey<PackingList>(packingList.ID);
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

                        var newPackingLines = new List<PackingLine>();
                        //var minQuantity = packingLines.OrderBy(x => x.SequenceNo).Take(packingOverQuantities.Count()).ToList().Min(x => x.TotalQuantity);
                        //decimal currentRatios = 0;
                        //decimal newRatios = 0;
                        var prePack = packingLines.FirstOrDefault().PrePack.Trim();
                        var errorMessage = "";
                        if (prePack == "Assorted Size - Solid Color")
                        {
                            //packingLines.OrderBy(x => x.SequenceNo).Take(packingOverQuantities.Count()).ToList().ForEach(x =>
                            //{
                            //    currentRatios += (decimal)x.Quantity;
                            //    newRatios += (decimal)(x.TotalQuantity / minQuantity);
                            //});
                            //if (currentRatios != newRatios)
                            //{
                            //    errorMessage = "Invalid total quantity input (difference current ratio sizes)";
                            //}
                            //else
                            //{
                                var assortedPackingLines = packingLines.OrderBy(x => x.SequenceNo).Take(packingOverQuantities.Count()).ToList();
                                foreach (var overQuantity in packingOverQuantities)
                                {
                                    if (assortedPackingLines.Find(x => x.Size == overQuantity.Size && x.TotalQuantity > overQuantity.Quantity) != null)
                                    {
                                        errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                        break;
                                    }
                                }
                            //}
                        }
                        else
                        {
                            //var solidPackingLines = packingLines.Where(x => x.PrePack.Trim() == "Solid Size - Solid Color").ToList();
                            foreach (var overQuantity in packingOverQuantities)
                            {
                                if (packingLines.Where(x => x.Size == overQuantity.Size).OrderBy(x => x.SequenceNo).FirstOrDefault().TotalQuantity > overQuantity.Quantity)
                                {
                                    errorMessage = "Invalid total quantity input (greater than remaining quantity)";
                                    break;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(errorMessage.Trim()))
                        {
                            var error = Message.GetMessageOptions(errorMessage, "Error",
                                            InformationType.Error, null, 5000);
                            Application.ShowViewStrategy.ShowMessage(error);
                            View.Refresh();
                        }
                        else
                        {
                            newPackingLines = PU_ClonePackingListProcess
                                .GeneratePacking(packingLines, packingOverQuantities, netWeights, true);

                            itemStyle.PackingOverQuantities = new List<PackingOverQuantity>(packingOverQuantities);
                            packingList.PackingLines = newPackingLines;
                            packingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);

                            var view = (DetailView)View.ObjectSpace.Owner;
                            view.Refresh();
                        }
                    }
                    else
                    {
                        View.Refresh();
                    }
                }
            }
            else
            {
                var error = Message.GetMessageOptions("Please add style before update", "Error",
                                        InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
                View.Refresh();
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
