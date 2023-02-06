using Common.Model;
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
using Ultils.Extensions;

namespace LS_ERP.XAF.Module.Service
{
    public class ShippingPlanService
    {
        public async Task<ImportShippingPlanResponse> ImportShippingPlan(
            ImportShippingPlanRequest request)
        {

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.CompanyID != null)
                    requestContent.Add(new StringContent(request.CompanyID), "CompanyID");

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
                        .Get("ImportShippingPlan").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<ImportShippingPlanResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new ImportShippingPlanResponse();
                }
            }

            return null;
        }

        public async Task<CommonResponseModel<object>> CreateShippingPlan(
            CreateShippingPlanRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("CreateShippingPlan").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonResponseModel<object>>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonResponseModel<object>()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new CommonResponseModel<object>()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<CommonResponseModel<object>> UpdateShippingPlan(
            UpdateShippingPlanRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    }), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("UpdateShippingPlan").ToString(), requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonResponseModel<object>>(responseContent);
                        return responseData;
                    }
                    else
                    {
                        return new CommonResponseModel<object>()
                            .SetResult(false, response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    return new CommonResponseModel<object>()
                        .SetResult(false, ex.InnerException.Message);
                }
            }
        }

        public async Task<BulkShippingPlanResponse> BulkShippingPlan(BulkShippingPlanRequest request)
        {
            var response = new BulkShippingPlanResponse();
           
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                try
                {
                    var remoteResponse = client.ExecutePost<BulkShippingPlanResponse>(ConfigurationManager
                        .AppSettings.Get("BulkShippingPlan").ToString(), request).Result;
                    return remoteResponse.Data;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }

            return null;
        }
    }
}
