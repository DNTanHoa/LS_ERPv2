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
    public class ImportBoxInfoCommandHandler : IRequestHandler<ImportBoxInfoCommand, CommonCommandResultHasData<List<BoxInfo>>>
    {
        private readonly ILogger<ImportBoxInfoCommandHandler> logger;
        private readonly SqlServerAppDbContext context;

        public ImportBoxInfoCommandHandler(ILogger<ImportBoxInfoCommandHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        public async Task<CommonCommandResultHasData<List<BoxInfo>>> Handle(
            ImportBoxInfoCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute import box info command with request",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            var result = new CommonCommandResultHasData<List<BoxInfo>>();

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var csvExtension = new CSVExtension();
            var Columns = new string[] {"TagID", "GarmentColorCode", "ItemCode","Description","Style","Quantity"};
            DataTable dt = csvExtension.ToDataTable(fullPath, ";", Columns, false);

            ReadData(ref result, dt);

            result.Success = true;

            return result;
        }

        public void ReadData(ref CommonCommandResultHasData<List<BoxInfo>> result, DataTable dt)
        {
            var boxInfos = new List<BoxInfo>();
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    var boxInfo = new BoxInfo();
                    boxInfo.ID = 0;
                    boxInfo.TagID = row["TagID"].ToString().Replace("\"","");
                    boxInfo.GarmentColorCode = row["GarmentColorCode"].ToString().Replace("\"", ""); 
                    boxInfo.ItemCode = row["ItemCode"].ToString().Replace("\"", ""); 
                    boxInfo.Description = row["Description"].ToString().Replace("\"", ""); 
                    if (decimal.TryParse(row["Quantity"].ToString().Replace("\"", ""), out decimal qty))
                    {
                        boxInfo.QuantityPerBox = qty;
                    }
                    else
                    {
                        boxInfo.QuantityPerBox = 0;
                    }

                    var style = row["Style"].ToString().Replace("\"\"", "\""); 
                    if (style.Contains("S:") && style.Contains("CC:"))
                    {
                        var indexSize = style.IndexOf("S:");
                        var indexCustomerStyle = style.IndexOf("CC:");

                        boxInfo.GarmentSize = style.Substring(indexSize + 2, indexCustomerStyle - indexSize - 5);
                        var customerStyle = style.Substring(indexCustomerStyle + 3, style.Length - indexCustomerStyle - 3);
                        if (customerStyle.Length >= 6)
                        {
                            boxInfo.CustomerStyle = customerStyle.Substring(customerStyle.Length - 6, 6);
                        }
                        else
                        {
                            boxInfo.CustomerStyle = customerStyle;
                        }
                    }

                    boxInfos.Add(boxInfo);
                }
                result.Data = boxInfos;
            }
            catch(Exception ex)
            {

            }
        }
    }
}
