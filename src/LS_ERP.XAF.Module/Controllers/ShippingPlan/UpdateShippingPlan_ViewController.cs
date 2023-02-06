using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateShippingPlan_ViewController : ObjectViewController<DetailView, ShippingPlan>
    {
        public UpdateShippingPlan_ViewController()
        {
            InitializeComponent();

            SimpleAction updateShippingPlan = new SimpleAction(this, "UpdateShippingPlan", PredefinedCategory.Unspecified);
            updateShippingPlan.ImageName = "Update";
            updateShippingPlan.Caption = "Update";
            //updateShippingPlan.TargetObjectType = typeof(ShippingPlan);
            //updateShippingPlan.TargetViewType = ViewType.DetailView;
            updateShippingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //exportSalesContract.Shortcut = "CtrlShiftP";
            updateShippingPlan.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            updateShippingPlan.Execute += UpdateShippingPlan_Execute;
        }

        public void UpdateShippingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ShippingPlan;
           
            var messageOptions = new MessageOptions();
            UpdateShippingPlan(viewObject);
            ObjectSpace.CommitChanges();
            messageOptions = Message.GetMessageOptions("Update Shipping Plan", "Success",
                       InformationType.Success, null, 5000);
            Application.ShowViewStrategy.ShowMessage(messageOptions);

            //View.RefreshDataSource();
            View.ObjectSpace.Refresh();
        }
        private bool UpdateShippingPlan(ShippingPlan shippingPlan)
        {
            var objectSpace = this.ObjectSpace;
            var shippingPlans = new List<ShipmentDetail>();
            decimal constInchToCm = 2.54M;
            //
            foreach (var shippingPlanDetail in shippingPlan.Details)
            {
                var packingList = objectSpace.FirstOrDefault<PackingList>(p => p.ID == shippingPlanDetail.PackingListID);
                if (packingList != null)
                {
                    if (packingList.CustomerID == "DE")
                    {
                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == packingList.LSStyles);                        
                        var productionDept = objectSpace.FirstOrDefault<ProductionDept>(p=>p.ProductionDescription == itemStyle.ProductionDescription);
                        var purchaseOrderType = objectSpace.FirstOrDefault<PurchaseOrderType>(p => p.Code == itemStyle.PurchaseOrderTypeCode);

                        shippingPlanDetail.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                        shippingPlanDetail.GarmentColorCode = itemStyle.ColorCode;
                        shippingPlanDetail.LSStyle = packingList.LSStyles;
                        shippingPlanDetail.CustomerStyle = itemStyle.CustomerStyle;
                        shippingPlanDetail.PCS = (int)packingList.TotalQuantity;
                        shippingPlanDetail.CTN = (int)packingList.PackingLines.Select(s => s.TotalCarton).Sum();
                        shippingPlanDetail.GrossWeight = (decimal)packingList.PackingLines.Select(s => s.GrossWeight).Sum();
                        shippingPlanDetail.Volume = (decimal)packingList.PackingLines.Select(s => s.TotalCarton *
                                                    s.Width * s.Height * s.Length / 1000000).Sum();                        
                        shippingPlanDetail.Destination = itemStyle.DeliveryPlace;
                        shippingPlanDetail.ShipDate = packingList.PPCBookDate;
                        shippingPlanDetail.Color = itemStyle.Description;
                        shippingPlanDetail.SheetName = packingList.SheetName.SheetName;
                        shippingPlanDetail.OrderType = purchaseOrderType?.Name;
                        shippingPlanDetail.Description = itemStyle.Description;
                        shippingPlanDetail.Dept = (int)productionDept?.Dept;
                        shippingPlanDetail.ProductionDescription = productionDept?.ProductionDescription;
                        shippingPlanDetail.ContractDate = itemStyle.ContractDate;
                        shippingPlanDetail.EstimatedSupplierHandOver = itemStyle.EstimatedSupplierHandOver;


                        var price = (decimal)itemStyle.OrderDetails.FirstOrDefault().Price;
                        if (itemStyle?.SalesOrder?.PriceTermCode == "CMT" || itemStyle?.SalesOrder?.PriceTermCode == "CM")
                        {
                            //CMQ
                            shippingPlanDetail.PriceCM = Math.Round(price, 3);
                            //FCA
                            var partPrice = objectSpace.FirstOrDefault<PartPrice>(x => x.Season == itemStyle.Season
                                                                                && x.GarmentColorCode == itemStyle.ColorCode
                                                                                && x.ProductionType == "FOB");
                            shippingPlanDetail.PriceFOB = partPrice == null ? 0 : Math.Round((decimal)partPrice?.Price, 3);
                            //total CMQ
                            shippingPlanDetail.TotalPriceCM = Math.Round(price * (decimal)packingList.TotalQuantity, 3);
                            //total FCA
                            shippingPlanDetail.TotalPriceFOB = partPrice == null ? 0 : Math.Round((decimal)partPrice.Price * (decimal)packingList.TotalQuantity, 3);
                        }
                        else
                        {
                            //FCA
                            shippingPlanDetail.PriceFOB = Math.Round(price, 3);
                            //CMQ
                            var partPrice = objectSpace.FirstOrDefault<PartPrice>(x => x.Season == itemStyle.Season
                                                                               && x.GarmentColorCode == itemStyle.ColorCode
                                                                               && x.ProductionType == "CMT");
                            shippingPlanDetail.PriceCM = partPrice == null ? 0 : Math.Round((decimal)partPrice?.Price, 3);
                            //total FCA
                            shippingPlanDetail.TotalPriceFOB = Math.Round(price * (decimal)packingList.TotalQuantity, 3);
                            //total CMQ
                            shippingPlanDetail.TotalPriceCM = partPrice == null ? 0 : Math.Round((decimal)partPrice.Price * (decimal)packingList.TotalQuantity, 3);
                        }                        
                    }
                    else if (packingList.CustomerID == "GA")
                    {
                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => packingList.LSStyles.Contains(i.LSStyle));
                        var pro = objectSpace.FirstOrDefault<ProductionDept>(p => p.ProductionDescription == itemStyle.ProductionDescription);
                        var LSStyles = packingList.LSStyles.Split(";").ToList<string>();
                       
                        shippingPlanDetail.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;                        
                        shippingPlanDetail.LSStyle = packingList.LSStyles;
                        shippingPlanDetail.CustomerStyle = itemStyle.CustomerStyle;
                        shippingPlanDetail.PCS = (int)packingList.TotalQuantity;
                        shippingPlanDetail.Destination = itemStyle.DeliveryPlace;
                        shippingPlanDetail.ShipDate = packingList.PPCBookDate;


                        if (pro != null)
                        {
                            shippingPlanDetail.Dept = pro.Dept;
                        }
                        //
                        shippingPlanDetail.GarmentColorCode = string.Empty;
                        shippingPlanDetail.CustomerStyle = string.Empty;
                        shippingPlanDetail.Description = string.Empty;
                        shippingPlanDetail.Color = string.Empty;  
                        shippingPlanDetail.OrderType = string.Empty;
                        //
                        foreach (var item in LSStyles)
                        {
                            var lsstyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == item);
                            if (lsstyle != null)
                            {
                                shippingPlanDetail.CustomerStyle += string.IsNullOrEmpty(shippingPlanDetail.CustomerStyle) ? lsstyle?.CustomerStyle : @"\" + lsstyle?.CustomerStyle;
                                shippingPlanDetail.Description += string.IsNullOrEmpty(shippingPlanDetail.Description) ? lsstyle?.Description : @"\" + lsstyle?.Description;
                                shippingPlanDetail.Color += string.IsNullOrEmpty(shippingPlanDetail.Color) ? lsstyle?.ColorName : @"\" + lsstyle?.ColorName;
                                shippingPlanDetail.GarmentColorCode += string.IsNullOrEmpty(shippingPlanDetail.GarmentColorCode) ? lsstyle?.ColorCode : @"\" + lsstyle?.ColorCode;

                                var purcharseOrderTypeCode1 = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == lsstyle.PurchaseOrderTypeCode);
                                var type = lsstyle.ExternalPurchaseOrderTypeName != null ?
                                    lsstyle.ExternalPurchaseOrderTypeName : purcharseOrderTypeCode1 == null ? null : purcharseOrderTypeCode1.Name;

                                shippingPlanDetail.OrderType += string.IsNullOrEmpty(shippingPlanDetail.OrderType) ? type : @"\" + type;
                            }
                        }

                        if (packingList.PackingLines.FirstOrDefault().PrePack == "Assorted Size - Solid Color")
                        {
                            var size = packingList.PackingLines.FirstOrDefault().Size;

                            shippingPlanDetail.NetWeight = (decimal)packingList.PackingLines.Where(p => p.Size == size).Sum(s => s.NetWeight);

                            var totalCartonWeigth = packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().GrossWeight
                                                                - packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().NetWeight;

                            shippingPlanDetail.GrossWeight = shippingPlanDetail.NetWeight + (decimal)totalCartonWeigth;

                            shippingPlanDetail.Volume = (decimal)packingList.PackingLines.Select(s => constInchToCm * constInchToCm * constInchToCm * s.TotalCarton *
                                                    s.Width * s.Height * s.Length / 1000000).FirstOrDefault();
                            shippingPlanDetail.CTN = (int)packingList.PackingLines.FirstOrDefault()?.TotalCarton;
                        }
                        else
                        {
                            shippingPlanDetail.NetWeight = (decimal)packingList.PackingLines.Select(s => s.NetWeight).Sum();
                            var sizes = packingList.PackingLines.Select(s => s.Size).Distinct().ToList();
                            decimal gw = 0M;
                            decimal vol = 0M;
                            int ctn = 0;
                            foreach (var size in sizes)
                            {
                                var netweightsize = packingList.PackingLines.Where(x => x.Size == size).Sum(s => s.NetWeight);
                                var totalCartonWeightSize = packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().GrossWeight
                                                        - packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().NetWeight;
                                gw += (decimal)netweightsize + (decimal)totalCartonWeightSize;
                                vol += (decimal)packingList.PackingLines.Where(p => p.Size == size).Select(s => s.TotalCarton * s.Width * s.Height * s.Length * constInchToCm * constInchToCm * constInchToCm / 1000000).FirstOrDefault();
                                ctn += (int)packingList.PackingLines.Where(p => p.Size == size).Select(s => s.TotalCarton).FirstOrDefault();
                            }
                            shippingPlanDetail.GrossWeight = gw;
                            shippingPlanDetail.Volume = vol;
                            shippingPlanDetail.CTN = ctn;
                        }

                        
                        
                    }
                    else if (packingList.CustomerID == "IFG")
                    {
                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => packingList.LSStyles.Contains(i.LSStyle));
                        var pro = objectSpace.FirstOrDefault<ProductionDept>(p => p.ProductionDescription == itemStyle.ProductionDescription);
                        var LSStyles = packingList.LSStyles.Split(";").ToList<string>();

                       
                        shippingPlanDetail.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;                        
                        shippingPlanDetail.LSStyle = packingList.LSStyles;
                        shippingPlanDetail.CustomerStyle = itemStyle.CustomerStyle;
                        shippingPlanDetail.PCS = (int)packingList.TotalQuantity;
                        shippingPlanDetail.Destination = itemStyle.DeliveryPlace;
                        shippingPlanDetail.ShipDate = packingList.PPCBookDate;
                        
                        if (pro != null)
                        {
                            shippingPlanDetail.Dept = pro.Dept;
                        }
                        //                        
                        shippingPlanDetail.CustomerStyle = string.Empty;
                        shippingPlanDetail.Description = string.Empty;                        
                        shippingPlanDetail.Color = string.Empty;
                        shippingPlanDetail.GarmentColorCode = string.Empty;
                        shippingPlanDetail.OrderType = string.Empty;
                        //
                        foreach (var item in LSStyles)
                        {
                            var lsstyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == item);
                            if (lsstyle != null)
                            {
                                shippingPlanDetail.CustomerStyle += string.IsNullOrEmpty(shippingPlanDetail.CustomerStyle) ? lsstyle?.CustomerStyle : @"#" + lsstyle?.CustomerStyle;
                                shippingPlanDetail.Description += string.IsNullOrEmpty(shippingPlanDetail.Description) ? lsstyle?.Description : @"#" + lsstyle?.Description;
                                shippingPlanDetail.Color += string.IsNullOrEmpty(shippingPlanDetail.Color) ? lsstyle?.ColorName : @"#" + lsstyle?.ColorName;
                                shippingPlanDetail.GarmentColorCode += string.IsNullOrEmpty(shippingPlanDetail.GarmentColorCode) ? lsstyle?.ColorCode : @"#" + lsstyle?.ColorCode;

                                var purcharseOrderTypeCode1 = objectSpace.FirstOrDefault<PurchaseOrderType>(i => i.Code == lsstyle.PurchaseOrderTypeCode);
                                var type = lsstyle.ExternalPurchaseOrderTypeName != null ?
                                    lsstyle.ExternalPurchaseOrderTypeName : purcharseOrderTypeCode1 == null ? null : purcharseOrderTypeCode1.Name;
                                shippingPlanDetail.OrderType += string.IsNullOrEmpty(shippingPlanDetail.OrderType) ? type : @"#" + type;
                            }
                        }

                        if (packingList.PackingLines.FirstOrDefault().PrePack == "Assorted Size - Solid Color")
                        {
                            var size = packingList.PackingLines.FirstOrDefault().Size;

                            shippingPlanDetail.NetWeight = (decimal)packingList.PackingLines.Where(p => p.Size == size).Sum(s => s.NetWeight);

                            var totalCartonWeigth = packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().GrossWeight
                                                                - packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().NetWeight;

                            shippingPlanDetail.GrossWeight = shippingPlanDetail.NetWeight + (decimal)totalCartonWeigth;

                            if (packingList.PackingUnit.ID == "Inch-lbs")
                            {
                                shippingPlanDetail.Volume = (decimal)packingList.PackingLines.Select(s => constInchToCm * constInchToCm * constInchToCm * s.TotalCarton *
                                                    s.Width * s.Height * s.Length / 1000000).FirstOrDefault();
                            }

                            shippingPlanDetail.CTN = (int)packingList.PackingLines.FirstOrDefault()?.TotalCarton;
                        }
                        else
                        {
                            shippingPlanDetail.NetWeight = (decimal)packingList.PackingLines.Select(s => s.NetWeight).Sum();
                            var sizes = packingList.PackingLines.Select(s => s.Size).Distinct().ToList();
                            decimal gw = 0M;
                            decimal vol = 0M;
                            int ctn = 0;
                            foreach (var size in sizes)
                            {
                                var netweightsize = packingList.PackingLines.Where(x => x.Size == size).Sum(s => s.NetWeight);
                                var totalCartonWeightSize = packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().GrossWeight
                                                        - packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().NetWeight;

                                if (packingList.PackingUnit.ID == "Inch-lbs")
                                {
                                    vol += (decimal)packingList.PackingLines.Where(p => p.Size == size).Select(s => s.TotalCarton *
                                    s.Width * s.Height * s.Length * constInchToCm * constInchToCm * constInchToCm / 1000000).FirstOrDefault();
                                }
                                gw += (decimal)netweightsize + (decimal)totalCartonWeightSize;
                                ctn += (int)packingList.PackingLines.Where(p => p.Size == size).Select(s => s.TotalCarton).FirstOrDefault();
                            }
                            shippingPlanDetail.GrossWeight = gw;
                            shippingPlanDetail.Volume = vol;
                            shippingPlanDetail.CTN = ctn;
                        }
                                                
                       
                        
                    }
                }
            }

            //
            objectSpace.CommitChanges();
            return true;
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
