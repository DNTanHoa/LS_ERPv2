using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LS_ERP.XAF.Module.Win.Controllers.Receipt
{
    public class FabricDataInfoAction_Win_ViewController : FabricDataInfoAction_ViewController
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            if (View.Id == "FabricPopupModel_FabricPurchaseInfor_ListView")
            {
                var grid = (View as ListView).Control as GridControl;
                grid.PreviewKeyDown += (s, e) =>
                {
                    if (e.Control && e.KeyCode == System.Windows.Forms.Keys.V)
                    {
                        Paste();
                    }
                };

                GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
                if (listEditor != null)
                {
                    GridView gridView = listEditor.GridView;
                    gridView.CustomSummaryCalculate += gridview_CustomSummaryCalculate;
                    gridView.RowClick += gridview_RowClick;

                    GridColumnSummaryItem entryQuantiyTotal = new GridColumnSummaryItem();
                    entryQuantiyTotal.Tag = "EntryQuantity";
                    entryQuantiyTotal.SummaryType = SummaryItemType.Custom;
                    entryQuantiyTotal.DisplayFormat = "Sum Qty: {0:#,###}";
                    gridView.Columns["EntryQuantity"].Summary.Clear();
                    gridView.Columns["EntryQuantity"].Summary.Add(entryQuantiyTotal);
                    gridView.OptionsView.ShowFooter = true;

                    GridColumnSummaryItem entryRollTotal = new GridColumnSummaryItem();
                    entryRollTotal.Tag = "Roll";
                    entryRollTotal.SummaryType = SummaryItemType.Custom;
                    entryRollTotal.DisplayFormat = "Sum Roll: {0:#,###}";
                    gridView.Columns["Roll"].Summary.Clear();
                    gridView.Columns["Roll"].Summary.Add(entryRollTotal);
                    gridView.OptionsView.ShowFooter = true;
                }
            }
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
            List<FabricPurchaseOrderInforData> rows = new List<FabricPurchaseOrderInforData>();

            for (int i = 0; i < selectedRows.Length; i++)
            {
                int selectedRowHandle = selectedRows[i];
                if (selectedRowHandle >= 0)
                {
                    var row = gridView.GetRow(selectedRowHandle) as FabricPurchaseOrderInforData;
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

                    if (item.Tag?.ToString() == "Roll")
                    {
                        var totalRoll = rows.Sum(x => x.Roll);
                        e.TotalValue = totalRoll;
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        void Paste()
        {

            IDataObject iData = Clipboard.GetDataObject();

            if (iData.GetDataPresent(DataFormats.Text))
            {
                var proObj = (View.CollectionSource) as PropertyCollectionSource;
                var fabrics = proObj.MasterObject as FabricPopupModel;

                var text = (string)iData.GetData(DataFormats.Text);
                string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

                int j = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    string[] values = line.Split(new char[] { '\t' });

                    string check = string.Join(',', values);
                    if (!string.IsNullOrEmpty(check))
                    {
                        if (values.Count() >= 4)
                        {
                            if (!string.IsNullOrEmpty(values[0]))
                            {
                                for (int ii = j; ii < fabrics.FabricPurchaseInfor.Count(); ii++)
                                {
                                    fabrics.FabricPurchaseInfor[ii].DyeNumber = values[0];
                                    fabrics.FabricPurchaseInfor[ii].LotNumber = values[1];
                                    fabrics.FabricPurchaseInfor[ii].Roll = decimal.Parse(values[2]);
                                    fabrics.FabricPurchaseInfor[ii].EntryQuantity = decimal.Parse(values[3]);
                                    j++;
                                    break;
                                }
                            }
                        }
                        else if (values.Count() >= 3)
                        {
                            if (!string.IsNullOrEmpty(values[0]))
                            {
                                for (int ii = j; ii < fabrics.FabricPurchaseInfor.Count(); ii++)
                                {
                                    fabrics.FabricPurchaseInfor[ii].DyeNumber = values[0];
                                    fabrics.FabricPurchaseInfor[ii].LotNumber = values[1];
                                    fabrics.FabricPurchaseInfor[ii].Roll = decimal.Parse(values[2]);
                                    j++;
                                    break;
                                }
                            }
                        }
                        else if (values.Count() >= 2)
                        {
                            if (!string.IsNullOrEmpty(values[0]))
                            {
                                for (int ii = j; ii < fabrics.FabricPurchaseInfor.Count(); ii++)
                                {
                                    fabrics.FabricPurchaseInfor[ii].DyeNumber = values[0];
                                    fabrics.FabricPurchaseInfor[ii].LotNumber = values[1];
                                    j++;
                                    break;
                                }
                            }
                        }
                        else if (values.Count() >= 1)
                        {
                            if (!string.IsNullOrEmpty(values[0]))
                            {
                                for (int ii = j; ii < fabrics.FabricPurchaseInfor.Count(); ii++)
                                {
                                    fabrics.FabricPurchaseInfor[ii].DyeNumber = values[0];
                                    j++;
                                    break;
                                }
                            }
                        }
                    }

                }

                proObj.Reload();
                View.Refresh();
            }
        }
    }
}
