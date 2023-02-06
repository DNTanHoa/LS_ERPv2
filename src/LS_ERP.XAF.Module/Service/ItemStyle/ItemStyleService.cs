using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.Response;
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
    public class ItemStyleService
    {
        public async Task<CommonRespone> BalanceQuantity(BalanceQuantityItemStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(ConfigurationManager.AppSettings.Get("ImportSaleOrder").ToString(),
                        requestContent);

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
        public async Task<CommonRespone> CalculateRequiredQuantity(CalculateRequiredQuantityItemStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("CalculateRequiredQuantity").ToString(),
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
        public async Task<CommonRespone> ClearingBom(ClearingBomRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(ConfigurationManager.AppSettings.Get("ClearingBom").ToString(),
                        requestContent);

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
        public async Task<CommonRespone> ClearingStyle(ClearingStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ClearingStyle").ToString(),
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
        public async Task<CommonRespone> PullBom(PullBomItemStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(20);

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("PullBom").ToString(),
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
        public async Task<CommonRespone> PullBomType(PullBomTypeItemStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("PullBomType").ToString(),
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
        public async Task<CommonRespone> BulkCancel(BulkCancelItemStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ItemStyleBulkCancel").ToString(),
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
        public async Task<CommonRespone> BulkIssue(BulkIssueItemStyleRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ItemStyleBulkIssue").ToString(),
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
        public async Task<CommonRespone> BulkUpdateInfor(BulkUpdateInforRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("ItemStyleBulkUpdateInfor").ToString(), requestContent).Result;

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
        public async Task<ImportUpdateInforResponse> UpdateInfor(ImportUpdateInforRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

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
                            ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "File", Path.GetFileName(request.FilePath));
                        }

                        try
                        {
                            var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ImportUpdateInfor").ToString(),
                                requestContent).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var responseData = JsonConvert.DeserializeObject<ImportUpdateInforResponse>(responseContent);
                                return responseData;
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ImportUpdateInforResponse().SetResult(false, ex.InnerException.Message);
                        }

                        fs.Close();
                    }
                }
            }

            return null;
        }
        public async Task<GetMaterialResponse> GetMaterial(string styles)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings
                    .Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);

                string url = ConfigurationManager.AppSettings
                        .Get("GetMaterialOfStyles").ToString() + "?";

                if (!string.IsNullOrEmpty(styles))
                {
                    url += "&styles=" + styles;
                }

                try
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<GetMaterialResponse>(
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
    }
}
