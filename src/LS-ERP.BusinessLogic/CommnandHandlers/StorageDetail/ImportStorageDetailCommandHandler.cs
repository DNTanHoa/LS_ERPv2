using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Global;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class ImportStorageDetailCommandHandler
        : IRequestHandler<ImportStorageDetailCommand, ImportStorageDetailResult>
    {
        private readonly ILogger<ImportStorageDetailCommandHandler> logger;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly SqlServerAppDbContext context;

        public ImportStorageDetailCommandHandler(ILogger<ImportStorageDetailCommandHandler> logger,
            IStorageDetailRepository storageDetailRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.storageDetailRepository = storageDetailRepository;
            this.context = context;
        }
        public async Task<ImportStorageDetailResult> Handle(ImportStorageDetailCommand request, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import storage detail command", DateTime.Now.ToString());
            var result = new ImportStorageDetailResult();

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

                foreach(DataRow row in table.Rows)
                {
                    var importStorageDto = new ImportStorageDto();
                    importStorageDto.ItemID = row["Item ID"]?.ToString();
                    importStorageDto.ItemName = row["Item Name"]?.ToString();
                    importStorageDto.ItemColorCode = row["Item Color Code"]?.ToString();
                    importStorageDto.ItemColorName = row["Item Color Name"]?.ToString();
                    importStorageDto.Specify = row["Specify"]?.ToString();
                    importStorageDto.Position = row["Position"]?.ToString();
                    importStorageDto.CustomerStyle = row["Style"]?.ToString();
                    importStorageDto.LSStyle = row["LSStyle"]?.ToString();
                    importStorageDto.GarmentColorCode = row["Garment Color Code"]?.ToString();
                    importStorageDto.GarmentColorName = row["Garment Color Name"]?.ToString();
                    importStorageDto.GarmentSize = row["Garment Size"]?.ToString();
                    importStorageDto.UnitID = row["Season"]?.ToString();
                    importStorageDto.PurchaseOrderNumber = row["Item ID"]?.ToString();
                    importStorageDto.Quantity = decimal.Parse(row["Quantity"]?.ToString());
                    importStorageDto.Roll = decimal.Parse(row["Roll"]?.ToString());

                    result.Data.Add(importStorageDto);
                }
            }

            try
            {
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
