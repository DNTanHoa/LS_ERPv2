using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdateSampleQuantityProcess
    {
        public static Dictionary<string, decimal> Import(string filePath, string customerID,
            out List<string> lsStyles,
            out string errorMessage)
        {
            lsStyles = new List<string>();
            errorMessage = string.Empty;
            var result = new Dictionary<string, decimal>();

            switch (customerID)
            {
                case "HA":
                    {
                        result = ReadDataSampleQuantity(filePath, out lsStyles, out errorMessage);
                    }
                    break;
            }

            return result;
        }

        public static Dictionary<string, decimal> ReadDataSampleQuantity(string filePath,
            out List<string> lsStyles,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            lsStyles = new List<string>();

            var result = new Dictionary<string, decimal>();

            if (File.Exists(filePath) &&
                Path.GetExtension(filePath).Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets.Count > 0)
                    {
                        var workSheet = package.Workbook.Worksheets.First();

                        var dicInverseAlphabel = AlphabelColumnExcelHelpers.InverseDictionaryAlphabet(120);

                        var merge = workSheet.MergedCells[2].Split(':');

                        int startColumn = dicInverseAlphabel[merge[0].Replace("2", "")];
                        int endColumn = dicInverseAlphabel[merge[1].Replace("2", "")];


                        var dataTableSize = workSheet.ToDataTable(startHeader: 3, startColumn: startColumn, endColumn: endColumn, startRow: 3);
                        var dataTable = workSheet.ToDataTable(startHeader: 3, startColumn: 1, endColumn: endColumn, startRow: 3);

                        var columnSizeName = dataTableSize.Columns;

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var lsStyle = row["LSStyle"]?.ToString()?.Trim();

                            foreach (var itemSize in columnSizeName)
                            {
                                var size = row[itemSize.ToString()]?.ToString()?.Trim();
                                if (decimal.TryParse(size, out decimal rs))
                                {
                                    result[lsStyle + itemSize.ToString()?.Replace("S_","")] = rs;

                                    lsStyles.Add(lsStyle);
                                }

                            }
                        }
                    }
                    else
                    {
                        errorMessage = "File no data";
                    }
                }
            }
            else
            {
                errorMessage = "File not exist or invalid format";
            }

            return result;
        }

        public static List<OrderDetail> UpdateSampleQuantity(Dictionary<string, decimal> importOrderDetail, List<ItemStyle> oldItemStyle)
        {
            List<OrderDetail> result = new List<OrderDetail>();

            var configOrderDetail = new MapperConfiguration(
            cfg => cfg.CreateMap<OrderDetail, OrderDetail>()
            .ForMember(d => d.ItemStyle, o => o.Ignore()));

            var mapperOrderDetail = new Mapper(configOrderDetail);

            foreach (var itemStyle in oldItemStyle)
            {
                if (itemStyle.Equals("585214-A1"))
                {
                    string str = "";
                }

                foreach (var oldOrderDetail in itemStyle.OrderDetails)
                {
                    if (importOrderDetail.TryGetValue(itemStyle.LSStyle + oldOrderDetail.Size, out decimal sampleQty))
                    {
                        OrderDetail updateOrder = new OrderDetail();
                        mapperOrderDetail.Map(oldOrderDetail, updateOrder);
                        updateOrder.SampleQuantity = sampleQty;

                        result.Add(updateOrder);
                    }
                }
            }

            return result;
        }
    }
}
