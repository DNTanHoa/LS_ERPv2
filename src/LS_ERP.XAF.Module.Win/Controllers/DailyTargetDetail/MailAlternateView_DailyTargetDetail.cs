using DevExpress.ExpressApp.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.XtraCharts;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Controllers.DomainComponent;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using LS_ERP.Service.Common;
using ViewType = DevExpress.XtraCharts.ViewType;
using System.Windows.Forms;
using System;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LS_ERP.XAF.Module.Win.Controllers
{
    public class MailAlternateView_DailyTargetDetail : DailyTargetDetailMailParam_ViewController
    {
        /// <summary>
        /// For create file excel DailyTargetDetail
        /// </summary>
        public const string TITLE = "LUCKY STAR Daily Sewwing Line Output Report";
        public const string CUSTOMER = "Customer";
        public const string LINE = "Line";
        public const string NUMBER_OF_WORKER = "Number of worker";
        public const string STYLE_NO = "Style NO";
        public const string ITEM = "Item";
        public const string INLINE_DATE = "Inline Date";
        public const string FINAL_SMV = "FINAL SMV";
        public const string TARGET100 = "Target 100%";
        public const string OUTPUT = "OUTPUT";
        public const string BALANCING = "Balancing";
        public const string ACTUAL = "Actual produced minutes";
        public const string TARGET80 = "Target 80%";
        public const string EFFICIENCY_STYLE = "Efficiency Style";
        public const string EFFICIENCY = "Efficiency";
        public const string REMARK = "Remark";
        public const string NO = "No";
        public const string QUANTITY = "Quantity";
        public override void SendViaGmailAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowViewCurrentObject as DailyTargetDetailMailParam;
            var searchParam = View.CurrentObject as DailyTargetDetailSummaryByDate;
            var dailyTargetDetailsService = new DailyTargetDetailService();
            var messageOptions = new MessageOptions();

            if (searchParam.ListDailyTargetDetailSummaryByDate.Count > 0)
            {
                var listOutputTable = searchParam.ListDailyTargetDetailSummaryByDate.AsEnumerable()
                                            .GroupBy(x => new { x.CustomerName, x.WorkCenterName })
                                            .Select(y => new OutputDailyTargetDetailByCustomerDtos()
                                            {
                                                CustomerName = y.Key.CustomerName,
                                                WorkCenterName = y.Key.WorkCenterName,
                                                Quantity = (decimal)y.Sum(t => t.Quantity),
                                            }
                                            ).ToList()
                                            .AsEnumerable()
                                            .GroupBy(g => new { g.CustomerName })
                                            .Select(s => new OutputDailyTargetDetailByCustomerDtos()
                                            {
                                                CustomerName = s.Key.CustomerName,
                                                Quantity = (decimal)s.Sum(t => t.Quantity),
                                                Lines = (int)s.Select(s => s.WorkCenterName).Count(),
                                            }).ToList();

                var resultTopMinJobOutput = new List<OutputDailyTargetDetailByCustomerDtos>();
                var getTopMinJobOutput = searchParam.ListDailyTargetDetailSummaryByDate.AsEnumerable()
                                          .GroupBy(x => new { x.WorkCenterName, })
                                          .Select(y => new OutputDailyTargetDetailByCustomerDtos()
                                          {
                                              WorkCenterName = y.Key.WorkCenterName,
                                              Quantity = (decimal)y.Sum(t => t.Quantity),
                                              Efficiency = (decimal)y.Sum(t => t.Efficiency)
                                          }
                                          ).ToList();
                int count = 0;
                foreach (var item in getTopMinJobOutput.OrderBy(x => x.Efficiency))
                {
                    if (count++ < 3)
                    {
                        resultTopMinJobOutput.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }

                ////Create file excel
                //SaveFileDialog dialog = new SaveFileDialog();
                //string fileName = dialog.FileName;
                //dialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All files|*.*";
                //if (dialog.ShowDialog() == DialogResult.OK)
                //{
                //    var stream = CreateExcelFile(searchParam.ListDetail, resultTopMinJobOutput, listOutputTable);
                //    var buffer = stream as MemoryStream;
                //    File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                //    return;
                //}
                ////

                var date = searchParam.ProduceDate.ToString("MMMM dd yyyy", System.Globalization.CultureInfo.InvariantCulture);
                importParam.Subject = "Daily Production Output Report of " + date;
                var message = new LS_ERP.Service.Common.Message()
                {
                    Destination = importParam.ToAddress,
                    CCs = importParam.CCs != null ? importParam.CCs.Split(";").ToList() : null,
                    BCCs = importParam.BCCs != null ? importParam.BCCs.Split(";").ToList() : null,
                    Subject = importParam.Subject,
                    Body = MailBody(listOutputTable, date),
                    AttachFile = importParam.ImportFilePath,
                };
                var chart = CreateChartControl(resultTopMinJobOutput);
                var alternateView = CreateAlternateView(message.Body, chart);

                var result = dailyTargetDetailsService.SenViaGmailDailyTargetDetailServices(message, alternateView, out string errorMessage);
                if (result)
                {
                    messageOptions = Helpers.Message.GetMessageOptions("Exchange successfully", "Succeess",
                                InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Helpers.Message.GetMessageOptions(errorMessage, "Error",
                                InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Helpers.Message.GetMessageOptions("No data... \nPlease search data before sending mail!", "Error",
                                InformationType.Error, null, 5000);
            }
            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        private static AlternateView CreateAlternateView(string html, ChartControl chart)
        {
            MemoryStream s = new MemoryStream();
            Image image;
            chart.ExportToImage(s, System.Drawing.Imaging.ImageFormat.Png);
            image = Image.FromStream(s);
            image.Save(s, System.Drawing.Imaging.ImageFormat.Png);
            s.Seek(0, SeekOrigin.Begin);
            LinkedResource Img = new LinkedResource(s);
            Img.ContentId = "MyImage";
            Img.ContentType.Name = "TopMinJobOutput.png";
            string str = html + @"  
            <table>              
                <tr>  
                    <td>  
                      <img src=cid:MyImage id='img' alt=''/>   
                    </td>  
                </tr>
            </table>
            <p><i>This email message was auto-generated. Please do not respond. If you need additional help, please contact ERP Team Support. Thank you!</i></p>";
            AlternateView AV =
            AlternateView.CreateAlternateViewFromString(str, null, System.Net.Mime.MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);
            return AV;
        }
        public ChartControl CreateChartControl(List<OutputDailyTargetDetailByCustomerDtos> listOutput)
        {
            ChartControl _chartControl = new ChartControl();
            ChartTitle chartTitle = new ChartTitle();
            chartTitle.Text = "TOP 3 LINES LOWEST EFFICIENCY";
            chartTitle.TextColor = Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));

            Series series1 = new Series("Quantity", ViewType.Bar);
            series1.DataSource = listOutput;
            series1.ArgumentDataMember = "WorkCenterName";
            series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series1.QualitativeSummaryOptions.SummaryFunction = "SUM([Quantity])";

            SideBySideBarSeriesView sideBySideBarSeriesView = new SideBySideBarSeriesView();
            sideBySideBarSeriesView.Color = Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(150)))), ((int)(((byte)(70)))));
            series1.View = sideBySideBarSeriesView;

            Series series2 = new Series("Efficiency", ViewType.Spline);
            series2.DataSource = listOutput;
            series2.ArgumentDataMember = "WorkCenterName";
            series2.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
            series2.QualitativeSummaryOptions.SummaryFunction = "SUM([Efficiency])";
            series2.Label.TextPattern = "{V: 0.00} %";

            XYDiagram xyDiagram = new XYDiagram();
            SecondaryAxisY secondaryAxisY = new SecondaryAxisY();
            LineSeriesView lineSeriesView = new LineSeriesView();
            secondaryAxisY.Name = "Secondary AxisY";
            secondaryAxisY.Title.Text = "Efficiency";
            secondaryAxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            lineSeriesView.AxisYName = "Secondary AxisY";
            lineSeriesView.MarkerVisibility = DevExpress.Utils.DefaultBoolean.True;
            xyDiagram.SecondaryAxesY.AddRange(new SecondaryAxisY[] {
            secondaryAxisY});
            lineSeriesView.Color = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(192)))));
            series2.View = lineSeriesView;
            _chartControl.Titles.AddRange(new ChartTitle[] {
            chartTitle});
            _chartControl.Series.AddRange(new Series[] { series1, series2 });

            XYDiagram diagram = _chartControl.Diagram as XYDiagram;
            diagram.AxisY.Title.Text = "Pcs";
            diagram.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.True;
            ((XYDiagram)_chartControl.Diagram).SecondaryAxesY.Add(secondaryAxisY);
            ((LineSeriesView)series2.View).AxisY = secondaryAxisY;
            _chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            _chartControl.Size = new Size(600, 400);
            return _chartControl;
        }
        public string MailBody(List<OutputDailyTargetDetailByCustomerDtos> listOutput, string date)
        {
            string styleTable = "style = 'border: 1px solid #EDF3ED; border-collapse: collapse; text-align: center; margin: 0px 0px 20px 50px; background-color: #DDEBF6;'";
            string styleRow = "style = 'border: 1px solid #EDF3ED; border-collapse: collapse; text-align: center; margin: 8px 10px; background-color: #DDEBF6;'";
            string styleHead = "style = 'border: 1px solid #EDF3ED; border-collapse: collapse; text-align: center; margin: 8px 10px; background-color: #5A9BD5;'";
            string styleFoot = "style = 'border: 1px solid #EDF3ED; border-collapse: collapse; text-align: center; margin: 8px 10px; background-color: #A9D08F; color: #F51F1F;'";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<body>");
            sb.AppendLine("<b>Dear all Manager,</b>");
            sb.AppendLine($"<p>Please see enclosed LK Daily production out put report of {date} for your reference</p>");
            sb.AppendLine("<table " + styleTable + ">");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tr " + styleHead + ">");
            sb.AppendLine("<th " + styleHead + ">Customer</th>");
            sb.AppendLine("<th " + styleHead + ">Quantity</th>");
            sb.AppendLine("<th " + styleHead + ">Remark</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            for (int i = 0; i < listOutput.Count; i++)
            {
                sb.AppendLine("<tbody>");
                sb.AppendLine("<tr " + styleRow + ">");
                sb.AppendLine("<td " + styleRow + ">" + listOutput[i].CustomerName + "</td>");
                sb.AppendLine("<td " + styleRow + ">" + listOutput[i].Quantity.ToString("N0") + "</td>");
                sb.AppendLine("<td " + styleRow + ">" + listOutput[i].Lines + " Lines</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</tbody>");
            }
            sb.AppendLine("<tfoot>");
            sb.AppendLine("<tr " + styleFoot + ">");
            sb.AppendLine("<th " + styleFoot + ">Total</th>");
            sb.AppendLine("<th " + styleFoot + ">" + listOutput.Sum(x => x.Quantity).ToString("N0") + "</th>");
            sb.AppendLine("<th " + styleFoot + ">" + listOutput.Sum(x => x.Lines) + " Lines</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</tfoot>");
            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        //Create file excel
        private Stream CreateExcelFile(List<DailyTargetDetailSummaryByDateParam> dailyTargetDetailSummaryByDateParams,
            List<OutputDailyTargetDetailByCustomerDtos> topMinOutput, List<OutputDailyTargetDetailByCustomerDtos> tableOutput,
            Stream stream = null, int numberSheet = 0)
        {
            string Author = "Lucky Star VN";

            string Title = dailyTargetDetailSummaryByDateParams.Select(x => x.ProduceDate).FirstOrDefault().ToString("d-MMMM");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                ExcelPackage excel = excelPackage;

                excelPackage.Workbook.Properties.Author = Author;
                excelPackage.Workbook.Properties.Title = Title;
                excelPackage.Workbook.Properties.Comments = "Packing List of Leading Start VN";
                excelPackage.Workbook.Worksheets.Add(Title);
                var workSheet = excelPackage.Workbook.Worksheets[numberSheet];

                var row = 0;
                CreateHeaderPage(workSheet);
                FillDataLine(workSheet, dailyTargetDetailSummaryByDateParams, out row);
                CreateFooter(workSheet, topMinOutput, tableOutput, row);

                workSheet.Cells.AutoFitColumns();
                workSheet.View.ZoomScale = 70;
                workSheet.View.FreezePanes(4, 5);

                workSheet.PrinterSettings.FitToPage = true;
                workSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                workSheet.PrinterSettings.VerticalCentered = true;
                workSheet.PrinterSettings.HorizontalCentered = true;

                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private void CreateHeaderPage(ExcelWorksheet workSheet)
        {
            workSheet.DefaultColWidth = 18;
            workSheet.Row(3).Height = 40;
            using (var range = workSheet.Cells["A1:Q3"])
            {
                range.Style.Font.SetFromFont(new Font("Times New Roman", 14));
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            workSheet.Cells["A1:Q3"].Style.Font.Bold = true;

            workSheet.Cells["A2:Q3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["A2:Q3"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A2:Q3"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A2:Q3"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A2:Q3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A2:Q3"].Style.WrapText = true;

            workSheet.Cells["A1:Q3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A1:Q3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            workSheet.Cells["C1:Q1"].Value = TITLE;
            workSheet.Cells["C1:Q1"].Merge = true;
            workSheet.Cells["C1:Q1"].Style.Font.Size = 28;
            workSheet.Cells["C1:Q1"].Style.Font.Color.SetColor(Color.Blue);

            workSheet.Cells["A2:A3"].Merge = true;
            workSheet.Cells["A2:A3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["B2:B3"].Value = CUSTOMER;
            workSheet.Column(2).Width = 13;
            workSheet.Cells["B2:B3"].Merge = true;
            workSheet.Cells["B2:B3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["C2:C3"].Value = LINE;
            workSheet.Cells["C2:C3"].Merge = true;
            workSheet.Cells["C2:C3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["D2:D3"].Value = NUMBER_OF_WORKER;
            workSheet.Cells["D2:D3"].Merge = true;
            workSheet.Cells["D2:D3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["E2:L2"].Merge = true;
            workSheet.Cells["E2:L2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["E3:E3"].Value = STYLE_NO;
            workSheet.Column(5).Width = 19;
            workSheet.Cells["E3:E3"].Style.Font.Color.SetColor(Color.White);
            workSheet.Cells["E3:E3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["F3:F3"].Value = ITEM;
            workSheet.Cells["F3:F3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["G3:G3"].Value = INLINE_DATE;
            workSheet.Cells["G3:G3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["H3:H3"].Value = FINAL_SMV;
            workSheet.Cells["H3:H3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["I3:I3"].Value = "-";
            workSheet.Cells["I3:I3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9BC2E6"));

            workSheet.Cells["J3:J3"].Value = TARGET100;
            workSheet.Cells["J3:J3"].Style.Font.Color.SetColor(Color.White);
            workSheet.Cells["J3:J3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["K3:K3"].Value = OUTPUT;
            workSheet.Cells["K3:K3"].Style.Font.Color.SetColor(Color.White);
            workSheet.Cells["K3:K3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["L3:L3"].Value = BALANCING;
            workSheet.Cells["L3:L3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["M2:M3"].Value = ACTUAL;
            workSheet.Cells["M2:M3"].Merge = true;
            workSheet.Cells["M2:M3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["N2:N3"].Value = TARGET80;
            workSheet.Cells["N2:N3"].Merge = true;
            workSheet.Cells["N2:N3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

            workSheet.Cells["O2:O3"].Value = EFFICIENCY_STYLE;
            workSheet.Cells["O2:O3"].Merge = true;
            workSheet.Column(15).Width = 11;
            workSheet.Cells["O2:O3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9BC2E6"));

            workSheet.Cells["P2:P3"].Value = EFFICIENCY;
            workSheet.Cells["P2:P3"].Merge = true;
            workSheet.Column(16).Width = 20;
            workSheet.Cells["P2:P3"].Style.Font.Color.SetColor(Color.White);
            workSheet.Cells["P2:P3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#92D050"));

            workSheet.Cells["Q2:Q3"].Value = REMARK;
            workSheet.Cells["Q2:Q3"].Merge = true;
            workSheet.Column(17).Width = 24;
            workSheet.Cells["Q2:Q3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));
        }
        private void FillDataLine(ExcelWorksheet workSheet, List<DailyTargetDetailSummaryByDateParam> dailyTargetDetailSummaryByDateParams, out int row)
        {
            row = 4;
            var lines = dailyTargetDetailSummaryByDateParams.AsEnumerable()
                                          .GroupBy(x => new
                                          {
                                              x.WorkCenterName,
                                              x.CustomerName,
                                              x.NumberOfWorker,
                                              x.StyleNO,
                                              x.SMV,
                                              x.InlineDate,
                                              x.Remark
                                          })
                                          .Select(y => new DailyTargetDetailSummaryByDateParam()
                                          {
                                              WorkCenterName = y.Key.WorkCenterName,
                                              CustomerName = y.Key.CustomerName,
                                              NumberOfWorker = y.Key.NumberOfWorker,
                                              StyleNO = y.Key.StyleNO,
                                              SMV = y.Key.SMV,
                                              InlineDate = y.Key.InlineDate,
                                              Remark = y.Key.Remark,
                                              Quantity = (decimal)y.Sum(t => t.Quantity),
                                              Efficiency = (decimal)y.Sum(t => t.Efficiency)
                                          }
                                          ).OrderBy(x => x.WorkCenterName);
            //Drawing lines
            foreach (var item in lines)
            {
                workSheet.Cells["A" + row + ":A" + row].Value = "KNIT";
                workSheet.Cells["B" + row + ":B" + row].Value = item.CustomerName;

                workSheet.Cells["C" + row + ":C" + row].Value = item.WorkCenterName;
                workSheet.Cells["C" + row + ":C" + row].Style.Font.Color.SetColor(Color.Blue);
                workSheet.Cells["C" + row + ":C" + row].Style.Font.Bold = true;
                workSheet.Cells["D" + row + ":D" + row].Value = item.NumberOfWorker;

                if (workSheet.Cells["C" + (row - 1) + ":C" + (row - 1)].Value.ToString().Equals(workSheet.Cells["C" + row + ":C" + row].Value.ToString()))
                {
                    workSheet.Cells["C" + (row - 1) + ":C" + row].Merge = true;
                    workSheet.Cells["D" + (row - 1) + ":D" + row].Merge = true;
                }

                workSheet.Cells["E" + row + ":E" + row].Value = item.StyleNO;
                workSheet.Cells["F" + row + ":F" + row].Value = "";

                workSheet.Cells["G" + row + ":G" + row].Value = item.InlineDate;
                workSheet.Cells["G" + row + ":G" + row].Style.Numberformat.Format = "d-MMM";

                workSheet.Cells["H" + row + ":H" + row].Value = item.SMV;
                workSheet.Cells["H" + row + ":H" + row].Style.Numberformat.Format = "#0.00";

                workSheet.Cells["I" + row + ":I" + row].Value = Math.Round((decimal)(11 * 60 * item.NumberOfWorker / item.SMV), 0);
                workSheet.Cells["I" + row + ":I" + row].Style.Numberformat.Format = "#,#";

                workSheet.Cells["J" + row + ":J" + row].Value = Math.Round((decimal)(11 * 60 * item.NumberOfWorker / item.SMV), 0); //Target 100%
                workSheet.Cells["J" + row + ":J" + row].Style.Numberformat.Format = "#,#";

                workSheet.Cells["K" + row + ":K" + row].Value = item.Quantity;
                workSheet.Cells["K" + row + ":K" + row].Style.Numberformat.Format = "#,#";
                workSheet.Cells["K" + row + ":K" + row].Style.Font.Color.SetColor(Color.Red);
                workSheet.Cells["K" + row + ":K" + row].Style.Font.Bold = true;

                workSheet.Cells["L" + row + ":L" + row].Value = item.Quantity - Math.Round((decimal)(11 * 60 * item.NumberOfWorker / item.SMV), 0);
                workSheet.Cells["L" + row + ":L" + row].Style.Numberformat.Format = "#,#";

                workSheet.Cells["M" + row + ":M" + row].Value = item.Quantity * item.SMV;
                workSheet.Cells["M" + row + ":M" + row].Style.Numberformat.Format = "#,#";

                workSheet.Cells["N" + row + ":N" + row].Value = Math.Round((decimal)(11 * 60 * item.NumberOfWorker / item.SMV) / 100 * 80, 0);
                workSheet.Cells["N" + row + ":N" + row].Style.Numberformat.Format = "#,#";

                var _efficiency = item.Quantity / (11 * 60 * item.NumberOfWorker / item.SMV);
                workSheet.Cells["O" + row + ":O" + row].Value = _efficiency;
                workSheet.Cells["O" + row + ":O" + row].Style.Numberformat.Format = "#0.00%";

                workSheet.Cells["P" + row + ":P" + row].Value = _efficiency;
                workSheet.Cells["P" + row + ":P" + row].Style.Numberformat.Format = "#0.00%";
                workSheet.Cells["P" + row + ":P" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                if (_efficiency > 1)
                {
                    workSheet.Cells["P" + row + ":P" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00B050"));
                }
                else if (_efficiency > 0.85M)
                {
                    workSheet.Cells["P" + row + ":P" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00B0F0"));
                }
                else if (_efficiency > 0.75M)
                {
                    workSheet.Cells["P" + row + ":P" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFC000"));
                }
                else
                {
                    workSheet.Cells["P" + row + ":P" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FF0000"));
                }

                workSheet.Cells["Q" + row + ":Q" + row].Value = item.Remark;
                workSheet.Row(row).Height = 30;
                workSheet.Row(row).Style.Font.SetFromFont(new Font("Times New Roman", 13));
                row++;
            }

            //Total end
            workSheet.Cells["B" + row + ":B" + row].Value = "Total";
            workSheet.Cells["C" + row].Value = lines.Select(x=>x.WorkCenterName).Distinct().Count();

            workSheet.Cells["D" + row].Value = lines.Select(x => new { x.WorkCenterName, x.NumberOfWorker }).Distinct().Select(y=>y.NumberOfWorker).Sum();
            workSheet.Cells["D" + row].Calculate();

            workSheet.Cells["H" + row].Formula = "=AVERAGE(H4:H" + (row - 1) + ")";
            workSheet.Cells["H" + row].Calculate();

            workSheet.Cells["J" + row].Formula = "=SUM(J4:J" + (row - 1) + ")";
            workSheet.Cells["J" + row].Calculate();

            workSheet.Cells["K" + row].Formula = "=SUBTOTAL(9,K4:K" + (row - 1) + ")";
            workSheet.Cells["K" + row].Calculate();

            workSheet.Cells["L" + row].Formula = "K" + row + "- J" + row;
            workSheet.Cells["L" + row].Calculate();

            workSheet.Row(row).Height = 40;
            workSheet.Cells["A" + row + ":Q" + row].Style.Font.SetFromFont(new Font("Times New Roman", 18));
            workSheet.Cells["A" + row + ":Q" + row].Style.Font.Bold = true;
            workSheet.Cells["A" + row + ":Q" + row].Style.Font.Color.SetColor(Color.Red);
            workSheet.Cells["A" + row + ":Q" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["A" + row + ":Q" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFF66"));

            workSheet.Cells["A4:Q" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A4:Q" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A4:Q" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A4:Q" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A4:Q" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A4:Q" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        }
        private void CreateFooter(ExcelWorksheet workSheet, List<OutputDailyTargetDetailByCustomerDtos> topMinOutputs,
            List<OutputDailyTargetDetailByCustomerDtos> tableOutputs, int row)
        {
            row += 2;
            int footerRow = row;
            //Top 3 lines lowest efficiency
            workSheet.Cells["B" + row + ":B" + row].Value = NO;
            workSheet.Cells["C" + row + ":C" + row].Value = LINE;
            workSheet.Cells["D" + row + ":D" + row].Value = STYLE_NO;
            workSheet.Cells["E" + row + ":E" + row].Value = ITEM;
            workSheet.Cells["F" + row + ":F" + row].Value = OUTPUT;
            workSheet.Cells["G" + row + ":G" + row].Value = EFFICIENCY;
            workSheet.Cells["H" + row + ":H" + row].Value = REMARK;
            using (var range = workSheet.Cells["B" + row + ":H" + row])
            {
                workSheet.Row(row).Height = 30;
                range.Style.Font.SetFromFont(new Font("Times New Roman", 14));
                range.Style.Font.Bold = true;
            }
            row++;

            for (int i = 0; i < topMinOutputs.Count; i++)
            {
                workSheet.Cells["B" + row + ":B" + row].Value = i + 1;
                workSheet.Cells["C" + row + ":C" + row].Value = topMinOutputs[i].WorkCenterName;
                workSheet.Cells["D" + row + ":D" + row].Value = "";//topMinOutputs[i].StyleNo;
                workSheet.Cells["E" + row + ":E" + row].Value = "item";
                workSheet.Cells["F" + row + ":F" + row].Value = topMinOutputs[i].Quantity;
                workSheet.Cells["G" + row + ":G" + row].Value = topMinOutputs[i].Efficiency;
                workSheet.Cells["H" + row + ":H" + row].Value = "";//topMinOutputs[i].Remark;
                workSheet.Row(row).Height = 30;
                workSheet.Row(row).Style.Font.SetFromFont(new Font("Times New Roman", 13));
                row++;
            }
            workSheet.Cells["B" + footerRow + ":H" + (row - 1)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":H" + (row - 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":H" + (row - 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":H" + (row - 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":H" + (row - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["B" + footerRow + ":H" + (row - 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //Customer table
            workSheet.Cells["B" + row + ":B" + row].Value = CUSTOMER;
            workSheet.Cells["C" + row + ":C" + row].Value = QUANTITY;
            workSheet.Cells["D" + row + ":D" + row].Value = REMARK;
            workSheet.Cells["B" + row + ":D" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["B" + row + ":D" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5B9BD5"));
            using (var range = workSheet.Cells["B" + row + ":D" + row])
            {
                workSheet.Row(row).Height = 20;
                range.Style.Font.SetFromFont(new Font("Times New Roman", 14));
                range.Style.Font.Bold = true;
            }
            row++;
            foreach (var item in tableOutputs)
            {
                workSheet.Cells["B" + row + ":B" + row].Value = item.CustomerName;

                workSheet.Cells["C" + row + ":C" + row].Value = item.Quantity;
                workSheet.Cells["C" + row + ":C" + row].Style.Numberformat.Format = "#,#";

                workSheet.Cells["D" + row + ":D" + row].Value = item.Lines;
                workSheet.Cells["B" + row + ":D" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["B" + row + ":D" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#DDEBF7"));
                workSheet.Row(row).Height = 20;
                workSheet.Row(row).Style.Font.SetFromFont(new Font("Times New Roman", 13));
                row++;
            }
            workSheet.Cells["B" + row + ":B" + row].Value = "Total";
            workSheet.Cells["C" + row + ":C" + row].Value = tableOutputs.Sum(x => x.Quantity);
            workSheet.Cells["C" + row + ":C" + row].Style.Numberformat.Format = "#,#";

            workSheet.Cells["D" + row + ":D" + row].Value = tableOutputs.Sum(x => x.Lines);
            workSheet.Cells["B" + row + ":D" + row].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["B" + row + ":D" + row].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#A9D08E"));
            workSheet.Row(row).Height = 20;
            workSheet.Row(row).Style.Font.SetFromFont(new Font("Times New Roman", 14));
            workSheet.Row(row).Style.Font.Bold = true;
            workSheet.Row(row).Style.Font.Color.SetColor(Color.Red);

            workSheet.Cells["B" + footerRow + ":D" + row].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":D" + row].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":D" + row].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":D" + row].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["B" + footerRow + ":D" + row].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["B" + footerRow + ":D" + row].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        }
    }

}
