using LS_ERP.XAF.Module.Service.Respone;
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
    public class PartRevisionService
    {
        public async Task<ImportPartRevisionRespone> ImportPartRevision(ImportPartRevisionRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(20);
                var requestContent = new MultipartFormDataContent();

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");
                if (request.StyleNumber != null)
                    requestContent.Add(new StringContent(request.StyleNumber), "StyleNumber");
                if (request.RevisionNumber != null)
                    requestContent.Add(new StringContent(request.RevisionNumber), "RevisionNumber");
                if (request.EffectDate != null)
                    requestContent.Add(new StringContent(request.EffectDate?.ToString()), "EffectDate");
                if (request.IsConfirmed != null)
                    requestContent.Add(new StringContent(request.IsConfirmed?.ToString()), "IsConfirmed");
                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");
                if (request.Season != null)
                    requestContent.Add(new StringContent(request.Season), "Season");

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
                            var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ImportPartRevision").ToString(),
                                requestContent).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var responseData = JsonConvert.DeserializeObject<ImportPartRevisionRespone>(responseContent);
                                return responseData;
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ImportPartRevisionRespone().SetResult(false, ex.InnerException.Message);
                        }

                        fs.Close();
                    }
                }
            }

            return null;
        }

        public async Task<CommonRespone> CreatePartRevision(CreatePartRevisionRequest request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(15);
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("CreatePartRevision").ToString(),
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
