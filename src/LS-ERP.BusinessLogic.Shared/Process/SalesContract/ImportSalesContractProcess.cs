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
    public class ImportSalesContractProcess
    {
        public static SalesContract Import(string filePath, string fileName, string userName,
            IQueryable<SalesContractDetail> existSaleContractDetail, string customerID, IQueryable<Part> dicParts,
            out List<Part> newParts, out string errorMessage)
        {
            var saleContract = new SalesContract();
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
                                        saleContract = workSheet.ImportSalesContractPU(userName, existSaleContractDetail.ToList(),
                                            dicParts.ToList(), ref newParts, out errorMessage);

                                        if (saleContract != null)
                                        {
                                            saleContract.SetCreateAudit(userName);
                                            saleContract.CustomerID = customerID;
                                            saleContract.SaveFilePath = filePath;
                                            saleContract.FileName = fileName;
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

            return saleContract;
        }
    }
}
