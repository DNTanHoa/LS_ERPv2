using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Extensions;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportShippingPlanCommandHandler
        : IRequestHandler<ImportShippingPlanCommand, CommonCommandResultHasData<List<ShippingPlanDetail>>>
    {
        private readonly ILogger<ImportShippingPlanCommandHandler> logger;
        private readonly SqlServerAppDbContext context;

        public ImportShippingPlanCommandHandler(ILogger<ImportShippingPlanCommandHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public Task<CommonCommandResultHasData<List<ShippingPlanDetail>>> Handle(
            ImportShippingPlanCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<List<ShippingPlanDetail>>();
            logger.LogInformation("{@time} - Execute import shipping plan command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Execute import shipping plan command for user {@user}",
                DateTime.Now.ToString(), request.UserName);

            try
            {
                if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                    out string fullPath, out string subPath))
                {
                    result.Message = "Error saving file";
                    return Task.FromResult(result);
                }

                var shippingPlanDetails = new List<ShippingPlanDetail>();
                var errorMessage = "";
                try
                {
                    shippingPlanDetails = ShippingPlanProcess.ImportShippingPlan(request.CustomerID, fullPath, out errorMessage);
                    if(shippingPlanDetails.Any())
                    {
                        CheckError(ref shippingPlanDetails, fullPath);
                        result.SetResult(true,string.Empty);
                        result.SetData(shippingPlanDetails);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }

        public void CheckError(ref List<ShippingPlanDetail> shippingPlanDetails, string filePath)
        {
            var checkShippingPlanDetails = shippingPlanDetails;
            var itemStyles = context.ItemStyle
                .Where(x => checkShippingPlanDetails.Select(s => s.PurchaseOrderNumber).Contains(x.PurchaseOrderNumber) &&
                            checkShippingPlanDetails.Select(s => s.LSStyle).Contains(x.LSStyle)).ToList();

            foreach(var data in shippingPlanDetails)
            {
                if(itemStyles.Find(i => i.PurchaseOrderNumber + i.LSStyle == data.PurchaseOrderNumber + data.LSStyle) != null)
                {
                    data.IsError = false;
                }
                else
                {
                    data.IsError = true;
                }
                data.FilePath = filePath;
            }
        }
    }
}
