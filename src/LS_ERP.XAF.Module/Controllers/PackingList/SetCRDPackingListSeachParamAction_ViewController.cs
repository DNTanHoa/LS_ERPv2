using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Extensions;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SetCRDPackingListSeachParamAction_ViewController
        : ObjectViewController<DetailView, PackingListSetCRDParam>
    {
        public SetCRDPackingListSeachParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPackingList = new SimpleAction(this, "SetCRDPackingListSeachPackingList", PredefinedCategory.Unspecified);
            searchPackingList.ImageName = "Action_Search_Object_FindObjectByID";
            searchPackingList.Caption = "Search";
            searchPackingList.TargetObjectType = typeof(PackingListSetCRDParam);
            searchPackingList.TargetViewType = ViewType.DetailView;
            searchPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchPackingList.Execute += SetCRDSearchPackingList_Execute;

          
        }

        private void SetCRDSearchPackingList_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PackingListSetCRDParam;            
            if(viewObject != null)
            {
                if(viewObject.Customer!=null && viewObject.Company != null)
                {
                    var objectSpace = this.ObjectSpace;
                    var config = new MapperConfiguration(x =>
                                                    x.CreateMap<PackingList, PackingListPopupModel>());
                    var mapper = config.CreateMapper();
                    //
                    var LSStyleList = new List<string>();
                    var str_LSStyles = viewObject.LSStyles;
                    if(!string.IsNullOrEmpty(str_LSStyles))
                    {
                        LSStyleList = str_LSStyles.Trim().Split("\r\n").ToList();                        
                    }
                    //
                    if (viewObject.Customer.ID == "DE")
                    {
                        var connectionString = Application.ConnectionString ?? string.Empty;
                        SqlParameter[] parameters =
                        {
                            new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                            new SqlParameter("@CompanyCode",viewObject?.Company.Code ?? string.Empty),
                            new SqlParameter("@FromDate",viewObject?.PackingFromDate),
                            new SqlParameter("@ToDate",viewObject?.PackingToDate)
                        };                        
                        DataTable table = SqlHelper.FillByReader(connectionString, "[sp_LoadPackingListForSetCargoReadyDate]", parameters);
                        var pkl = table.AsListObject<PackingListPopupModel>();
                        //pkl = pkl.Where(p => LSStyles.Contains(p.LSStyles)).ToList();
                        //
                        if(string.IsNullOrEmpty(str_LSStyles) && pkl.Count > 0)
                        {
                            viewObject.PackingLists = pkl;
                            
                        }
                        else
                        {
                            var ListPackingListPopupModel = new List<PackingListPopupModel>();

                            foreach (var LSStyle in LSStyleList)
                            {
                                var p = pkl.Where(p => p.LSStyles == LSStyle).FirstOrDefault();
                                if (p != null)
                                {
                                    var pm = p;
                                    ListPackingListPopupModel.Add(pm);
                                }

                            }
                            viewObject.PackingLists = ListPackingListPopupModel;
                        }  
                    }    
                    else if(viewObject.Customer.ID == "GA")
                    {
                        var connectionString = Application.ConnectionString ?? string.Empty;
                        SqlParameter[] parameters =
                        {
                            new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                            new SqlParameter("@CompanyCode",viewObject?.Company.Code ?? string.Empty),
                            new SqlParameter("@FromDate",viewObject?.PackingFromDate),
                            new SqlParameter("@ToDate",viewObject?.PackingToDate)
                        };
                        DataTable table = SqlHelper.FillByReader(connectionString, "[sp_LoadPackingListForSetCargoReadyDate]", parameters);
                        var pkl = table.AsListObject<PackingListPopupModel>();
                        //
                        if (string.IsNullOrEmpty(str_LSStyles) && pkl.Count > 0)
                        {
                            //viewObject.PackingLists = pkl;
                            var ListPackingListPopupModel = new List<PackingListPopupModel>();
                            foreach (var p in pkl)
                            {
                                var LSStyles = p.LSStyles.Split(";").ToList<string>();
                                var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => LSStyles.Contains(i.LSStyle));
                                //var packingSheetName = objectSpace.FirstOrDefault<PackingSheetName>(x => x.ID == p.SheetNameID);
                                //var itemStyle = p.ItemStyles.FirstOrDefault();

                                if (itemStyle != null)
                                {
                                    var pm = mapper.Map<PackingListPopupModel>(p);
                                    pm.CustomerStyle = string.Empty;
                                    pm.Description = string.Empty;
                                    pm.Color = string.Empty;
                                    pm.Model = string.Empty;
                                    pm.OrderType = string.Empty;
                                    //var purcharseOrderTypeCode = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == itemStyle.PurchaseOrderTypeCode);
                                    pm.PurchaseOrderNumber = itemStyle?.PurchaseOrderNumber;
                                    pm.ProductionDescription = itemStyle?.ProductionDescription;
                                    foreach (var item in LSStyles)
                                    {
                                        var lsstyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == item);
                                        if (lsstyle != null)
                                        {
                                            pm.CustomerStyle += string.IsNullOrEmpty(pm.CustomerStyle) ? lsstyle?.CustomerStyle : @"\" + lsstyle?.CustomerStyle;
                                            pm.Description += string.IsNullOrEmpty(pm.Description) ? lsstyle?.Description : @"\" + lsstyle?.Description;
                                            pm.Color += string.IsNullOrEmpty(pm.Color) ? lsstyle?.ColorName : @"\" + lsstyle?.ColorName;
                                            pm.Model += string.IsNullOrEmpty(pm.Model) ? lsstyle?.ColorCode : @"\" + lsstyle?.ColorCode;

                                            var purcharseOrderTypeCode1 = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == lsstyle.PurchaseOrderTypeCode);
                                            var type = lsstyle.ExternalPurchaseOrderTypeName != null ?
                                                lsstyle.ExternalPurchaseOrderTypeName : purcharseOrderTypeCode1 == null ? null : purcharseOrderTypeCode1.Name;

                                            pm.OrderType += string.IsNullOrEmpty(pm.OrderType) ? type : @"\" + type;
                                        }
                                    }

                                    pm.DeliveryPlace = itemStyle?.DeliveryPlace;
                                    pm.ProductionSkedDeliveryDate = itemStyle?.ProductionSkedDeliveryDate ?? null;
                                    pm.ContractDate = itemStyle?.ContractDate ?? null;
                                    pm.Season = itemStyle?.Season;
                                    pm.Unit = itemStyle?.Unit?.Name;

                                    ListPackingListPopupModel.Add(pm);
                                }
                            }
                            viewObject.PackingLists = ListPackingListPopupModel;
                        }
                        else
                        {
                            var ListPackingListPopupModel = new List<PackingListPopupModel>();
                            foreach (var LSStyle in LSStyleList)
                            {
                                var pmm = pkl.Where(x=>CheckLSStyle(x,LSStyle)).ToList();
                                foreach (var p in pmm)
                                {
                                    var LSStyles = p.LSStyles.Split(";").ToList<string>();
                                    var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => LSStyles.Contains(i.LSStyle));
                                    //var packingSheetName = objectSpace.FirstOrDefault<PackingSheetName>(x => x.ID == p.SheetNameID);
                                    //var itemStyle = p.ItemStyles.FirstOrDefault();

                                    if (itemStyle != null)
                                    {                                       
                                        var pm = mapper.Map<PackingListPopupModel>(p);
                                        pm.CustomerStyle = string.Empty;
                                        pm.Description = string.Empty;
                                        pm.Color = string.Empty;
                                        pm.Model = string.Empty;
                                        pm.OrderType = string.Empty;
                                        //var purcharseOrderTypeCode = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == itemStyle.PurchaseOrderTypeCode);
                                        pm.PurchaseOrderNumber = itemStyle?.PurchaseOrderNumber;
                                        pm.ProductionDescription = itemStyle?.ProductionDescription;
                                        foreach (var item in LSStyles)
                                        {
                                            var lsstyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == item);
                                            if (lsstyle != null)
                                            {
                                                pm.CustomerStyle += string.IsNullOrEmpty(pm.CustomerStyle) ? lsstyle?.CustomerStyle : @"\" + lsstyle?.CustomerStyle;
                                                pm.Description += string.IsNullOrEmpty(pm.Description) ? lsstyle?.Description : @"\" + lsstyle?.Description;
                                                pm.Color += string.IsNullOrEmpty(pm.Color) ? lsstyle?.ColorName : @"\" + lsstyle?.ColorName;
                                                pm.Model += string.IsNullOrEmpty(pm.Model) ? lsstyle?.ColorCode : @"\" + lsstyle?.ColorCode;

                                                var purcharseOrderTypeCode1 = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == lsstyle.PurchaseOrderTypeCode);
                                                var type = lsstyle.ExternalPurchaseOrderTypeName != null ?
                                                    lsstyle.ExternalPurchaseOrderTypeName : purcharseOrderTypeCode1 == null ? null : purcharseOrderTypeCode1.Name;

                                                pm.OrderType += string.IsNullOrEmpty(pm.OrderType) ? type : @"\" + type;
                                            }
                                        }

                                        pm.DeliveryPlace = itemStyle?.DeliveryPlace;
                                        pm.ProductionSkedDeliveryDate = itemStyle?.ProductionSkedDeliveryDate ?? null;
                                        pm.ContractDate = itemStyle?.ContractDate ?? null;
                                        pm.Season = itemStyle?.Season;
                                        pm.Unit = itemStyle?.Unit?.Name;

                                        ListPackingListPopupModel.Add(pm);
                                       

                                    }

                                }
                            }
                            viewObject.PackingLists = ListPackingListPopupModel;
                        }    
                        
                    }
                    else if (viewObject.Customer.ID == "IFG")
                    {
                        var connectionString = Application.ConnectionString ?? string.Empty;
                        SqlParameter[] parameters =
                        {
                            new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                            new SqlParameter("@CompanyCode",viewObject?.Company.Code ?? string.Empty),
                            new SqlParameter("@FromDate",viewObject?.PackingFromDate),
                            new SqlParameter("@ToDate",viewObject?.PackingToDate)
                        };
                        DataTable table = SqlHelper.FillByReader(connectionString, "[sp_LoadPackingListForSetCargoReadyDate]", parameters);
                        var pkl = table.AsListObject<PackingListPopupModel>();

                        //
                        if (string.IsNullOrEmpty(str_LSStyles) && pkl.Count > 0)
                        {
                            var ListPackingListPopupModel = new List<PackingListPopupModel>();
                            foreach (var p in pkl)
                            {
                                if (p != null)
                                {
                                    var LSStyles = p.LSStyles.Split(";").ToList<string>();
                                    var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => LSStyles.Contains(i.LSStyle));

                                    if (itemStyle != null)
                                    {
                                        var pm = mapper.Map<PackingListPopupModel>(p);
                                        pm.CustomerStyle = string.Empty;
                                        pm.Description = string.Empty;
                                        pm.Color = string.Empty;
                                        pm.Model = string.Empty;
                                        pm.OrderType = string.Empty;
                                        //var purcharseOrderTypeCode = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == itemStyle.PurchaseOrderTypeCode);
                                        pm.PurchaseOrderNumber = itemStyle?.PurchaseOrderNumber;
                                        pm.ProductionDescription = itemStyle?.ProductionDescription;
                                        foreach (var item in LSStyles)
                                        {
                                            var lsstyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == item);
                                            if (lsstyle != null)
                                            {
                                                pm.CustomerStyle += string.IsNullOrEmpty(pm.CustomerStyle) ? lsstyle?.CustomerStyle : @"#" + lsstyle?.CustomerStyle;
                                                pm.Description += string.IsNullOrEmpty(pm.Description) ? lsstyle?.Description : @"#" + lsstyle?.Description;
                                                pm.Color += string.IsNullOrEmpty(pm.Color) ? lsstyle?.ColorName : @"#" + lsstyle?.ColorName;
                                                pm.Model += string.IsNullOrEmpty(pm.Model) ? lsstyle?.ColorCode : @"#" + lsstyle?.ColorCode;

                                                var purcharseOrderTypeCode1 = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == lsstyle.PurchaseOrderTypeCode);
                                                var type = lsstyle.ExternalPurchaseOrderTypeName != null ?
                                                    lsstyle.ExternalPurchaseOrderTypeName : purcharseOrderTypeCode1 == null ? null : purcharseOrderTypeCode1.Name;
                                                pm.OrderType += string.IsNullOrEmpty(pm.OrderType) ? type : @"#" + type;
                                            }
                                        }
                                        pm.DeliveryPlace = itemStyle?.ShipTo;
                                        pm.Description = itemStyle?.Brand;
                                        pm.Gender = itemStyle?.Gender;
                                        pm.Season = itemStyle?.Season;
                                        ListPackingListPopupModel.Add(pm);
                                    }

                                }
                            }
                            viewObject.PackingLists = ListPackingListPopupModel;
                        }
                        else
                        {
                            var ListPackingListPopupModel = new List<PackingListPopupModel>();
                            foreach (var LSStyle in LSStyleList)
                            {
                                var pmm = pkl.Where(x => CheckLSStyle(x, LSStyle)).ToList();
                                foreach (var p in pmm)
                                {
                                    if (p != null)
                                    {
                                        var LSStyles = p.LSStyles.Split(";").ToList<string>();
                                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => LSStyles.Contains(i.LSStyle));

                                        if (itemStyle != null)
                                        {
                                            var pm = mapper.Map<PackingListPopupModel>(p);
                                            pm.CustomerStyle = string.Empty;
                                            pm.Description = string.Empty;
                                            pm.Color = string.Empty;
                                            pm.Model = string.Empty;
                                            pm.OrderType = string.Empty;
                                            //var purcharseOrderTypeCode = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == itemStyle.PurchaseOrderTypeCode);
                                            pm.PurchaseOrderNumber = itemStyle?.PurchaseOrderNumber;
                                            pm.ProductionDescription = itemStyle?.ProductionDescription;
                                            foreach (var item in LSStyles)
                                            {
                                                var lsstyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == item);
                                                if (lsstyle != null)
                                                {
                                                    pm.CustomerStyle += string.IsNullOrEmpty(pm.CustomerStyle) ? lsstyle?.CustomerStyle : @"#" + lsstyle?.CustomerStyle;
                                                    pm.Description += string.IsNullOrEmpty(pm.Description) ? lsstyle?.Description : @"#" + lsstyle?.Description;
                                                    pm.Color += string.IsNullOrEmpty(pm.Color) ? lsstyle?.ColorName : @"#" + lsstyle?.ColorName;
                                                    pm.Model += string.IsNullOrEmpty(pm.Model) ? lsstyle?.ColorCode : @"#" + lsstyle?.ColorCode;

                                                    var purcharseOrderTypeCode1 = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == lsstyle.PurchaseOrderTypeCode);
                                                    var type = lsstyle.ExternalPurchaseOrderTypeName != null ?
                                                        lsstyle.ExternalPurchaseOrderTypeName : purcharseOrderTypeCode1 == null ? null : purcharseOrderTypeCode1.Name;
                                                    pm.OrderType += string.IsNullOrEmpty(pm.OrderType) ? type : @"#" + type;
                                                }
                                            }
                                            pm.DeliveryPlace = itemStyle?.ShipTo;
                                            pm.Description = itemStyle?.Brand;
                                            pm.Gender = itemStyle?.Gender;
                                            pm.Season = itemStyle?.Season;
                                            ListPackingListPopupModel.Add(pm);                                            
                                        }

                                    }
                                }
                            }
                            viewObject.PackingLists = ListPackingListPopupModel;
                        }                            
                    }    
                    View.Refresh();
                } 
            }
        }
        private bool CheckLSStyle(PackingListPopupModel packingListModel, string lsstyle)
        {
            var lsstyles = packingListModel.LSStyles.Split(";").ToList();
            foreach (var style in lsstyles)
            {
                if (style == lsstyle)
                    return true;
            }
            return false;
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
