using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportSalesOrderOffsetCommandHandler
        : IRequestHandler<ImportSalesOrderOffsetCommand,
            CommonCommandResultHasData<IEnumerable<SalesOrderOffsetDto>>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<ImportSalesOrderOffsetCommandHandler> logger;

        public ImportSalesOrderOffsetCommandHandler(SqlServerAppDbContext context,
            ILogger<ImportSalesOrderOffsetCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Task<CommonCommandResultHasData<IEnumerable<SalesOrderOffsetDto>>> Handle(
            ImportSalesOrderOffsetCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<SalesOrderOffsetDto>>();
            logger.LogInformation("{@time} - Exceute import sales order offset command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute import sales order offset command for user {@user}",
                DateTime.Now.ToString(), request.Username);

            try
            {
                if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                    out string fullPath, out string subPath))
                {
                    result.Message = "Error saving file";
                    return Task.FromResult(result);
                }

                if (File.Exists(fullPath) &&
                    Path.GetExtension(fullPath).Equals(".xlsx"))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    FileInfo fileInfo = new FileInfo(fullPath);

                    using (ExcelPackage package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            var workSheet = package.Workbook.Worksheets.First();
                            var dataTable = workSheet.ToDataTable();
                            
                            var salesOrderOffsetDtos = new List<SalesOrderOffsetDto>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                var salesOrderDto = new SalesOrderOffsetDto()
                                {
                                    Type = row["Type"]?.ToString(),
                                    CustomerStyle = row["Customer Style"]?.ToString(),
                                    SalesOrderID = row["Order No"]?.ToString(),
                                    SourceLSStyle = row["S-LSStyle"]?.ToString(),
                                    TargetLSStyle = row["D-LSStyle"]?.ToString(),
                                    Season = row["Season"]?.ToString(),
                                    GarmentSize = row["Garment Size"]?.ToString(),
                                    ItemID = row["Item ID"]?.ToString(),
                                    ItemName = row["Item Name"]?.ToString(),
                                    ItemColorCode = row["Item Color Code"]?.ToString(),
                                    ItemColorName = row["Item Color Name"]?.ToString(),
                                    Specify = row["Specify"]?.ToString(),
                                    Position = row["position"]?.ToString(),
                                };

                                if(!string.IsNullOrEmpty(row["Offset Quantity"]?.ToString()))
                                {
                                    salesOrderDto.OffsetQuantity = 
                                        decimal.Parse(row["Offset Quantity"]?.ToString());
                                }

                                salesOrderOffsetDtos.Add(salesOrderDto);
                            }

                            result.Data = salesOrderOffsetDtos;
                            result.Success = true;
                        }
                    }
                }
                else
                {
                    result.Message = "Invalid file";
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
