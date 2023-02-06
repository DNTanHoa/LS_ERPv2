using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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
    public class ImportStorageCommandHandler
        : IRequestHandler<ImportStorageCommand, CommonCommandResultHasData<List<ImportStorageDto>>>
    {
        private readonly ILogger<ImportStorageCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IStorageDetailRepository storageDetailRepository;

        public ImportStorageCommandHandler(ILogger<ImportStorageCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper,
            IStorageDetailRepository storageDetailRepository)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.storageDetailRepository = storageDetailRepository;
        }

        public async Task<CommonCommandResultHasData<List<ImportStorageDto>>> Handle(
            ImportStorageCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import storage detail command with request",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            var result = new CommonCommandResultHasData<List<ImportStorageDto>>();

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

                switch (request.CustomerID)
                {
                    case "DE":
                        {
                            if (request.Output)
                            {
                                var storageDetails = storageDetailRepository.GetOnlyStorageDetailsForCustomer(request.StorageCode, request.CustomerID);
                                ReadDataFabricOutput_DE(ref result, table, storageDetails);
                            }
                            else
                            {
                                ReadDataFabricInput_DE(ref result, table);
                            }

                        }
                        break;
                    case "PU":
                        {
                            ReadDataPU(ref result, table);

                        }
                        break;
                    default:
                        {
                            ReadDataHA(ref result, table);
                        }
                        break;
                }

                if (string.IsNullOrEmpty(result.Message))
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                }
            }

            return result;
        }

        public void ReadDataFabricInput_DE(ref CommonCommandResultHasData<List<ImportStorageDto>> result, DataTable table)
        {
            result.Data = new List<ImportStorageDto>();

            foreach (DataRow row in table.Rows)
            {
                var importStorageDto = new ImportStorageDto();
                importStorageDto.ItemID = row["DK Model Code"]?.ToString().Replace("\n", "");
                importStorageDto.ItemName = row["Comp. Description"]?.ToString().Replace("\n", "");
                importStorageDto.ItemColorCode = row["HB Model Code"]?.ToString().Replace("\n", "");
                importStorageDto.ItemColorName = row["Color - size"]?.ToString().Replace("\n", "");
                importStorageDto.LotNumber = row["LOT NO"]?.ToString();
                importStorageDto.DyeLotNumber = row["DYE LOT"]?.ToString();
                importStorageDto.InvoiceNumber = row["INVOICE NO "]?.ToString();
                importStorageDto.InvoiceNumberNoTotal = row["INVOICE NO TOTAL"]?.ToString();
                importStorageDto.FabricPurchaseOrderNumber = row["FABRIC PO"]?.ToString();
                importStorageDto.CustomerStyle = row["CC"]?.ToString();
                importStorageDto.StorageBinCode = row["LOCATION"]?.ToString();
                importStorageDto.UnitID = row["unit(YDS)"]?.ToString();
                importStorageDto.ProductionMethodCode = row["Season"]?.ToString();
                importStorageDto.Note = row["Note mail Reject"]?.ToString();
                importStorageDto.FabricContent = row["Fabric content"]?.ToString();
                importStorageDto.Zone = row["Zone"]?.ToString();
                importStorageDto.UserFollow = row["BU3"]?.ToString();
                importStorageDto.GarmentSize = row["SIZE"]?.ToString();
                importStorageDto.Supplier = row["Supplier"]?.ToString();
                importStorageDto.Specify = row["Specify"]?.ToString();
                importStorageDto.TransactionDate = DateTime.Parse(row["Reception Date"]?.ToString());

                //if (importStorageDto.FabricPurchaseOrderNumber.Equals("-DM20 RIBBON-305815-FC5DKT-L18B GREYW1F919710118401602296939628*2"))
                //{
                //    string str = " ";
                //}

                if (decimal.TryParse(row["AVAILABLE Stock"]?.ToString(), out decimal Quantity))
                {
                    importStorageDto.Quantity = Quantity;
                }
                else
                {
                    importStorageDto.Quantity = 0;
                }

                if (decimal.TryParse(row["roll stock"]?.ToString(), out decimal Roll))
                {
                    importStorageDto.Roll = Roll;
                }
                else
                {
                    importStorageDto.Roll = 0;
                }
                importStorageDto.Remark = row["Remark"]?.ToString();
                importStorageDto.DocumentNumber = row["Document"]?.ToString();

                if (importStorageDto.InvoiceNumber.Contains("HB"))
                {
                    importStorageDto.Offset = true;
                    importStorageDto.FabricPurchaseOrderNumber = "HB-" + importStorageDto.FabricPurchaseOrderNumber;
                }

                if (!string.IsNullOrEmpty(importStorageDto.Note) &&
                    importStorageDto.Note.ToUpper().Contains("REJECT"))
                {
                    importStorageDto.StorageStatusID = "R";
                }
                else if (!string.IsNullOrEmpty(importStorageDto.Note) &&
                    importStorageDto.Note.ToUpper().Contains("HOLD"))
                {
                    importStorageDto.StorageStatusID = "H";
                }
                //importStorageDto.StorageDetailID = 0;

                result.Data.Add(importStorageDto);
            }
        }

        public void ReadDataFabricOutput_DE(ref CommonCommandResultHasData<List<ImportStorageDto>> result, DataTable table, IQueryable<StorageDetail> storageDetails)
        {
            result.Data = new List<ImportStorageDto>();
            try
            {
                //var dicStorageDetails = new Dictionary<string, StorageDetail>();

                //foreach (var itemDetail in storageDetails)
                //{
                //    string key = (itemDetail.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (itemDetail.ItemID ?? string.Empty).Trim().ToUpper() +
                //                 (itemDetail.ItemName ?? string.Empty).Trim().ToUpper() + (itemDetail.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                //                 (itemDetail.ItemColorName ?? string.Empty).Trim().ToUpper() + (itemDetail.Specify ?? string.Empty).Trim().ToUpper() +
                //                 (itemDetail.LotNumber ?? string.Empty).Trim().ToUpper() + (itemDetail.DyeLotNumber ?? string.Empty).Trim().ToUpper();

                //    if (!dicStorageDetails.ContainsKey(key))
                //    {
                //        dicStorageDetails[key] = itemDetail;
                //    }
                //    else
                //    {
                //        string str = "";
                //    }
                //}

                //var dicStorageDetails = storageDetails?.ToDictionary(x => (x.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (x.ItemID ?? string.Empty).Trim().ToUpper() +
                //                         (x.ItemName ?? string.Empty).Trim().ToUpper() + (x.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                //                         (x.ItemColorName ?? string.Empty).Trim().ToUpper() + (x.Specify ?? string.Empty).Trim().ToUpper() +
                //                         (x.LotNumber ?? string.Empty).Trim().ToUpper() + (x.DyeLotNumber ?? string.Empty).Trim().ToUpper());
                var dicStorageDetails = storageDetails?.ToDictionary(x => x.FabricPurchaseOrderNumber);

                foreach (DataRow row in table.Rows)
                {
                    var importStorageDto = new ImportStorageDto();
                    importStorageDto.ItemID = row["DK Model Code"]?.ToString().Replace("\n", "");
                    importStorageDto.ItemName = row["Comp. Description"]?.ToString().Replace("\n", "");
                    importStorageDto.ItemColorCode = row["HB Model Code"]?.ToString().Replace("\n", "");
                    importStorageDto.ItemColorName = row["Color - size"]?.ToString().Replace("\n", "");
                    importStorageDto.LotNumber = row["LOT NO"]?.ToString();
                    importStorageDto.DyeLotNumber = row["DYE LOT"]?.ToString();
                    importStorageDto.InvoiceNumber = row["INVOICE NO "]?.ToString();
                    importStorageDto.InvoiceNumberNoTotal = row["INVOICE NO TOTAL"]?.ToString();
                    importStorageDto.FabricPurchaseOrderNumber = row["FABRIC PO"]?.ToString();
                    importStorageDto.OutputOrder = row["OUTPUT Order"]?.ToString();
                    //importStorageDto.CustomerStyle = row["CC"]?.ToString();
                    //importStorageDto.StorageBinCode = row["LOCATION"]?.ToString();
                    importStorageDto.UnitID = row["UNIT"]?.ToString();
                    importStorageDto.Season = row["Type"]?.ToString();
                    //importStorageDto.Note = row["Note mail Reject"]?.ToString();
                    //importStorageDto.FabricContent = row["Fabric content"]?.ToString();
                    importStorageDto.Zone = row["Zone"]?.ToString();
                    //importStorageDto.UserFollow = row["BU3"]?.ToString();
                    importStorageDto.GarmentSize = row["SIZE"]?.ToString();
                    importStorageDto.Remark = row["REMARK"]?.ToString();
                    importStorageDto.Specify = row["Specify"]?.ToString();
                    importStorageDto.TransactionDate = DateTime.Parse(row["OUTPUT Date"]?.ToString());

                    if (decimal.TryParse(row["OUTPUT Qty"]?.ToString(), out decimal Quantity))
                    {
                        importStorageDto.Quantity = Quantity;
                    }
                    else
                    {
                        importStorageDto.Quantity = 0;
                    }

                    if (decimal.TryParse(row["Roll out put"]?.ToString(), out decimal Roll))
                    {
                        importStorageDto.Roll = Roll;
                    }
                    else
                    {
                        importStorageDto.Roll = 0;
                    }

                    if (decimal.TryParse(row["Roll No."]?.ToString(), out decimal RollNo))
                    {
                        importStorageDto.RollNo = RollNo;
                    }
                    else
                    {
                        importStorageDto.RollNo = 0;
                    }

                    //importStorageDto.Remark = row["Remark"]?.ToString();
                    importStorageDto.DocumentNumber = row["Document"]?.ToString();

                    //if (importStorageDto.InvoiceNumber.Contains("HB"))
                    //{
                    //    importStorageDto.Offset = true;
                    //    importStorageDto.FabricPurchaseOrderNumber = "HB-" + importStorageDto.FabricPurchaseOrderNumber;
                    //}

                    //if (!string.IsNullOrEmpty(importStorageDto.Note) &&
                    //    importStorageDto.Note.ToUpper().Contains("REJECT"))
                    //{
                    //    importStorageDto.StorageStatusID = "R";
                    //}
                    //else if (!string.IsNullOrEmpty(importStorageDto.Note) &&
                    //    importStorageDto.Note.ToUpper().Contains("HOLD"))
                    //{
                    //    importStorageDto.StorageStatusID = "H";
                    //}

                    var key = importStorageDto.FabricPurchaseOrderNumber;
                    //(importStorageDto.FabricPurchaseOrderNumber ?? string.Empty).Trim().ToUpper() + (importStorageDto.ItemID ?? string.Empty).Trim().ToUpper() +
                    //      (importStorageDto.ItemName ?? string.Empty).Trim().ToUpper() + (importStorageDto.ItemColorCode ?? string.Empty).Trim().ToUpper() +
                    //      (importStorageDto.ItemColorName ?? string.Empty).Trim().ToUpper() + (importStorageDto.Specify ?? string.Empty).Trim().ToUpper() +
                    //      (importStorageDto.LotNumber ?? string.Empty).Trim().ToUpper() + (importStorageDto.DyeLotNumber ?? string.Empty).Trim().ToUpper();

                    //if (importStorageDto.FabricPurchaseOrderNumber.Equals("880975629-BELOUCHADKT-N00A WHITE96081151-2208189196HVD064"))
                    //{
                    //    string str = "";
                    //}

                    if (dicStorageDetails != null && dicStorageDetails.TryGetValue(key, out StorageDetail storageDetail))
                    {
                        importStorageDto.StorageDetailID = storageDetail.ID;
                        importStorageDto.CustomerStyle = storageDetail.CustomerStyle;
                        importStorageDto.GarmentColorCode = storageDetail.GarmentColorCode;
                        importStorageDto.GarmentColorName = storageDetail.GarmentColorName;
                    }

                    result.Data.Add(importStorageDto);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

        }

        public void ReadDataHA(ref CommonCommandResultHasData<List<ImportStorageDto>> result, DataTable table)
        {
            result.Data = new List<ImportStorageDto>();

            foreach (DataRow row in table.Rows)
            {
                var importStorageDto = new ImportStorageDto();
                importStorageDto.ItemID = row["Item ID"]?.ToString();
                importStorageDto.ItemName = row["Item Name"]?.ToString();
                importStorageDto.ItemColorCode = row["Item Color Code"]?.ToString();
                importStorageDto.ItemColorName = row["Item Color Name"]?.ToString();
                importStorageDto.Specify = row["Specify"]?.ToString();
                importStorageDto.Position = row["Position"]?.ToString();
                importStorageDto.StorageBinCode = row["Bin Code"]?.ToString();
                importStorageDto.LotNumber = row["Lot Number"]?.ToString();
                importStorageDto.InvoiceNumber = row["Invoice Number"]?.ToString();
                importStorageDto.PurchaseOrderNumber = row["Purchase Number"]?.ToString();
                importStorageDto.CustomerStyle = row["Style"]?.ToString();
                importStorageDto.LSStyle = row["LS Style"]?.ToString();
                importStorageDto.GarmentColorCode = row["Garment Color Code"]?.ToString();
                importStorageDto.GarmentColorName = row["Garment Color Name"]?.ToString();
                importStorageDto.GarmentSize = row["Garment Size"]?.ToString();
                importStorageDto.UnitID = row["Unit"]?.ToString();
                importStorageDto.Season = row["Season"]?.ToString();
                if (decimal.TryParse(row["Available Stock"]?.ToString(), out decimal Quantity))
                {
                    importStorageDto.Quantity = Quantity;
                }
                else
                {
                    importStorageDto.Quantity = 0;
                }
                if (decimal.TryParse(row["Roll"]?.ToString(), out decimal Roll))
                {
                    importStorageDto.Roll = Roll;
                }
                else
                {
                    importStorageDto.Roll = 0;
                }
                importStorageDto.Remark = row["Remark"]?.ToString();
                importStorageDto.DocumentNumber = row["Document Number"]?.ToString().Replace("'", "");

                result.Data.Add(importStorageDto);
            }
        }

        public void ReadDataPU(ref CommonCommandResultHasData<List<ImportStorageDto>> result, DataTable table)
        {
            result.Data = new List<ImportStorageDto>();

            foreach (DataRow row in table.Rows)
            {
                var importStorageDto = new ImportStorageDto();
                importStorageDto.ItemID = row["Item ID"]?.ToString();
                importStorageDto.ItemName = row["Item Name"]?.ToString();
                importStorageDto.ItemColorCode = row["Item Color Code"]?.ToString();
                importStorageDto.ItemColorName = row["Item Color Name"]?.ToString();
                importStorageDto.Specify = row["Specify"]?.ToString();
                importStorageDto.Position = row["Position"]?.ToString();
                importStorageDto.StorageBinCode = row["Bin Code"]?.ToString();
                importStorageDto.LotNumber = row["Lot Number"]?.ToString();
                importStorageDto.InvoiceNumber = row["Invoice Number"]?.ToString();
                importStorageDto.PurchaseOrderNumber = row["Purchase Number"]?.ToString();
                importStorageDto.CustomerStyle = row["Style"]?.ToString();
                importStorageDto.LSStyle = row["LS Style"]?.ToString();
                importStorageDto.GarmentColorCode = row["Garment Color Code"]?.ToString();
                importStorageDto.GarmentColorName = row["Garment Color Name"]?.ToString();
                importStorageDto.GarmentSize = row["Garment Size"]?.ToString();
                importStorageDto.UnitID = row["Unit"]?.ToString();
                importStorageDto.Season = row["Season"]?.ToString();
                if (decimal.TryParse(row["Available Stock"]?.ToString(), out decimal Quantity))
                {
                    importStorageDto.Quantity = Quantity;
                }
                else
                {
                    importStorageDto.Quantity = 0;
                }
                if (decimal.TryParse(row["Roll"]?.ToString(), out decimal Roll))
                {
                    importStorageDto.Roll = Roll;
                }
                else
                {
                    importStorageDto.Roll = 0;
                }
                importStorageDto.Remark = row["Remark"]?.ToString();
                importStorageDto.DocumentNumber = row["Document Number"]?.ToString().Replace("'", "");

                result.Data.Add(importStorageDto);
            }
        }

        #region support function

        public List<ImportStorageDto> ReadDataACC(ExcelWorksheet worksheet)
        {
            var result = new List<ImportStorageDto>();

            var table = worksheet.ToDataTable(4);

            foreach (DataRow row in table.Rows)
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
                importStorageDto.Quantity = decimal.Parse(row["Available Stock"]?.ToString());
                importStorageDto.Roll = decimal.Parse(row["Roll"]?.ToString());
                importStorageDto.Remark = row["Remarks"]?.ToString();

                result.Add(importStorageDto);
            }

            return result;
        }

        #endregion
    }
}
