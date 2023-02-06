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
    public partial class ReportReceipt_ViewController 
        : ObjectViewController<ListView, ReceiptReportDetail>
    {
        public ReportReceipt_ViewController()
        {
            InitializeComponent();

            TargetObjectType = typeof(ReceiptReportDetail);
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
            List<ReceiptReportDetail> rows = new List<ReceiptReportDetail>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as ReceiptReportDetail;
                    if (row != null)
                        rows.Add(row);
                }
            }

            try
            {
                if (rows != null & rows.Count > 0)
                {
                    GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;

                    if (item.Tag?.ToString() == "ReceiptQuantity")
                    {
                        var totalEntryQuantity = rows.Sum(x => x.ReceiptQuantity);
                        e.TotalValue = totalEntryQuantity;
                    }
                }
            }
            catch (Exception)
            {
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
                entryQuantiyTotal.Tag = "ReceiptQuantity";
                entryQuantiyTotal.SummaryType = SummaryItemType.Custom;
                entryQuantiyTotal.DisplayFormat = "Sum: {0:G29}";
                gridView.Columns["ReceiptQuantity"].Summary.Clear();
                gridView.Columns["ReceiptQuantity"].Summary.Add(entryQuantiyTotal);
                gridView.OptionsView.ShowFooter = true;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
