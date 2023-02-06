using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class SummaryStorageDetailsReport_Win_ViewController
        : ObjectViewController<ListView, StorageDetailsReport>
    {
        public SummaryStorageDetailsReport_Win_ViewController()
        {
            TargetObjectType = typeof(StorageDetailsReport);
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
            List<StorageDetailsReport> rows = new List<StorageDetailsReport>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as StorageDetailsReport;
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

                    if (item.Tag?.ToString() == "InputQuantity")
                    {
                        var totalInputQuantity = rows.Sum(x => x.InputQuantity);
                        e.TotalValue = totalInputQuantity;
                    }

                    if (item.Tag?.ToString() == "OutputQuantity")
                    {
                        var totalOutputQuantity = rows.Sum(x => x.OutputQuantity);
                        e.TotalValue = totalOutputQuantity;
                    }

                    if (item.Tag?.ToString() == "RollNo")
                    {
                        var totalRollNo = rows.Sum(x => x.RollNo);
                        e.TotalValue = totalRollNo;
                    }

                    if (item.Tag?.ToString() == "RollOutput")
                    {
                        var totalRollOutput = rows.Sum(x => x.RollOutput);
                        e.TotalValue = totalRollOutput;
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
                gridView.Columns["OnHandQuantity"]?.Summary?.Clear();
                gridView.Columns["OnHandQuantity"]?.Summary?.Add(onHandQuantiyTotal);

                GridColumnSummaryItem rollQuantiyTotal = new GridColumnSummaryItem();
                rollQuantiyTotal.Tag = "RollQuantity";
                rollQuantiyTotal.SummaryType = SummaryItemType.Custom;
                rollQuantiyTotal.DisplayFormat = "Roll: {0:G29}";
                gridView.Columns["Roll"]?.Summary?.Clear();
                gridView.Columns["Roll"]?.Summary?.Add(rollQuantiyTotal);

                GridColumnSummaryItem inputQuantiyTotal = new GridColumnSummaryItem();
                inputQuantiyTotal.Tag = "InputQuantity";
                inputQuantiyTotal.SummaryType = SummaryItemType.Custom;
                inputQuantiyTotal.DisplayFormat = "InputQuantity: {0:G29}";
                gridView.Columns["InputQuantity"]?.Summary?.Clear();
                gridView.Columns["InputQuantity"]?.Summary?.Add(inputQuantiyTotal);

                GridColumnSummaryItem outputQuantiyTotal = new GridColumnSummaryItem();
                outputQuantiyTotal.Tag = "OutputQuantity";
                outputQuantiyTotal.SummaryType = SummaryItemType.Custom;
                outputQuantiyTotal.DisplayFormat = "OutputQuantity: {0:G29}";
                gridView.Columns["OutputQuantity"]?.Summary?.Clear();
                gridView.Columns["OutputQuantity"]?.Summary?.Add(outputQuantiyTotal);

                GridColumnSummaryItem rollInputTotal = new GridColumnSummaryItem();
                rollInputTotal.Tag = "RollNo";
                rollInputTotal.SummaryType = SummaryItemType.Custom;
                rollInputTotal.DisplayFormat = "Roll Input: {0:G29}";
                gridView.Columns["RollNo"]?.Summary?.Clear();
                gridView.Columns["RollNo"]?.Summary?.Add(rollInputTotal);

                GridColumnSummaryItem rollOutputTotal = new GridColumnSummaryItem();
                rollOutputTotal.Tag = "RollOutput";
                rollOutputTotal.SummaryType = SummaryItemType.Custom;
                rollOutputTotal.DisplayFormat = "Roll Output: {0:G29}";
                gridView.Columns["RollOutput"]?.Summary?.Clear();
                gridView.Columns["RollOutput"]?.Summary?.Add(rollOutputTotal);

                gridView.OptionsView.ShowFooter = true;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
