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
    public class DailyTargetService
    {
        public async Task<ImportDailyTargetRespone> ImportDailyTarget(ImportDailyTargetRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(20);
                var requestContent = new MultipartFormDataContent();

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] data;
                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {
                            data = br.ReadBytes((int)fs.Length);
                            ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "ImportFile", Path.GetFileName(request.FilePath));
                        }
                        try
                        {
                            var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ImportDailyTarget").ToString(),
                                requestContent).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var responseData = JsonConvert.DeserializeObject<ImportDailyTargetRespone>(responseContent);
                                return responseData;
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ImportDailyTargetRespone().SetResult(false, ex.InnerException.Message);
                        }
                        fs.Close();
                    }
                }
            }
            return null;
        }
        public Task<GetItemDailyTargetReportResponse> GetDailyTargetReport(string companyID, DateTime produceDate)
        {
            var resultResponse = new GetItemDailyTargetReportResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetDailyTarget");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";
                    if (!string.IsNullOrEmpty(companyID))
                    {
                        url += "companyID=" + companyID;
                    }
                        url += "&produceDate=" + produceDate.ToString("yyyy-MM-dd");
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetItemDailyTargetReportResponse>(content);
                        resultResponse.Data = data.Data;
                        resultResponse.IsSuccess = true;
                    }
                    else
                    {
                        resultResponse.ErrorMessage = response.ReasonPhrase;
                    }
                }
                catch (Exception ex)
                {
                    resultResponse.ErrorMessage = ex.Message;
                    return Task.FromResult(resultResponse);
                }
            }
            return Task.FromResult(resultResponse);
        }
        public async Task<BulkDailyTargetResponse> Bulk(BulkDailyTargetRequest request)
        {
            var response = new BulkDailyTargetResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                try
                {
                    var remoteResponse = client.ExecutePost<BulkDailyTargetResponse>(ConfigurationManager
                        .AppSettings.Get("BulkDailyTarget").ToString(), request).Result;
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
