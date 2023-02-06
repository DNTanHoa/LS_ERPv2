using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ChoosePackingListSearchAction_ViewController
        : ObjectViewController<DetailView, ChoosePackingListParam>
    {
        public ChoosePackingListSearchAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPackingList = new SimpleAction(this, "InvoiceSearchPackingList", PredefinedCategory.Unspecified);
            searchPackingList.ImageName = "Action_Search_Object_FindObjectByID";
            searchPackingList.Caption = "Search";
            searchPackingList.TargetObjectType = typeof(ChoosePackingListParam);
            searchPackingList.TargetViewType = ViewType.DetailView;
            searchPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchPackingList.Execute += InvoiceSearchPackingList_Execute;

          
        }

        private void InvoiceSearchPackingList_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as ChoosePackingListParam;      
            
            if(viewObject != null)
            {
               
                string fillPackingList = "";

                if (viewObject.ChoosePackingListPopupModel != null && viewObject.ExistPackingList.Any())
                {
                    fillPackingList = "AND NOT (ID IN (" + String.Join(",", viewObject.ExistPackingList.Select(x => x.ID)) + "))";
                }

                var criteria = CriteriaOperator.Parse("[CustomerID] = ? AND ([Invoice].[ID] IS NULL OR [Invoice].[ID] = 0 ) AND PPCBookDate IS NOT NULL AND [Confirm] = true AND [IsSeparated] <> true " + fillPackingList
                    , viewObject.CustomerID);

                var PackingLists = objectSpace.GetObjects<PackingList>(criteria).ToList();
                var config = new MapperConfiguration(x =>
                                                    x.CreateMap<PackingList, ChoosePackingListPopupModel>().ForMember(s=>s.SheetName, y=>y.MapFrom(s=>s.SheetName.SheetName)));
                var mapper = config.CreateMapper();
                var ChoosePackingListPopupModels = PackingLists.Select(p=>mapper.Map<ChoosePackingListPopupModel>(p)).ToList();
                var result = new List<ChoosePackingListPopupModel>();

                if(!string.IsNullOrEmpty(viewObject.LSStyles))
                {
                    var LSStyles = viewObject.LSStyles.Trim().Split("\r\n").ToList();
                    foreach (var lsstyle in LSStyles)
                    {
                        var choosePackingListPopupModel = ChoosePackingListPopupModels.Where(x=>CheckLSStyle(x,lsstyle) == true
                                                            && !IsExistChoose(result,x.ID)
                                                                ).ToList();
                        foreach(var p in choosePackingListPopupModel)
                        {
                            if(p.CustomerID == "DE")
                            {
                                p.PurchaseOrderNumber = p.ItemStyles.FirstOrDefault()?.PurchaseOrderNumber;
                            }
                            else
                            {
                                var packingListLSStyles = p.LSStyles.Split(";").ToList<string>();
                                var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => packingListLSStyles.Contains(i.LSStyle));
                                p.PurchaseOrderNumber = itemStyle?.PurchaseOrderNumber;
                            }
                            result.Add(p);
                        }                            
                    }    
                }
                else
                {
                    foreach (var p in ChoosePackingListPopupModels)
                    {
                        if (p.CustomerID == "DE")
                        {
                            p.PurchaseOrderNumber = p.ItemStyles.FirstOrDefault()?.PurchaseOrderNumber;
                        }
                        else
                        {
                            var packingListLSStyles = p.LSStyles.Split(";").ToList<string>();
                            var itemStyle = objectSpace.FirstOrDefault<ItemStyle>(i => packingListLSStyles.Contains(i.LSStyle));
                            p.PurchaseOrderNumber = itemStyle?.PurchaseOrderNumber;
                        }
                        result.Add(p);
                    }
                }    
                viewObject.ChoosePackingListPopupModel = result;
                View.Refresh();
            }
        }
        private bool IsExistChoose(List<ChoosePackingListPopupModel> PackingLists,int id)
        {
            bool result = false;
            foreach(var p in PackingLists)
            {
                if (p.ID == id)
                    return true;
            }    
            return result;
        }
        private bool CheckLSStyle(ChoosePackingListPopupModel packing, string lsstyle)
        {
            var lsstyles = packing.LSStyles.Split(";").ToList();
            foreach(var style in lsstyles)
            {
                if (style == lsstyle)
                    return true;
            }
            return false;
        }
        private void UpdateMasterObject(object masterObject)
        {
            Invoice MasterObject = (Invoice)masterObject;
                       
        }
        private void OnMasterObjectChanged(object sender, System.EventArgs e)
        {
            UpdateMasterObject(((PropertyCollectionSource)sender).MasterObject);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            PropertyCollectionSource collectionSource = View.CurrentObject as PropertyCollectionSource;
            if (collectionSource != null)
            {
                collectionSource.MasterObjectChanged += OnMasterObjectChanged;
                if (collectionSource.MasterObject != null)
                {
                    UpdateMasterObject(collectionSource.MasterObject);
                }
            }
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
