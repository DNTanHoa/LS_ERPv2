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
    public class PackingListService
    {
        public async Task<ImportPackingListResponse> ImportPackingList(ImportPackingListRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(20);
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
                            //ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "File",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }
                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ImportPackingList").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<ImportPackingListResponse>(responseContent);
                        responseData.Result.IsSuccess = true;
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new ImportPackingListResponse().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }
    }
}
