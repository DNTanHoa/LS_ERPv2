using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraPivotGrid;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Win.Controller.View
{
    public partial class OrderDetail_PivotGrid_SizeSort_ViewController : ViewController
    {
        public OrderDetail_PivotGrid_SizeSort_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(OrderDetail);
            TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            PivotGridListEditor pivotGridEditor = ((ListView)View).Editor as PivotGridListEditor;
            if (pivotGridEditor != null)
            {
                PivotGridControl pivotGridControl = pivotGridEditor.PivotGridControl;
                pivotGridControl.CustomFieldSort += PivotGridControl_CustomFieldSort;
            }
        }

        private void PivotGridControl_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
        {
            if(e.Field.FieldName == "Size")
            {
                if(e.SortLocation == PivotSortLocation.Pivot)
                {
                    object orderValue1 = e.GetListSourceColumnValue(e.ListSourceRowIndex1, "SizeSortIndex"),
                        orderValue2 = e.GetListSourceColumnValue(e.ListSourceRowIndex2, "SizeSortIndex");
                    e.Result = Comparer.Default.Compare(orderValue1, orderValue2);
                }
                e.Handled = true;
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
