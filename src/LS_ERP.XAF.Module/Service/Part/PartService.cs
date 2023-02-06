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
    public class PartService
    {
        public async Task<CommonRespone> Import(
            ImportPartRequest request)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(60);
                ConfigurationManager.AppSettings.Get("Server");

                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "UserName");

                if (request.SalesOrderIDs != null)
                    requestContent.Add(new StringContent(request.SalesOrderIDs), "SalesOrderIDs");

                requestContent.Add(new StringContent(request.Update.ToString()), "Update");

                if (File.Exists(request.FilePath))
                {
                    using (var fs = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read))
                    {

                        byte[] data;

                        using (var br = new BinaryReader(fs, Encoding.UTF8))
                        {

                            data = br.ReadBytes((int)fs.Length);

                            //ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "FilePath",
                                Path.GetFileName(request.FilePath));
                        }

                        fs.Close();
                    }
                }

                try
                {

                    var response = client.PostAsync(ConfigurationManager.AppSettings
                        .Get("ImportPart").ToString(),
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
