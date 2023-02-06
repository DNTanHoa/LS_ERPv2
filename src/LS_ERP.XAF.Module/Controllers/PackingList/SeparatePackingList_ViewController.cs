using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SeparatePackingList_ViewController : ViewController
    {
        public SeparatePackingList_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction separatePackingList = new PopupWindowShowAction(this, "SeparatePackingList", PredefinedCategory.Unspecified);
            separatePackingList.ImageName = "SeparatorList_32x32";
            separatePackingList.Caption = "Separate (Ctrl + S)";
            separatePackingList.TargetObjectType = typeof(ChoosePackingListPopupModel);
            separatePackingList.TargetViewType = ViewType.ListView;
            separatePackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            separatePackingList.Shortcut = "CtrlS";

            separatePackingList.CustomizePopupWindowParams += SeparatePackingList_CustomizePopupWindowParams;
            separatePackingList.Execute += SeparatePackingList_Execute;

            //SimpleAction searchSeparatePackingListAction = new SimpleAction(this, "SearchSeparatePackingList", PredefinedCategory.Unspecified);
            //searchSeparatePackingListAction.ImageName = "Action_Search_Object_FindObjectByID";
            //searchSeparatePackingListAction.Caption = "Search (Ctrl + L)";
            //searchSeparatePackingListAction.TargetObjectType = typeof(PackingList);
            //searchSeparatePackingListAction.TargetViewType = ViewType.ListView;
            //searchSeparatePackingListAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //searchSeparatePackingListAction.Shortcut = "CtrlL";

            //searchSeparatePackingListAction.Execute += SearchSeparatePackingList_Execute;
        }
        private void SeparatePackingList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            //var objectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var packingObjectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var remainObjectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var tableName = "";

            var connectString = Application.ConnectionString ?? string.Empty;
            var db = new QueryFactory(new SqlConnection(connectString), new SqlServerCompiler());

            var originPackingList = (ChoosePackingListPopupModel)View.CurrentObject;
            var separate = e.PopupWindowView.CurrentObject as PackingListSeparateParam;

            if (separate.Details.Any())
            {
                var prePack = separate.Details?.FirstOrDefault()?.PrePack;
                var fisrtStyle = separate.Details?.FirstOrDefault()?.LSStyle;
                var totCarton = separate.Details?.FirstOrDefault()?.TotalCarton ?? 0;

                var itemStyles = new Dictionary<string, string>();
                var overQuantities = new List<PackingOverQuantity>();
                foreach (var item in originPackingList.ItemStyles.OrderBy(x => x.LSStyle))
                {
                    itemStyles.Add(item.LSStyle, item.LSStyle);
                    overQuantities.AddRange(item.PackingOverQuantities);
                }

                var criteria = CriteriaOperator.Parse("[CustomerStyle] " + "IN (" + string.Join(",",
                                originPackingList.ItemStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                var netWeights = ObjectSpace.GetObjects<StyleNetWeight>(criteria).ToList();

                criteria = CriteriaOperator.Parse("[LSStyle] " + "IN (" + string.Join(",",
                                originPackingList?.PackingLines?.OrderBy(x => x.SequenceNo)?
                                .Select(x => "'" + x.LSStyle + "'")) + ")");
                var ifgStyles = ObjectSpace.GetObjects<ItemStyle>(criteria).ToList();

                var remainCarton = new Dictionary<string, decimal>();
                var separateCarton = new Dictionary<string, decimal>();

                originPackingList.PackingLines.ToList().ForEach(x =>
                {
                    remainCarton.Add(x.LSStyle + ";" + x.Size, (decimal)x.TotalCarton);
                    if (originPackingList.CustomerID == "IFG" && x.LSStyle == fisrtStyle)
                    {
                        if (prePack == "Assorted Size - Solid Color")
                            separateCarton.Add(x.Size, (decimal)(separate?.Details?.
                                                FirstOrDefault()?.TotalCarton));
                        else
                            separateCarton.Add(x.Size, (decimal)(separate?.Details?.
                                                FirstOrDefault(s => s.Size == x.Size)?.TotalCarton));
                    }
                });

                var packingLines = new List<PackingLine>();
                if (string.IsNullOrEmpty(tableName))
                    tableName = typeof(PackingLine).Name;

                packingLines = db.Query(tableName).WhereRaw(" [PackingListID] = " + originPackingList?.ID).Get<PackingLine>().ToList();

                packingLines.OrderBy(x => x.SequenceNo)
                    .ToList().ForEach(p =>
                    {
                        if (prePack == "Assorted Size - Solid Color")
                        {
                            p.TotalQuantity = p.QuantitySize * totCarton;
                        }
                        else
                        {
                            p.TotalQuantity = p.QuantitySize *
                                separate.Details.FirstOrDefault(s => s.Size == p.Size).TotalCarton;
                        }

                        p.BoxDimension = ObjectSpace.GetObjectByKey<BoxDimension>(p.BoxDimensionCode);
                    });

                try
                {
                    criteria = CriteriaOperator.Parse("[Number] " + "IN (" + string.Join(",",
                                originPackingList.ItemStyles.Select(x => "'" + x.Number + "'")) + ")");

                    var newStyles = packingObjectSpace.GetObjects<ItemStyle>(criteria);
                    var newUnit = packingObjectSpace.GetObjectByKey<PackingUnit>(originPackingList?.PackingUnit?.ID);

                    var newPackingLines = new List<PackingLine>();
                    if (originPackingList.CustomerID == "IFG")
                    {
                        newPackingLines = IFG_SeparatePackingListProcess
                                .SeparatePacking(packingLines, ifgStyles, netWeights, separateCarton, false);
                    }
                    else
                    {
                        newPackingLines = GA_ClonePackingListProcess
                                .GeneratePacking(packingLines, itemStyles, overQuantities, netWeights, true);
                    }


                    var newPackingList = packingObjectSpace.CreateObject<PackingList>();
                    newPackingList.PackingListCode = "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6);
                    newPackingList.PackingListDate = originPackingList.PackingListDate;
                    newPackingList.CompanyCode = originPackingList.CompanyCode;
                    newPackingList.RatioLeft = originPackingList.RatioLeft;
                    newPackingList.RatioRight = originPackingList.RatioRight;
                    newPackingList.RatioBottom = originPackingList.RatioBottom;
                    newPackingList.Confirm = originPackingList.Confirm;
                    newPackingList.DeliveryGroup = originPackingList.DeliveryGroup;
                    newPackingList.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                    newPackingList.DontShortShip = originPackingList.DontShortShip;
                    newPackingList.LSStyles = originPackingList.LSStyles;
                    newPackingList.Factory = originPackingList.Factory;
                    newPackingList.ShippingMark1Code = originPackingList.ShippingMark1Code;
                    newPackingList.ShippingMark2Code = originPackingList.ShippingMark2Code;
                    newPackingList.ShippingMethodCode = originPackingList.ShippingMethodCode;
                    newPackingList.CustomerID = originPackingList.CustomerID;
                    newPackingList.SalesOrderID = originPackingList.SalesOrderID;
                    newPackingList.PackingUnit = newUnit;
                    newPackingList.SheetNameID = originPackingList.SheetNameID;
                    newPackingList.OrdinalShip = originPackingList.OrdinalShip;
                    newPackingList.ItemStyles = newStyles;
                    newPackingList.ParentPackingListID = originPackingList.ID;
                    newPackingList.SeparatedOrdinal = 1;
                    newPackingList.SetCreateAudit(SecuritySystem.CurrentUserName);
                    newPackingList.PackingLines = new List<PackingLine>();
                    newPackingList.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListGA").ToString(),
                        SortIndex = 1
                    });
                    newPackingList.PPCBookDate = originPackingList.PPCBookDate;
                    newPackingList.IsRevised = originPackingList.IsRevised;

                    foreach (var data in newPackingLines.OrderBy(x => x.SequenceNo))
                    {
                        var newpackingLine = new PackingLine()
                        {
                            SequenceNo = data.SequenceNo,
                            LSStyle = data.LSStyle,
                            QuantitySize = data.QuantitySize,
                            QuantityPerPackage = data.QuantityPerPackage,
                            PackagesPerBox = data.PackagesPerBox,
                            QuantityPerCarton = data.QuantityPerCarton,
                            TotalQuantity = data.TotalQuantity,
                            NetWeight = data.NetWeight,
                            GrossWeight = data.GrossWeight,
                            Color = data.Color,
                            PrePack = data.PrePack,
                            Size = data.Size,
                            Quantity = data.Quantity,
                            BoxDimensionCode = data.BoxDimensionCode,
                            Length = data.Length,
                            Width = data.Width,
                            Height = data.Height,
                            FromNo = data.FromNo,
                            ToNo = data.ToNo,
                            TotalCarton = data.TotalCarton,
                            DeliveryPlace = data.DeliveryPlace,
                        };

                        if (remainCarton.ContainsKey(data.LSStyle + ";" + data.Size))
                        {
                            remainCarton[data.LSStyle + ";" + data.Size] -= (decimal)data.TotalCarton;
                        }

                        newpackingLine.SetCreateAudit(SecuritySystem.CurrentUserName);

                        newPackingList.PackingLines.Add(newpackingLine);
                    }

                    //// Remain
                    packingLines.OrderBy(x => x.SequenceNo)
                    .ToList().ForEach(p =>
                    {
                        p.TotalQuantity = p.QuantitySize * remainCarton[p.LSStyle + ";" + p.Size];
                    });

                    var remainPackingLines = new List<PackingLine>();
                    if (originPackingList.CustomerID == "IFG")
                    {
                        remainPackingLines = IFG_SeparatePackingListProcess
                                .SeparatePacking(packingLines, ifgStyles, netWeights, separateCarton, true);
                    }
                    else
                    {
                        remainPackingLines = GA_ClonePackingListProcess
                                .GeneratePacking(packingLines, itemStyles, overQuantities, netWeights, true);
                    }


                    var remainStyles = remainObjectSpace.GetObjects<ItemStyle>(criteria);
                    var remainUnit = remainObjectSpace.GetObjectByKey<PackingUnit>(originPackingList?.PackingUnit?.ID);

                    var remainPackingList = remainObjectSpace.CreateObject<PackingList>();
                    remainPackingList.PackingListCode = "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6);
                    remainPackingList.PackingListDate = originPackingList.PackingListDate;
                    remainPackingList.CompanyCode = originPackingList.CompanyCode;
                    remainPackingList.RatioLeft = originPackingList.RatioLeft;
                    remainPackingList.RatioRight = originPackingList.RatioRight;
                    remainPackingList.RatioBottom = originPackingList.RatioBottom;
                    remainPackingList.Confirm = originPackingList.Confirm;
                    remainPackingList.DeliveryGroup = originPackingList.DeliveryGroup;
                    remainPackingList.TotalQuantity = remainPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
                    remainPackingList.DontShortShip = originPackingList.DontShortShip;
                    remainPackingList.LSStyles = originPackingList.LSStyles;
                    remainPackingList.Factory = originPackingList.Factory;
                    remainPackingList.ShippingMark1Code = originPackingList.ShippingMark1Code;
                    remainPackingList.ShippingMark2Code = originPackingList.ShippingMark2Code;
                    remainPackingList.ShippingMethodCode = originPackingList.ShippingMethodCode;
                    remainPackingList.CustomerID = originPackingList.CustomerID;
                    remainPackingList.SalesOrderID = originPackingList.SalesOrderID;
                    remainPackingList.PackingUnit = remainUnit;
                    remainPackingList.SheetNameID = originPackingList.SheetNameID;
                    remainPackingList.OrdinalShip = originPackingList.OrdinalShip;
                    remainPackingList.ItemStyles = remainStyles;
                    remainPackingList.ParentPackingListID = originPackingList.ID;
                    remainPackingList.SeparatedOrdinal = 2;
                    remainPackingList.SetCreateAudit(SecuritySystem.CurrentUserName);
                    remainPackingList.PackingLines = new List<PackingLine>();
                    remainPackingList.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListGA").ToString(),
                        SortIndex = 1
                    });
                    remainPackingList.PPCBookDate = originPackingList.PPCBookDate;
                    remainPackingList.IsRevised = originPackingList.IsRevised;

                    foreach (var remain in remainPackingLines.OrderBy(x => x.SequenceNo))
                    {
                        var remainPackingLine = new PackingLine()
                        {
                            SequenceNo = remain.SequenceNo,
                            LSStyle = remain.LSStyle,
                            QuantitySize = remain.QuantitySize,
                            QuantityPerPackage = remain.QuantityPerPackage,
                            PackagesPerBox = remain.PackagesPerBox,
                            QuantityPerCarton = remain.QuantityPerCarton,
                            TotalQuantity = remain.TotalQuantity,
                            NetWeight = remain.NetWeight,
                            GrossWeight = remain.GrossWeight,
                            Color = remain.Color,
                            PrePack = remain.PrePack,
                            Size = remain.Size,
                            Quantity = remain.Quantity,
                            BoxDimensionCode = remain.BoxDimensionCode,
                            Length = remain.Length,
                            Width = remain.Width,
                            Height = remain.Height,
                            FromNo = remain.FromNo,
                            ToNo = remain.ToNo,
                            TotalCarton = remain.TotalCarton,
                            DeliveryPlace = remain.DeliveryPlace,
                        };
                        remainPackingLine.SetCreateAudit(SecuritySystem.CurrentUserName);

                        remainPackingList.PackingLines.Add(remainPackingLine);
                    }

                    packingObjectSpace.CommitChanges();
                    remainObjectSpace.CommitChanges();

                    ///  Update IsSeparated for Packing List
                    tableName = typeof(PackingList).Name;
                    var update = db.Query(tableName)
                        .Where("ID", originPackingList.ID)
                        .Update(new
                        {
                            IsSeparated = true
                        });

                    //var oldPackingList = objectSpace.GetObjectByKey<PackingList>(originPackingList.ID);
                    //oldPackingList.IsSeparated = true;
                    //objectSpace.CommitChanges();

                    //packingLists.Remove(originPackingList);
                    //var addLists = this.ObjectSpace.GetObjects<PackingList>(CriteriaOperator.Parse("[ParentPackingListID] = ?", originPackingList.ID));
                    //var config = new MapperConfiguration(x =>
                    //    x.CreateMap<PackingList, ChoosePackingListPopupModel>().ForMember(s => s.SheetName, y => y.MapFrom(s => s.SheetName.SheetName)));
                    //var mapper = config.CreateMapper();
                    //var ChoosePackingListPopupModels = addLists.Select(p => mapper.Map<ChoosePackingListPopupModel>(p)).ToList();
                    //packingLists.AddRange(ChoosePackingListPopupModels);

                    //masterObject.ChoosePackingListPopupModel = new List<ChoosePackingListPopupModel>(packingLists);

                    var message = Message.GetMessageOptions("Separate packing list successful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);

                    View.Refresh(true);
                }
                catch (Exception ex)
                {
                    var message = Message.GetMessageOptions("Separate packing list failed", "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
            }
        }

        private void SeparatePackingList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var packingList = ((ListView)View).CurrentObject as ChoosePackingListPopupModel;

            var model = new PackingListSeparateParam();
            if (packingList.PackingLines.Any())
            {
                var lsStyle = packingList.PackingLines.OrderBy(x => x.SequenceNo).FirstOrDefault().LSStyle;
                var lines = new List<SeparatePackingLine>();

                if(packingList?.PackingLines?.FirstOrDefault()?.PrePack == "Assorted Size - Solid Color")
                {
                    var data = packingList?.PackingLines?.FirstOrDefault();
                    var line = new SeparatePackingLine()
                    {
                        SequenceNo = data.SequenceNo,
                        FromNo = data.FromNo,
                        ToNo = data.ToNo,
                        LSStyle = data.LSStyle,
                        QuantitySize = data.QuantitySize,
                        QuantityPerCarton = data.QuantityPerCarton,
                        TotalQuantity = data.TotalQuantity,
                        QuantityPerPackage = data.QuantityPerPackage,
                        PackagesPerBox = data.PackagesPerBox,
                        PrePack = data.PrePack,
                        Quantity = data.Quantity,
                        //Size = data.Size,
                        BoxDemensionCode = data.BoxDimensionCode,
                        Length = data.Length,
                        Width = data.Width,
                        Height = data.Height,
                        TotalCarton = data.TotalCarton ?? 0,
                    };
                    lines.Add(line);
                }
                else
                {
                    var sortedPackingLines = packingList.PackingLines
                    .Where(x => x.LSStyle == lsStyle).OrderBy(x => x.SequenceNo).ToList();

                    foreach (var data in sortedPackingLines)
                    {
                        var line = new SeparatePackingLine()
                        {
                            SequenceNo = data.SequenceNo,
                            FromNo = data.FromNo,
                            ToNo = data.ToNo,
                            LSStyle = data.LSStyle,
                            QuantitySize = data.QuantitySize,
                            QuantityPerCarton = data.QuantityPerCarton,
                            TotalQuantity = data.TotalQuantity,
                            QuantityPerPackage = data.QuantityPerPackage,
                            PackagesPerBox = data.PackagesPerBox,
                            PrePack = data.PrePack,
                            Quantity = data.Quantity,
                            Size = data.Size,
                            BoxDemensionCode = data.BoxDimensionCode,
                            Length = data.Length,
                            Width = data.Width,
                            Height = data.Height,
                            TotalCarton = data.TotalCarton ?? 0,
                        };
                        lines.Add(line);
                    }
                
                }
                model.Details = lines.OrderBy(x => x.SequenceNo).ToList();
            }

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true; 
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
