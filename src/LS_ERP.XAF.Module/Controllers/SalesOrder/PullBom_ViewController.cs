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

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PullBom_ViewController 
        : ObjectViewController<ListView, SalesOrder>
    {
        public PullBom_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction pullBomSalesOrder = new PopupWindowShowAction(this,
                "PullBomSalesOrder", PredefinedCategory.Unspecified);
            pullBomSalesOrder.ImageName = "TopDown";
            pullBomSalesOrder.Caption = "Pull Bom Order";
            pullBomSalesOrder.TargetObjectType = typeof(SalesOrder);
            pullBomSalesOrder.TargetViewType = ViewType.ListView;
            pullBomSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            pullBomSalesOrder.CustomizePopupWindowParams += 
                PullBomSalesOrder_CustomizePopupWindowParams;
            pullBomSalesOrder.Execute += PullBomSalesOrder_Execute;
        }

        private void PullBomSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            
        }

        private void PullBomSalesOrder_CustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderPullBomParam()
            {
                SalesOrders = View.SelectedObjects.Cast<SalesOrder>().ToList(),
                ItemStyles = View.SelectedObjects.Cast<SalesOrder>()
                    .SelectMany(x => x.ItemStyles).ToList(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.IsSizeable = true;
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
