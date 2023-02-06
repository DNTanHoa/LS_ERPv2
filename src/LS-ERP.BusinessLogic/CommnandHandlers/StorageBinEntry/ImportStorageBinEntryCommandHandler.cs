using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Global;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class ImportStorageBinEntryCommandHandler
        : IRequestHandler<ImportStorageBinEntryCommand, CommonCommandResultHasData<List<ImportStorageBinEntryDto>>>
    {
        private readonly ILogger<ImportStorageBinEntryCommandHandler> logger;
        private readonly SqlServerAppDbContext context;

        public ImportStorageBinEntryCommandHandler(ILogger<ImportStorageBinEntryCommandHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<CommonCommandResultHasData<List<ImportStorageBinEntryDto>>> Handle(
            ImportStorageBinEntryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute import storage bin entry command with request",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            var result = new CommonCommandResultHasData<List<ImportStorageBinEntryDto>>();

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(fullPath);

            using (var package = new ExcelPackage(fileInfo))
            {
                var workSheet = package.Workbook.Worksheets.First();
                var table = workSheet.ToDataTable();
                          
                ReadData(ref result, table, request.CustomerID);

                result.Success = true;
            }

            return result;
        }
        public void ReadData(ref CommonCommandResultHasData<List<ImportStorageBinEntryDto>> result, DataTable table,string customerID)
        {
            var storageBinEntryDtos = new List<ImportStorageBinEntryDto>();

            foreach (DataRow row in table.Rows)
            {
                var PONumber = row["PurchaseOrderNumber"]?.ToString().Trim().ToUpper();
                var itemStyles = context.ItemStyle
                    .Include(x => x.SalesOrder)
                    .Where(x => x.PurchaseOrderNumber.Trim().ToUpper() == PONumber && x.SalesOrder.CustomerID == customerID).ToList();

                itemStyles.ForEach(x => 
                {
                    var importStorageBinEntryDto = new ImportStorageBinEntryDto();
                    importStorageBinEntryDto.PurchaseOrderNumber = PONumber;
                    importStorageBinEntryDto.CustomerStyle = x.CustomerStyle;
                    importStorageBinEntryDto.LSStyle= x.LSStyle;
                    importStorageBinEntryDto.GarmentColorCode = x.ColorCode;
                    importStorageBinEntryDto.GarmentColorName= x.ColorName;
                    importStorageBinEntryDto.Season = x.Season;
                    importStorageBinEntryDto.BinCode = row["BinCode"]?.ToString();

                    storageBinEntryDtos.Add(importStorageBinEntryDto);
                });
            }
            result.Data = storageBinEntryDtos;
        }
    }
}
