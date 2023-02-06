using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShippingPlanLoadPackingList_ViewController : ObjectViewController<DetailView, ShippingPlan>
    {
        public ShippingPlanLoadPackingList_ViewController()
        {
            InitializeComponent();            

            PopupWindowShowAction loacdPackingList = new PopupWindowShowAction(this,
                "LoadPackingList", PredefinedCategory.Unspecified);
            loacdPackingList.ImageName = "Header";
            loacdPackingList.Caption = "Load Packing list";
            loacdPackingList.TargetObjectType = typeof(ShippingPlan);
            loacdPackingList.TargetViewType = ViewType.DetailView;
            loacdPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
           

            loacdPackingList.CustomizePopupWindowParams += LoadPackingList_CustomizePopupWindowParams;
            loacdPackingList.Execute += LoadPackingList_Execute;
        }

        private void LoadPackingList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as ShippingPlan;
            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("PackingLists") as ListPropertyEditor;
            var packingListModels = listPropertyEditor.ListView?.SelectedObjects.Cast<PackingListPopupModel>().ToList();

            var invoiceNO = ((DetailView)e.PopupWindowView).FindItem("InvoiceNO");

            var LoadPackingListSearchParam = invoiceNO.CurrentObject as LoadPackingListSearchParam;

            decimal constInchToCm = 2.54M;
            foreach(var packingListModel in packingListModels)
            {
                var packingList = objectSpace.FirstOrDefault<PackingList>(p => p.ID == packingListModel.ID);
                if(packingList != null)
                {
                    if (packingList.CustomerID == "DE")
                    {
                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => i.LSStyle == packingList.LSStyles);
                        
                        var shippingPlanDetail = new ShippingPlanDetail();
                        shippingPlanDetail.PackingListID = packingList.ID;
                        shippingPlanDetail.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                        shippingPlanDetail.GarmentColorCode = itemStyle.ColorCode;
                        shippingPlanDetail.LSStyle = packingList.LSStyles;
                        shippingPlanDetail.CustomerStyle = itemStyle.CustomerStyle;
                        shippingPlanDetail.PCS = (int)packingList.TotalQuantity;
                        shippingPlanDetail.CTN = (int)packingList.PackingLines.Select(s => s.TotalCarton).Sum();
                        shippingPlanDetail.GrossWeight = (decimal)packingList.PackingLines.Select(s => s.GrossWeight).Sum();
                        shippingPlanDetail.Volume = (decimal)packingList.PackingLines.Select(s => s.TotalCarton*
                                                    s.Width * s.Height * s.Length / 1000000).Sum();
                        shippingPlanDetail.InvoiceNumber = LoadPackingListSearchParam.InvoiceNO;
                        shippingPlanDetail.Destination = itemStyle.DeliveryPlace;
                        shippingPlanDetail.ShipDate = packingList.PPCBookDate;
                        shippingPlanDetail.Color = packingListModel.Color;
                        shippingPlanDetail.SheetName = packingListModel.SheetName;
                        shippingPlanDetail.OrderType = packingListModel.OrderType;
                        shippingPlanDetail.Description = packingListModel.Description;
                        shippingPlanDetail.Dept = packingListModel.Dept;
                        shippingPlanDetail.ProductionDescription = packingListModel.ProductionDescription;
                        shippingPlanDetail.ContractDate = packingListModel.ContractDate;
                        shippingPlanDetail.EstimatedSupplierHandOver = packingListModel.EstimatedSupplierHandOver;


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
                        viewObject.Details.Add(shippingPlanDetail);
                        viewObject.SetUpdateAudit(SecuritySystem.CurrentUserName);
                    }
                    else if (packingList.CustomerID == "GA")
                    {
                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i =>packingList.LSStyles.Contains(i.LSStyle));
                        var pro = objectSpace.FirstOrDefault<ProductionDept>(p => p.ProductionDescription == itemStyle.ProductionDescription);
                        var shippingPlanDetail = new ShippingPlanDetail();
                        shippingPlanDetail.PackingListID = packingList.ID;
                        shippingPlanDetail.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                        shippingPlanDetail.GarmentColorCode = packingListModel.Model;
                        shippingPlanDetail.LSStyle = packingList.LSStyles;
                        shippingPlanDetail.CustomerStyle = itemStyle.CustomerStyle;
                        shippingPlanDetail.PCS = (int)packingList.TotalQuantity;
                        shippingPlanDetail.InvoiceNumber = LoadPackingListSearchParam.InvoiceNO;
                        shippingPlanDetail.Destination = itemStyle.DeliveryPlace;
                        shippingPlanDetail.ShipDate = packingList.PPCBookDate;
                        shippingPlanDetail.Color = packingListModel.Color;
                        shippingPlanDetail.SheetName = packingListModel.SheetName;
                        shippingPlanDetail.OrderType = packingListModel.OrderType;
                        shippingPlanDetail.Description = packingListModel.Description;

                        if (pro != null)
                        {
                            shippingPlanDetail.Dept = pro.Dept;
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
                            var sizes = packingList.PackingLines.Select(s=>s.Size).Distinct().ToList();
                            decimal gw = 0M;
                            decimal vol = 0M;
                            int ctn= 0;
                            foreach(var size in sizes)
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
                                                
                        
                        viewObject.Details.Add(shippingPlanDetail);
                        viewObject.SetUpdateAudit(SecuritySystem.CurrentUserName);
                    }  
                    else if(packingList.CustomerID == "IFG")
                    {
                        var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => packingList.LSStyles.Contains(i.LSStyle));
                        var pro = objectSpace.FirstOrDefault<ProductionDept>(p => p.ProductionDescription == itemStyle.ProductionDescription);
                        var shippingPlanDetail = new ShippingPlanDetail();
                        shippingPlanDetail.PackingListID = packingList.ID;
                        shippingPlanDetail.PurchaseOrderNumber = itemStyle.PurchaseOrderNumber;
                        shippingPlanDetail.GarmentColorCode = packingListModel.Color;
                        shippingPlanDetail.LSStyle = packingList.LSStyles;
                        shippingPlanDetail.CustomerStyle = packingListModel.CustomerStyle;
                        shippingPlanDetail.PCS = (int)packingList.TotalQuantity;
                        shippingPlanDetail.InvoiceNumber = LoadPackingListSearchParam.InvoiceNO;
                        shippingPlanDetail.Destination = packingListModel.DeliveryPlace;
                        shippingPlanDetail.ShipDate = packingList.PPCBookDate;
                        shippingPlanDetail.Color = packingListModel.Color;
                        shippingPlanDetail.SheetName = packingListModel.SheetName;
                        shippingPlanDetail.OrderType = packingListModel.OrderType;
                        if (pro != null)
                        {
                            shippingPlanDetail.Dept = pro.Dept;
                        }
                        if (packingList.PackingLines.FirstOrDefault().PrePack == "Assorted Size - Solid Color")
                        {
                            var size = packingList.PackingLines.FirstOrDefault().Size;

                            shippingPlanDetail.NetWeight = (decimal)packingList.PackingLines.Where(p => p.Size == size).Sum(s => s.NetWeight);

                            var totalCartonWeigth = packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().GrossWeight
                                                                - packingList.PackingLines.Where(p => p.Size == size).FirstOrDefault().NetWeight;

                            shippingPlanDetail.GrossWeight = shippingPlanDetail.NetWeight + (decimal)totalCartonWeigth;

                            if(packingList.PackingUnit.ID == "Inch-lbs")
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


                        
                        viewObject.Details.Add(shippingPlanDetail);
                        viewObject.SetUpdateAudit(SecuritySystem.CurrentUserName);
                    }    
                }    
            }
            
            objectSpace.CommitChanges();
            View.Refresh();
        }

        private void LoadPackingList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as ShippingPlan;
           
            var model = new LoadPackingListSearchParam();   
            model.Company = viewObject.Company;
            model.Customer = viewObject.Customer;

            var fromDate = DateTime.Now;
            fromDate = fromDate.AddMonths(-3);
            model.FromDate = fromDate;

            var toDate = DateTime.Now;
            toDate = toDate.AddMonths(1);           
            model.ToDate = toDate;

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
