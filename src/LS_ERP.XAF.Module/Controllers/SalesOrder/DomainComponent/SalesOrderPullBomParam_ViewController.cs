using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class SalesOrderPullBomParam_ViewController 
        : ObjectViewController<DetailView, SalesOrderPullBomParam>
    {
        public SalesOrderPullBomParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchSalesOrderPullBomAction = 
                new SimpleAction(this, "SearchSalesOrderPullBomAction", PredefinedCategory.Unspecified);
            searchSalesOrderPullBomAction.ImageName = 
                "Action_Search_Object_FindObjectByID";
            searchSalesOrderPullBomAction.Caption = 
                "Search (Ctrl + L)";
            searchSalesOrderPullBomAction.TargetObjectType = 
                typeof(SalesOrderPullBomParam);
            searchSalesOrderPullBomAction.TargetViewType = ViewType.DetailView;
            searchSalesOrderPullBomAction.PaintStyle = 
                ActionItemPaintStyle.CaptionAndImage;
            searchSalesOrderPullBomAction.Shortcut = "CtrlL";

            searchSalesOrderPullBomAction.Execute += 
                SearchSalesOrderPullBomAction_Execute;
        }

        private void SearchSalesOrderPullBomAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var param = View.CurrentObject as SalesOrderPullBomParam;
            if(!string.IsNullOrEmpty(param.Season) ||
               !string.IsNullOrEmpty(param.Styles))
            {
                var criteria = CriteriaOperator
                    .Parse("([Season] = ? OR ?) AND ([CustomerStyle] = ? OR ?)",
                    param.Season, string.IsNullOrEmpty(param.Season),
                    param.Styles, string.IsNullOrEmpty(param.Styles));
                var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria)
                    .ToList();
                param.ItemStyles = itemStyles;
                param.SalesOrders = new List<SalesOrder>();

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
