using LS_ERP.EntityFrameworkCore.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process
{
    public class UpdateSalesContractProcess
    {
        public static void Update(SalesContract currentSalesContract, string filePath, string fileName, string userName,
            IQueryable<SalesContractDetail> existSaleContractDetail, string customerID, IQueryable<Part> dicParts,
            out List<Part> newParts, ref List<SalesContractDetail> newSalesContractDetail, out string errorMessage)
        {
            newParts = new List<Part>();
            errorMessage = string.Empty;

            if (!String.IsNullOrEmpty(customerID))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            switch (customerID)
                            {
                                case "PU":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();
                                        workSheet.UpdateSalesContractPU(currentSalesContract, userName, existSaleContractDetail.ToList(),
                                        dicParts.ToList(), ref newParts, out newSalesContractDetail, out errorMessage);

                                        if (currentSalesContract != null)
                                        {
                                            currentSalesContract.SetUpdateAudit(userName);
                                            currentSalesContract.SaveFilePath = filePath;
                                            currentSalesContract.FileName = fileName;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            errorMessage = "Not found sheet";
                        }
                    }
                }
            }
        }

        public static void UpdatePurchaseOrderNumberSalesContract(IQueryable<SalesContractDetail> currentSaleContractDetail,
            IQueryable<ItemStyle> itemStyles,
            string filePath, string fileName, string userName,
            string customerID,
            out List<SalesContractDetail> newSalesContractDetail,
            out List<ItemStyle> updateItemStyle,
            out string errorMessage)
        {
            errorMessage = String.Empty;
            updateItemStyle = new List<ItemStyle>();
            newSalesContractDetail = new List<SalesContractDetail>();
            if (!String.IsNullOrEmpty(customerID))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                FileInfo fileInfo = new FileInfo(filePath);

                if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
                {
                    using (var package = new ExcelPackage(fileInfo))
                    {
                        if (package.Workbook.Worksheets.Count > 0)
                        {
                            switch (customerID)
                            {
                                case "PU":
                                    {
                                        var workSheet = package.Workbook.Worksheets.First();
                                        newSalesContractDetail = workSheet.ReadDataUpdatePONumberSalesContractPU(
                                                            currentSaleContractDetail, itemStyles,
                                                            userName, out updateItemStyle, out errorMessage);

                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            errorMessage = "Not found sheet";
                        }
                    }
                }
            }
        }
    }
}
