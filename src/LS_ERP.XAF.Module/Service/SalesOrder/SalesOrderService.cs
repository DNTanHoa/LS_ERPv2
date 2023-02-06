using LS_ERP.XAF.Module.Service.Common;
using LS_ERP.XAF.Module.Service.Response;
using LS_ERP.XAF.Module.Service.SalesOrder.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class SalesOrderService
    {
        public async Task<CommonRespone> ImportSaleOrder(
            ImportSalesOrderRequest request)
        {

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.BrandCode != null)
                    requestContent.Add(new StringContent(request.BrandCode), "BrandCode");

                if (request.ConfirmDate != null)
                    requestContent.Add(new StringContent(request.ConfirmDate.GetValueOrDefault().ToString()), "ConfirmDate");

                if (request.Style != null)
                    requestContent.Add(new StringContent(request.Style), "Style");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            //ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "File",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("ImportSalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<CompareSalesOrderResponse> CompareSaleOrder(
            ImportSalesOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.BrandCode != null)
                    requestContent.Add(new StringContent(request.BrandCode), "BrandCode");

                if (request.ConfirmDate != null)
                    requestContent.Add(new StringContent(request.ConfirmDate.GetValueOrDefault().ToString()), "ConfirmDate");

                if (request.Style != null)
                    requestContent.Add(new StringContent(request.Style), "Style");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (request.IsSaveCompare != null)
                    requestContent.Add(new StringContent(request.IsSaveCompare?.ToString()), "IsSaveCompare");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            //ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "CompareFile",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("CompareSalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CompareSalesOrderResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CompareSalesOrderResponse().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<CompareSalesOrderResponse> SaveCompareSaleOrder(
            ImportSalesOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("SaveCompareSalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CompareSalesOrderResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CompareSalesOrderResponse().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<CommonRespone> UpdateQuantitySaleOrder(
            UpdateSalesOrderQuantityRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "UpdateFile",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("UpdateQuantitySalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<CommonRespone> UpdateSaleOrder(
           UpdateSalesOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (request.ID != null)
                    requestContent.Add(new StringContent(request.ID), "ID");

                if (request.CurrencyID != null)
                    requestContent.Add(new StringContent(request.CurrencyID), "CurrencyID");

                if (request.DivisionID != null)
                    requestContent.Add(new StringContent(request.DivisionID), "DivisionID");

                if (request.PaymentTermCode != null)
                    requestContent.Add(new StringContent(request.PaymentTermCode), "PaymentTermCode");

                if (request.PriceTermCode != null)
                    requestContent.Add(new StringContent(request.PriceTermCode), "PriceTermCode");

                if (request.SalesOrderStatusCode != null)
                    requestContent.Add(new StringContent(request.SalesOrderStatusCode), "SalesOrderStatusCode");

                if (request.SalesOrderOrderTypeCode != null)
                    requestContent.Add(new StringContent(request.SalesOrderOrderTypeCode), "SalesOrderOrderTypeCode");

                if (request.Year != null)
                    requestContent.Add(new StringContent(request.Year?.ToString()), "Year");

                if (request.OrderDate != null)
                    requestContent.Add(new StringContent(request.OrderDate?.ToString()), "OrderDate");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "UpdateFile",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("UpdateSalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<CommonRespone> Delete(string SalesOrderID)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                try
                {
                    var response = client.DeleteAsync(ConfigurationManager.AppSettings
                        .Get("DeleteSalesOrder").ToString()
                        + SalesOrderID).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonRespone()
                            .SetResult(false, response.ReasonPhrase);
                    }

                }
                catch (Exception ex)
                {
                    return new CommonRespone()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<GetPurchaseOrderForSalesOrderResponse>
            GetPurchaseOrderForSalesOrder(string SalesOrderID, string CustomerID, string Style)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                string url = ConfigurationManager.AppSettings
                        .Get("GetPurchaseOrderForSalesOrder").ToString() + "?";

                if (!string.IsNullOrEmpty(SalesOrderID))
                {
                    url += "&customerID=" + CustomerID;
                }

                if (!string.IsNullOrEmpty(SalesOrderID))
                {
                    url += "&saleOrderID=" + SalesOrderID;
                }

                if (!string.IsNullOrEmpty(Style))
                {
                    url += "&style=" + Style;
                }

                try
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<GetPurchaseOrderForSalesOrderResponse>(
                                responseContent);
                        return responseData;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<ImportSalesOrderOffsetResponse> ImportSalesOrderOffset(
            ImportSalesOrderOffsetRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");
                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");
                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "File",
                                Path.GetFileName(request.FilePath));
                        }
                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("ImportSalesOrderOffset").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<ImportSalesOrderOffsetResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new ImportSalesOrderOffsetResponse()
                    {
                        Result = new CommonResult(false, ex.Message)
                    };
                }

            }
            return null;
        }

        public async Task<CommonRespone> SalesOrderOffset(OffsetSalesOrderRequest request)
        {
            var resultResponse = new CommonRespone();

            try
            {
                using (var client = new HttpClient())
                {
                    ConfigurationManager.AppSettings.Get("Server");
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                    client.Timeout = TimeSpan.FromMinutes(15);

                    var requestContent = new StringContent(JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    }), Encoding.UTF8, "application/json");

                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("OffsetSalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }

                }
            }
            catch (Exception ex)
            {
                resultResponse.SetResult(false, ex.Message);
            }

            return null;
        }

        public async Task<CommonRespone> UpdateShipQuantitySaleOrder(
            UpdateSalesOrderShipQuantityRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "UpdateFile",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("UpdateShipQuantitySalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }


        public async Task<CommonRespone> UpdateSampleQuantitySaleOrder(
            UpdateSampleQuantityRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "UpdateFile",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("UpdateSampleQuantitySalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<CommonRespone> UpdateCAC_CHD_EHDSaleOrder(
            UpdateSalesOrderCAC_CHD_EHDRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "UpdateFile",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("UpdateCAC_CHD_EHDSalesOrder").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }
    }
}
