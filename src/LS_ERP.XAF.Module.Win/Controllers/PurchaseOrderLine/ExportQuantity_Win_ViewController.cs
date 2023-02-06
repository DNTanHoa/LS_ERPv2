using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Controllers;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Win.Dtos;
using LS_ERP.XAF.Module.Win.Dtos.PurchaseOrderLine;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = LS_ERP.XAF.Module.Helpers.Message;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class ExportQuantity_Win_ViewController : ExportQuantityPurchaseOrderLine_ViewController
    {
        public override void ExportQuantityAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            base.ExportQuantityAction_Execute(sender, e);

            var purchaseOrderLineExportQuantity = e.PopupWindowViewCurrentObject as PurchaseOrderLineExportQuantity;

            var purchaseOrder = (((DevExpress.ExpressApp.ListView)View).CollectionSource as PropertyCollectionSource)
                             .MasterObject as PurchaseOrder;

            var objectSpace = Application.CreateObjectSpace(typeof(PurchaseOrder));
            purchaseOrder = objectSpace.GetObjectByKey<PurchaseOrder>(purchaseOrder.ID);

            SaveFileDialog dialog = new SaveFileDialog();

            if (!String.IsNullOrEmpty(purchaseOrder.Number))
            {
                dialog.FileName = purchaseOrder.Number.Replace('/', '-');
            }
            else
            {
                dialog.FileName = purchaseOrder.Number.Replace('/', '-');
            }

            dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";

            MessageOptions options = null;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (purchaseOrderLineExportQuantity.Template)
                    {
                        case Template.Template1:
                            {
                                var stream = CreateExcelFile_ADVN(purchaseOrder, purchaseOrderLineExportQuantity);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                        case Template.Template2:
                            {
                                var stream = CreateExcelFile(purchaseOrder, purchaseOrderLineExportQuantity);
                                var buffer = stream as MemoryStream;

                                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                            }
                            break;
                    }

                    //var stream = CreateExcelFile(purchaseOrder);
                    //var buffer = stream as MemoryStream;

                    //File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                    options = Message.GetMessageOptions("Export successfully. ", "Successs",
                   InformationType.Success, null, 5000);
                }
                catch (Exception EE)
                {
                    options = Message.GetMessageOptions("Export failed. " + EE.Message, "Error",
                   InformationType.Error, null, 5000);

                }
                Application.ShowViewStrategy.ShowMessage(options);
                View.Refresh();
            }
        }

        private Stream CreateExcelFile(PurchaseOrder purchaseOrder,
            PurchaseOrderLineExportQuantity purchaseOrderLineExportQuantity,
            Stream stream = null)
        {
            int row = 1;

            List<ExportQuantity_Dto> list = ListData(purchaseOrder, purchaseOrderLineExportQuantity, out row);

            string Author = "Admin LS";

            if (!String.IsNullOrEmpty(purchaseOrder.CreatedBy))
            {
                Author = purchaseOrder.CreatedBy;

            }

            string Title = purchaseOrder.Number;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {

                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Purchase order of Leading Start";
                excelPackage.Workbook.Worksheets.Add("Sheet_" + Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeader(workSheet, Title);


                workSheet.Cells[2, 1].LoadFromCollection(list, false);

                string modelRangeBorder = "A1:H" + row.ToString();
                using (var range = workSheet.Cells[modelRangeBorder])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.AutoFilter = true;
                    range.AutoFitColumns();
                }

                string modelRangeBorderTitle_Frist = "A1:E1";
                using (var range = workSheet.Cells[modelRangeBorderTitle_Frist])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                }

                string modelRangeBorderTitle_Last = "F1:H1";
                using (var range = workSheet.Cells[modelRangeBorderTitle_Last])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(204, 255, 204));
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        public List<ExportQuantity_Dto> ListData(PurchaseOrder purchaseOrder,
            PurchaseOrderLineExportQuantity purchaseOrderLineExportQuantity, out int row)
        {
            List<ExportQuantity_Dto> list = new List<ExportQuantity_Dto>();

            row = 1;
            Dictionary<string, ExportQuantity_Dto> dicTotal = new Dictionary<string, ExportQuantity_Dto>();
            if (purchaseOrderLineExportQuantity.GroupQty == GroupQtyEnum.Color)
            {
                foreach (PurchaseOrderGroupLine purchaseOrderLine in purchaseOrder.PurchaseOrderGroupLines)
                {
                    ExportQuantity_Dto export_DTO = new ExportQuantity_Dto();
                    if (dicTotal.TryGetValue(purchaseOrderLine.ItemColorCode + purchaseOrderLine.Specify,
                        out ExportQuantity_Dto rs))
                    {
                        export_DTO.ItemID = rs.ItemID;
                        export_DTO.Shipment = rs.Shipment;
                        export_DTO.Incoterm = rs.Incoterm;
                        export_DTO.SupplierCnuf = rs.SupplierCnuf;
                        export_DTO.Currency = "USD";
                        export_DTO.CustomerStyle = rs.CustomerStyle;
                        export_DTO.Comment = rs.Comment;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        export_DTO.Quantity = rs.Quantity + quantity;
                    }
                    else
                    {
                        export_DTO.ItemID = purchaseOrderLine.ItemColorCode;
                        //export_DTO.Quantity = purchaseOrderLine.Quantity;
                        export_DTO.Shipment = purchaseOrder.ShippingMethod?.Name;
                        export_DTO.Incoterm = purchaseOrder.IncoTerm?.Name;
                        export_DTO.SupplierCnuf = purchaseOrder.SupplierCNUFCode;
                        export_DTO.Currency = "USD";
                        export_DTO.CustomerStyle = purchaseOrderLine.CustomerStyle;
                        export_DTO.Comment = purchaseOrderLine.Specify;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        export_DTO.Quantity = quantity;
                    }

                    dicTotal[purchaseOrderLine.ItemColorCode + purchaseOrderLine.Specify] = export_DTO;
                }
            }
            else
            {
                foreach (PurchaseOrderGroupLine purchaseOrderLine in purchaseOrder.PurchaseOrderGroupLines)
                {
                    ExportQuantity_Dto export_DTO = new ExportQuantity_Dto();
                    if (dicTotal.TryGetValue(purchaseOrderLine.ItemID + purchaseOrderLine.Specify,
                        out ExportQuantity_Dto rs))
                    {
                        export_DTO.ItemID = rs.ItemID;
                        export_DTO.Shipment = rs.Shipment;
                        export_DTO.Incoterm = rs.Incoterm;
                        export_DTO.Currency = "USD";
                        export_DTO.SupplierCnuf = rs.SupplierCnuf;
                        export_DTO.CustomerStyle = rs.CustomerStyle;
                        export_DTO.Comment = rs.Comment;
                        //export_DTO.Quantity = rs.Quantity + purchaseOrderLine.Quantity;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        export_DTO.Quantity = rs.Quantity + quantity;
                    }
                    else
                    {
                        export_DTO.ItemID = purchaseOrderLine.ItemID;
                        //export_DTO.Quantity = purchaseOrderLine.Quantity;
                        export_DTO.Shipment = purchaseOrder.ShippingMethod?.Name;
                        export_DTO.Currency = "USD";
                        export_DTO.Incoterm = purchaseOrder.IncoTerm?.Name;
                        export_DTO.SupplierCnuf = purchaseOrder.SupplierCNUFCode;
                        export_DTO.CustomerStyle = purchaseOrderLine.CustomerStyle;
                        export_DTO.Comment = purchaseOrderLine.Specify;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        export_DTO.Quantity = quantity;
                    }

                    dicTotal[purchaseOrderLine.ItemID + purchaseOrderLine.Specify] = export_DTO;
                }
            }

            foreach (var itemData in dicTotal)
            {
                itemData.Value.Quantity = Math.Round((decimal)itemData.Value.Quantity,
                                            purchaseOrderLineExportQuantity.Rounding);
                list.Add(itemData.Value);
                row++;
            }

            return list;
        }

        // template 2
        public void CreateHeader(ExcelWorksheet workSheet, string Title)
        {
            workSheet.Cells[1, 1].Value = "Code DSM, Model or Item";
            workSheet.Cells[1, 1].Style.Font.Bold = true;

            workSheet.Cells[1, 2].Value = "Qty";
            workSheet.Cells[1, 2].Style.Font.Bold = true;

            workSheet.Cells[1, 3].Value = "Supplier Cnuf";
            workSheet.Cells[1, 3].Style.Font.Bold = true;

            workSheet.Cells[1, 4].Value = "Shipment";
            workSheet.Cells[1, 4].Style.Font.Bold = true;

            workSheet.Cells[1, 5].Value = "Incoterm";
            workSheet.Cells[1, 5].Style.Font.Bold = true;

            workSheet.Cells[1, 6].Value = "Currency";
            workSheet.Cells[1, 6].Style.Font.Bold = true;

            workSheet.Cells[1, 7].Value = "FG CC";
            workSheet.Cells[1, 7].Style.Font.Bold = true;

            workSheet.Cells[1, 8].Value = "Comments";
            workSheet.Cells[1, 8].Style.Font.Bold = true;
        }

        private Stream CreateExcelFile_ADVN(PurchaseOrder purchaseOrder,
            PurchaseOrderLineExportQuantity purchaseOrderLineExportQuantity,
            Stream stream = null)
        {
            int row = 1;

            List<ExportQuantityADVN_Dto> list = ListData_ADVN(purchaseOrder,
                                               purchaseOrderLineExportQuantity, out row);

            string Author = "Admin LS";

            if (!String.IsNullOrEmpty(purchaseOrder.CreatedBy))
            {
                Author = purchaseOrder.CreatedBy;

            }

            string Title = purchaseOrder.Number;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {

                ExcelPackage excel = excelPackage;


                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Purch order of Leading Start";
                excelPackage.Workbook.Worksheets.Add("Sheet_" + Title);
                var workSheet = excelPackage.Workbook.Worksheets[0];

                workSheet.Cells.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                workSheet.DefaultColWidth = 12;

                CreateHeader_ADVN(workSheet, Title);


                workSheet.Cells[2, 1].LoadFromCollection(list, false);

                string modelRangeBorder = "A1:B" + row.ToString();
                using (var range = workSheet.Cells[modelRangeBorder])
                {
                    range.Style.Font.SetFromFont(new Font("Times New Roman", 11));
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.AutoFilter = true;
                    range.AutoFitColumns();
                }

                string modelRangeBorderTitle = "A1:B1";
                using (var range = workSheet.Cells[modelRangeBorderTitle])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                excelPackage.Save();
                return excelPackage.Stream;
            }

        }

        // template 1
        public void CreateHeader_ADVN(ExcelWorksheet workSheet, string Title)
        {
            workSheet.Cells[1, 1].Value = "Model or Item";
            workSheet.Cells[1, 1].Style.Font.Bold = true;

            workSheet.Cells[1, 2].Value = "Qty";
            workSheet.Cells[1, 2].Style.Font.Bold = true;
        }

        public List<ExportQuantityADVN_Dto> ListData_ADVN(PurchaseOrder purchaseOrder,
            PurchaseOrderLineExportQuantity purchaseOrderLineExportQuantity, out int row)
        {
            List<ExportQuantityADVN_Dto> list = new List<ExportQuantityADVN_Dto>();

            row = 1;
            Dictionary<string, ExportQuantityADVN_Dto> dicTotal = new Dictionary<string, ExportQuantityADVN_Dto>();

            if (purchaseOrderLineExportQuantity.GroupQty == GroupQtyEnum.Item)
            {
                foreach (PurchaseOrderGroupLine purchaseOrderLine in purchaseOrder.PurchaseOrderGroupLines)
                {
                    ExportQuantityADVN_Dto exportADVN_DTO = new ExportQuantityADVN_Dto();
                    string key = purchaseOrderLine.ItemID + purchaseOrderLine.CustomerStyle + purchaseOrderLine.ItemColorCode;

                    if (dicTotal.TryGetValue(key, out ExportQuantityADVN_Dto rs))
                    {
                        //exportADVN_DTO.Quantity = rs.Quantity + purchaseOrderLine.Quantity;
                        exportADVN_DTO.Color = purchaseOrderLine.ItemID;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        exportADVN_DTO.Quantity = rs.Quantity + quantity;
                    }
                    else
                    {
                        exportADVN_DTO.Color = purchaseOrderLine.ItemID;
                        //exportADVN_DTO.Quantity = purchaseOrderLine.Quantity;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        exportADVN_DTO.Quantity = quantity;
                    }

                    dicTotal[key] = exportADVN_DTO;
                }
            }
            else
            {
                foreach (PurchaseOrderGroupLine purchaseOrderLine in purchaseOrder.PurchaseOrderGroupLines)
                {
                    ExportQuantityADVN_Dto exportADVN_DTO = new ExportQuantityADVN_Dto();

                    string key = purchaseOrderLine.ItemID + purchaseOrderLine.ItemColorCode;

                    if (dicTotal.TryGetValue(key, out ExportQuantityADVN_Dto rs))
                    {
                        //exportADVN_DTO.Quantity = rs.Quantity + purchaseOrderLine.Quantity;
                        exportADVN_DTO.Color = purchaseOrderLine.ItemColorCode;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        exportADVN_DTO.Quantity = rs.Quantity + quantity;
                    }
                    else
                    {
                        exportADVN_DTO.Color = purchaseOrderLine.ItemColorCode;
                        //exportADVN_DTO.Quantity = purchaseOrderLine.Quantity;

                        var quantity = purchaseOrderLine.Quantity;

                        if (purchaseOrderLine.Unit.RoundDown != null && (bool)purchaseOrderLine.Unit.RoundDown)
                        {
                            quantity = Math.Floor((decimal)purchaseOrderLine.Quantity);
                        }
                        else if (purchaseOrderLine.Unit.RoundUp != null && (bool)purchaseOrderLine.Unit.RoundUp)
                        {
                            quantity = Math.Ceiling((decimal)purchaseOrderLine.Quantity);
                        }

                        exportADVN_DTO.Quantity = quantity;
                    }

                    dicTotal[key] = exportADVN_DTO;
                }
            }

            foreach (var itemData in dicTotal)
            {
                itemData.Value.Quantity = Math.Round((decimal)itemData.Value.Quantity,
                                         purchaseOrderLineExportQuantity.Rounding);
                list.Add(itemData.Value);
                row++;
            }

            return list.OrderBy(x => x.Color).ToList();
        }
    }
}
