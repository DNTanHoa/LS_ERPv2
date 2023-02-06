using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public partial class SumPurchaseOrderGroupInforData_ViewController : ObjectViewController<ListView, PurchaseOrderGroupInforData>
    {
        public SumPurchaseOrderGroupInforData_ViewController()
        {
            InitializeComponent();
        }

        private void gridview_RowClick(object sender, RowClickEventArgs e)
        {
            GridView gridView = sender as GridView;
            gridView.UpdateSummary();
        }

        private void gridview_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            GridView gridView = sender as GridView;
            var selectedRows = gridView.GetSelectedRows();
            List<PurchaseOrderGroupInforData> rows = new List<PurchaseOrderGroupInforData>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as PurchaseOrderGroupInforData;
                    if (row != null)
                        rows.Add(row);
                }
            }

            try
            {
                if (rows != null & rows.Count > 0)
                {
                    GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;

                    if (item.Tag?.ToString() == "EntryQuantity")
                    {
                        var totalEntryQuantity = rows.Sum(x => x.EntryQuantity);
                        e.TotalValue = totalEntryQuantity;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            if (listEditor != null)
            {
                GridView gridView = listEditor.GridView;
                gridView.CustomSummaryCalculate += gridview_CustomSummaryCalculate;
                gridView.RowClick += gridview_RowClick;

                GridColumnSummaryItem entryQuantiyTotal = new GridColumnSummaryItem();
                entryQuantiyTotal.Tag = "EntryQuantity";
                entryQuantiyTotal.SummaryType = SummaryItemType.Custom;
                entryQuantiyTotal.DisplayFormat = "Sum: {0:G29}";
                gridView.Columns["EntryQuantity"]?.Summary?.Clear();
                gridView.Columns["EntryQuantity"]?.Summary?.Add(entryQuantiyTotal);
                gridView.OptionsView.ShowFooter = true;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
