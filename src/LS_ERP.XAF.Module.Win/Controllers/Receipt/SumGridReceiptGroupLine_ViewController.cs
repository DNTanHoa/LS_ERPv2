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
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public partial class SumGridReceiptGroupLine_ViewController 
        : ObjectViewController<ListView, ReceiptGroupLine>
    {
        public SumGridReceiptGroupLine_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(ReceiptGroupLine);
            TargetViewType = ViewType.ListView;
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
            List<ReceiptGroupLine> rows = new List<ReceiptGroupLine>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as ReceiptGroupLine;
                    if (row != null)
                        rows.Add(row);
                }
            }

            try
            {
                if (rows != null & rows.Count > 0)
                {
                    GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;

                    if (item.Tag?.ToString() == "Quantity")
                    {
                        var totalEntryQuantity = rows.Sum(x => x.ReceiptQuantity);
                        e.TotalValue = totalEntryQuantity;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
                entryQuantiyTotal.Tag = "Quantity";
                entryQuantiyTotal.SummaryType = SummaryItemType.Custom;
                entryQuantiyTotal.DisplayFormat = "Sum: {0:G29}";
                gridView.Columns["ItemID"]?.Summary?.Clear();
                gridView.Columns["ItemID"]?.Summary?.Add(entryQuantiyTotal);
                gridView.OptionsView.ShowFooter = true;
            }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
