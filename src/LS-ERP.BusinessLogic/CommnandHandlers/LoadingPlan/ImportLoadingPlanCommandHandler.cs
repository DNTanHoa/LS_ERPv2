using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Global;
using MediatR;
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
    public class ImportLoadingPlanCommandHandler : IRequestHandler<ImportLoadingPlanCommand, CommonCommandResultHasData<List<LoadingPlan>>>
    {
        private readonly ILogger<ImportLoadingPlanCommandHandler> logger;
        private readonly SqlServerAppDbContext context;

        public ImportLoadingPlanCommandHandler(ILogger<ImportLoadingPlanCommandHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        public async Task<CommonCommandResultHasData<List<LoadingPlan>>> Handle(
            ImportLoadingPlanCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute import loading plan command with request",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            var result = new CommonCommandResultHasData<List<LoadingPlan>>();

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

                ReadData(ref result, table);

                result.Success = true;
            }

            return result;
        }
        public void ReadData(ref CommonCommandResultHasData<List<LoadingPlan>> result, DataTable table)
        {
            var loadingPlans = new List<LoadingPlan>();
            var customers = context.Customer;

            foreach (DataRow row in table.Rows)
            {
                var loadingPlan = new LoadingPlan();
                loadingPlan.ID = 0;
                loadingPlan.ContainerNumber = row["Container Number"]?.ToString();
                loadingPlan.ASNumber = row["AS Number"]?.ToString();
                loadingPlan.TiersName = row["Tiers name"]?.ToString();
                loadingPlan.Shu = row["Shu"]?.ToString();
                loadingPlan.OrderNumber = row["Order Number"]?.ToString();
                loadingPlan.ItemCode = row["Item code"]?.ToString();
                if (decimal.TryParse(row["PCB"]?.ToString(), out decimal PCB))
                {
                    loadingPlan.PCB = PCB;
                }
                else
                {
                    loadingPlan.PCB = 0;
                }
                
                loadingPlan.Port = row["Port"].ToString();
                if (int.TryParse(row["Rank"]?.ToString(), out int rank))
                {
                    loadingPlan.Rank = rank;
                }
                else
                {
                    loadingPlan.Rank = 0;
                }
                loadingPlan.ORINumber = row["ORI Number"].ToString();
                if (decimal.TryParse(row["Gross Weight"]?.ToString(), out decimal grossWeight))
                {
                    loadingPlan.GrossWeight = grossWeight;
                }
                else
                {
                    loadingPlan.GrossWeight = 0;
                }
                if (decimal.TryParse(row["Net Weight"]?.ToString(), out decimal netWeight))
                {
                    loadingPlan.NetWeight = netWeight;
                }
                else
                {
                    loadingPlan.NetWeight = 0;
                }
                if (decimal.TryParse(row["Qty"]?.ToString(), out decimal qty))
                {
                    loadingPlan.Quantity = qty;
                }
                else
                {
                    loadingPlan.Quantity = 0;
                }
                loadingPlan.ModelCode = row["Model code"].ToString();
                loadingPlan.Destination = row["Destination"].ToString();
                if (decimal.TryParse(row["Volume"]?.ToString(), out decimal volumn))
                {
                    loadingPlan.Volumn = volumn;
                }
                else
                {
                    loadingPlan.Volumn = 0;
                }
                loadingPlan.Description = row["Description"].ToString();

                loadingPlans.Add(loadingPlan);
            }
            result.Data = loadingPlans;
        }
    }
}

