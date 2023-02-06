using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateLSStyleStorageDetailJob
    {
        private readonly ILogger<UpdateLSStyleStorageDetailJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateLSStyleStorageDetailJob(ILogger<UpdateLSStyleStorageDetailJob> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update LSStyle Storage Detail Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(List<MaterialTransaction> materialTransactions, string customerID, string storageCode)
        {
            try
            {
                var updateStorageDetails = new List<StorageDetail>();   
                var distinctMaterialTrans = materialTransactions
                    .Select(x =>  new { x.PurchaseOrderNumber, x.CustomerStyle, 
                                        x.Season, x.LSStyle }).Distinct().ToList();
                var storgeDetails = context.StorageDetail
                    .Where(x => x.CustomerID == customerID && x.StorageCode == storageCode).ToList();

                foreach (var trans in distinctMaterialTrans)
                {
                    var updateMaterialTrans = materialTransactions
                        .Where(x => x.PurchaseOrderNumber.Trim().ToUpper() == trans.PurchaseOrderNumber.Trim().ToUpper() &&
                                    x.CustomerStyle.Trim().ToUpper() == trans.CustomerStyle.Trim().ToUpper() &&
                                    x.Season.Trim().ToUpper() == trans.Season.Trim().ToUpper() &&
                                    x.LSStyle.Trim().ToUpper() == trans.LSStyle.Trim().ToUpper()).ToList();
                    if(updateMaterialTrans != null)
                    {
                        var firstMaterialTran = updateMaterialTrans.FirstOrDefault();
                        var lsStyles = ConvertLSStyle(firstMaterialTran.PurchaseOrderNumber ?? string.Empty,
                                                      firstMaterialTran.CustomerStyle ?? string.Empty,
                                                      firstMaterialTran.Season ?? string.Empty,
                                                      firstMaterialTran.LSStyle ?? string.Empty);

                        foreach (var data in updateMaterialTrans)
                        {
                            var storgeDetail = storgeDetails
                                .FirstOrDefault(x => x.ID == data.StorageDetailID);
                            if (storgeDetail != null)
                            {
                                storgeDetail.LSStyle = lsStyles;
                                updateStorageDetails.Add(storgeDetail);
                            }
                        }
                    }
                }

                context.StorageDetail.UpdateRange(updateStorageDetails);
                context.SaveChanges();      
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update LS style storage detail job has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update LS style storage detail job has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }

        public string ConvertLSStyle(string purchaseNumber, string customerStyle, string season, string lsStyle)
        {
            var newLSStyle = "";
            var lsStyles = lsStyle.Replace("->", "~").Replace(",", "/").Replace(";", "/").Replace("+", "/").Replace("TO","/");
            if (string.IsNullOrEmpty(purchaseNumber) || string.IsNullOrEmpty(customerStyle) ||
               string.IsNullOrEmpty(season) || string.IsNullOrEmpty(lsStyle) ||
               lsStyle.Trim().ToUpper().Contains("ADD") || lsStyle.Trim().ToUpper().Contains("FC") ||
               lsStyle.Trim().ToUpper().Contains("BS"))
            {
                return "";
            }
            else if (lsStyle.Trim().ToUpper().Contains("ALL") ||
                (lsStyles.Trim().Contains(" ") && !lsStyles.Contains("-") &&
                !lsStyles.Contains("~") && !lsStyles.Contains("/")))
            {
                var existLSStyles = context.PurchaseOrderLine
                                .Include(x => x.PurchaseOrder)
                                .Where(x => x.PurchaseOrder.Number.Trim().ToUpper() == purchaseNumber.Trim().ToUpper() &&
                                            x.CustomerStyle.Trim().ToUpper() == customerStyle.Trim().ToUpper() &&
                                            x.Season.Trim().ToUpper() == season.Trim().ToUpper())
                                .Select(x => x.LSStyle).Distinct();
                foreach (var data in existLSStyles)
                {
                    if (string.IsNullOrEmpty(newLSStyle))
                    {
                        newLSStyle = data;
                    }
                    else
                    {
                        newLSStyle += ";" + data;
                    }
                }
            }
            else if (lsStyles.Trim().Length == 1 ||
                lsStyles.Trim().Split('/')[0].Trim().Length == 1)
            {
                var existLSStyles = context.PurchaseOrderLine
                                .Include(x => x.PurchaseOrder)
                                .Where(x => x.PurchaseOrder.Number.Trim().ToUpper() == purchaseNumber.Trim().ToUpper() &&
                                            x.CustomerStyle.Trim().ToUpper() == customerStyle.Trim().ToUpper() &&
                                            x.Season.Trim().ToUpper() == season.Trim().ToUpper())
                                .Select(x => x.LSStyle).Distinct();
                var list = lsStyles.Trim().Split('/').ToList();
                foreach (var data in existLSStyles)
                {
                    var value = data.Split('-').Last();
                    bool check = false;
                    foreach (var item in list)
                    {
                        if (value.Contains(item))
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        if (string.IsNullOrEmpty(newLSStyle))
                        {
                            newLSStyle = data;
                        }
                        else
                        {
                            newLSStyle += ";" + data;
                        }
                    }
                }
            }
            else
            {
                var character = "A";
                int value = 0;
                int fromValue = 0;
                int toValue = 0;
                var fromCharacter = "";
                var toCharacter = "";
                lsStyles = lsStyles.Replace(customerStyle + "-", "").Replace(customerStyle + " ", "")
                                .Replace(season + "-", "").Replace(season + " ", "");
                var list1 = lsStyles.Split('/');
                foreach (var data in list1)
                {
                    if (data.Count(d => d == '-') > 1)
                    {
                        var list2 = data.Trim().Replace(" ", "").Split('-');
                        foreach (var data2 in list2)
                        {
                            if (!data2.Contains("~"))
                            {
                                if (int.TryParse(data2, out value))
                                {
                                    if (string.IsNullOrEmpty(newLSStyle))
                                    {
                                        newLSStyle = customerStyle + "-" + season + "-" + character + value.ToString();
                                    }
                                    else
                                    {
                                        newLSStyle += ";" + customerStyle + "-" + season + "-" + character + value.ToString();
                                    }
                                }
                                else
                                {
                                    if (int.TryParse(data2.Substring(1, data2.Length - 1), out value))
                                    {
                                        character = data2.Substring(0, 1);
                                        if (string.IsNullOrEmpty(newLSStyle))
                                        {
                                            newLSStyle = customerStyle + "-" + season + "-" + character + value.ToString();
                                        }
                                        else
                                        {
                                            newLSStyle += ";" + customerStyle + "-" + season + "-" + character + value.ToString();
                                        }
                                    }
                                    else if (int.TryParse(data2.Substring(2, data2.Length - 2), out value))
                                    {
                                        character = data2.Substring(0, 2);
                                        value = int.Parse(data2.Substring(2, data2.Length - 2));
                                        if (string.IsNullOrEmpty(newLSStyle))
                                        {
                                            newLSStyle = customerStyle + "-" + season + "-" + character + value.ToString();
                                        }
                                        else
                                        {
                                            newLSStyle += ";" + customerStyle + "-" + season + "-" + character + value.ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var list3 = data2.Trim().Replace(" ", "").Split('~');
                                if (list3.Length > 1)
                                {
                                    if (int.TryParse(list3[0], out value))
                                    {
                                        fromValue = value;
                                    }
                                    else
                                    {
                                        if (int.TryParse(list3[0].Substring(1, list3[0].Length - 1), out value))
                                        {
                                            character = list3[0].Substring(0, 1);
                                            fromValue = value;
                                        }
                                        else if (int.TryParse(list3[0].Substring(2, list3[0].Length - 2), out value))
                                        {
                                            character = list3[0].Substring(0, 2);
                                            fromValue = int.Parse(list3[0].Substring(2, list3[0].Length - 2));
                                        }
                                    }

                                    if (int.TryParse(list3[1], out value))
                                    {
                                        toValue = value;
                                    }
                                    else
                                    {
                                        if (int.TryParse(list3[1].Substring(1, list3[1].Length - 1), out value))
                                        {
                                            character = list3[1].Substring(0, 1);
                                            toValue = value;
                                        }
                                        else if (int.TryParse(list3[1].Substring(2, list3[1].Length - 2), out value))
                                        {
                                            character = list3[1].Substring(0, 2);
                                            toValue = int.Parse(list3[1].Substring(2, list3[1].Length - 2));
                                        }
                                    }
                                }

                                for (int i = fromValue; i <= toValue; i++)
                                {
                                    if (string.IsNullOrEmpty(newLSStyle))
                                    {
                                        newLSStyle = customerStyle + "-" + season + "-" + character + i.ToString();
                                    }
                                    else
                                    {
                                        newLSStyle += ";" + customerStyle + "-" + season + "-" + character + i.ToString();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] list4;
                        if (data.Contains('~'))
                        {
                            list4 = data.Trim().Replace(" ", "").Split('~');
                        }
                        else
                        {
                            list4 = data.Trim().Replace(" ", "").Split('-');
                        }
                        if (list4.Length == 1)
                        {
                            if (int.TryParse(list4[0], out value))
                            {
                                if (string.IsNullOrEmpty(newLSStyle))
                                {
                                    newLSStyle = customerStyle + "-" + season + "-" + character + value.ToString();
                                }
                                else
                                {
                                    newLSStyle += ";" + customerStyle + "-" + season + "-" + character + value.ToString();
                                }
                            }
                            else
                            {
                                if (int.TryParse(list4[0].Substring(1, list4[0].Length - 1), out value))
                                {
                                    character = list4[0].Substring(0, 1);
                                    if (string.IsNullOrEmpty(newLSStyle))
                                    {
                                        newLSStyle = customerStyle + "-" + season + "-" + character + value.ToString();
                                    }
                                    else
                                    {
                                        newLSStyle += ";" + customerStyle + "-" + season + "-" + character + value.ToString();
                                    }
                                }
                                else if (int.TryParse(list4[0].Substring(2, list4[0].Length - 2), out value))
                                {
                                    character = list4[0].Substring(0, 2);
                                    value = int.Parse(list4[0].Substring(2, list4[0].Length - 2));
                                    if (string.IsNullOrEmpty(newLSStyle))
                                    {
                                        newLSStyle = customerStyle + "-" + season + "-" + character + value.ToString();
                                    }
                                    else
                                    {
                                        newLSStyle += ";" + customerStyle + "-" + season + "-" + character + value.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (int.TryParse(list4[0], out value))
                            {
                                fromValue = value;
                                fromCharacter = character;
                            }
                            else
                            {
                                if (int.TryParse(list4[0].Substring(1, list4[0].Length - 1), out value))
                                {
                                    fromCharacter = list4[0].Substring(0, 1);
                                    character = fromCharacter;
                                    fromValue = value;
                                }
                                else if (int.TryParse(list4[0].Substring(2, list4[0].Length - 2), out value))
                                {
                                    fromCharacter = list4[0].Substring(0, 2);
                                    character = fromCharacter;
                                    fromValue = int.Parse(list4[0].Substring(2, list4[0].Length - 2));
                                }
                            }

                            if (int.TryParse(list4[1], out value))
                            {
                                toValue = value;
                                toCharacter = fromCharacter;
                            }
                            else
                            {
                                if (int.TryParse(list4[1].Substring(1, list4[1].Length - 1), out value))
                                {
                                    toCharacter = list4[1].Substring(0, 1);
                                    character = toCharacter;
                                    toValue = value;
                                }
                                else if (int.TryParse(list4[1].Substring(2, list4[1].Length - 2), out value))
                                {
                                    toCharacter = list4[1].Substring(0, 2);
                                    character = toCharacter;
                                    toValue = int.Parse(list4[1].Substring(2, list4[1].Length - 2));
                                }
                            }
                            if(fromCharacter != toCharacter)
                            {
                                newLSStyle = customerStyle + "-" + season + "-" + fromCharacter + fromValue.ToString() +
                                        ";" + customerStyle + "-" + season + "-" + toCharacter + toValue.ToString();
                                character = toCharacter;
                            }
                            else
                            {
                                if(fromValue > toValue)
                                {
                                    var temp = fromValue;
                                    fromValue = toValue;
                                    toValue = temp;
                                }
                                for (int i = fromValue; i <= toValue; i++)
                                {
                                    if (string.IsNullOrEmpty(newLSStyle))
                                    {
                                        newLSStyle = customerStyle + "-" + season + "-" + character + i.ToString();
                                    }
                                    else
                                    {
                                        newLSStyle += ";" + customerStyle + "-" + season + "-" + character + i.ToString();
                                    }
                                }
                                character = toCharacter;
                            }
                        }
                    }
                }
            }

            return newLSStyle;
        }

        public bool CheckLSStyle(string lsStyle)
        {
            bool check = true;
            var lsStyles = lsStyle.Replace("->", "~").Replace(",", "/").Replace(";", "/").Replace("+", "/");
            if (lsStyles.Trim().Length == 1)
                check = false;
            else if (lsStyles.Trim().Split('/')[0].Trim().Length == 1)
                check = false;
            else if (lsStyles.Trim().Contains(" ") && !lsStyles.Contains("-") &&
                    !lsStyles.Contains("~") && !lsStyles.Contains("/"))
                check = false;
            return check;
        }
    }
}


