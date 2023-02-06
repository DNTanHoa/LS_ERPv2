using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class CountChoosePackingList_Win_ViewController
    : ObjectViewController<ListView, ChoosePackingListPopupModel>
    {
        public CountChoosePackingList_Win_ViewController()
        {
            TargetObjectType = typeof(ChoosePackingListPopupModel);
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

        private void gridview_CustomCount(object sender, CustomSummaryEventArgs e)
        {
            GridView gridView = sender as GridView;
            var selectedRows = gridView.GetSelectedRows();
            List<ChoosePackingListPopupModel> rows = new List<ChoosePackingListPopupModel>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as ChoosePackingListPopupModel;
                    if (row != null)
                        rows.Add(row);
                }
            }

            try
            {
                if (rows != null & rows.Count > 0)
                {
                    GridColumnSummaryItem item = e.Item as GridColumnSummaryItem;

                    if (item.Tag?.ToString() == "Count")
                    {
                        var countEntry = rows.Count();
                        e.TotalValue = countEntry;
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
                gridView.CustomSummaryCalculate += gridview_CustomCount;
                gridView.RowClick += gridview_RowClick;

                GridColumnSummaryItem packingListCount = new GridColumnSummaryItem();
                packingListCount.Tag = "Count";
                packingListCount.SummaryType = SummaryItemType.Custom;
                packingListCount.DisplayFormat = "Count: {0:N0}";
                gridView.Columns["LSStyles"]?.Summary?.Clear();
                gridView.Columns["LSStyles"]?.Summary?.Add(packingListCount);

                gridView.OptionsView.ShowFooter = true;
            }
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
