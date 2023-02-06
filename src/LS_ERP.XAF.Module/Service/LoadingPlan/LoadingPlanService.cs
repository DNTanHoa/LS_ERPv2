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
    public class LoadingPlanService
    {
        public async Task<ImportLoadingPlanResponse> Import(ImportLoadingPlanRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

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
                        .Get("ImportLoadingPlan").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert
                            .DeserializeObject<ImportLoadingPlanResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new ImportLoadingPlanResponse().SetResult(false, ex.InnerException.Message);
                }
            }
            return null;
        }
        public async Task<BulkLoadingPlanResponse> Bulk(BulkLoadingPlanRequest request)
        {
            var response = new BulkLoadingPlanResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                try
                {
                    var remoteResponse = client.ExecutePost<BulkLoadingPlanResponse>(ConfigurationManager
                        .AppSettings.Get("BulkLoadingPlan").ToString(), request).Result;
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
