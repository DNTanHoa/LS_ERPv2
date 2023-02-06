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
    public partial class SummaryStorageDetail_Win_ViewController 
        : ObjectViewController<ListView, StorageDetail>
    {
        public SummaryStorageDetail_Win_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(StorageDetail);
            TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
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
            List<StorageDetail> rows = new List<StorageDetail>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as StorageDetail;
                    if (row != null)
                        rows.Add(row);
                }
            }

            try
            {
                if (rows != null & rows.Count > 0)
                {
                    GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;

                    if (item.Tag?.ToString() == "OnHandQuantity")
                    {
                        var totalEntryQuantity = rows.Sum(x => x.OnHandQuantity);
                        e.TotalValue = totalEntryQuantity;
                    }

                    if (item.Tag?.ToString() == "RollQuantity")
                    {
                        var totalRollQuantity = rows.Sum(x => x.Roll);
                        e.TotalValue = totalRollQuantity;
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

                GridColumnSummaryItem onHandQuantiyTotal = new GridColumnSummaryItem();
                onHandQuantiyTotal.Tag = "OnHandQuantity";
                onHandQuantiyTotal.SummaryType = SummaryItemType.Custom;
                onHandQuantiyTotal.DisplayFormat = "OnHand: {0:G29}";
                gridView.Columns["ItemName"]?.Summary?.Clear();
                gridView.Columns["ItemName"]?.Summary?.Add(onHandQuantiyTotal);

                GridColumnSummaryItem rollQuantiyTotal = new GridColumnSummaryItem();
                rollQuantiyTotal.Tag = "RollQuantity";
                rollQuantiyTotal.SummaryType = SummaryItemType.Custom;
                rollQuantiyTotal.DisplayFormat = "Roll: {0:G29}";
                gridView.Columns["Roll"]?.Summary?.Clear();
                gridView.Columns["Roll"]?.Summary?.Add(rollQuantiyTotal);

                gridView.OptionsView.ShowFooter = true;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
