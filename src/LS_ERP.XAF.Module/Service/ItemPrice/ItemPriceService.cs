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
    public class ItemPriceService
    {
        public async Task<CommonRespone> CreateMultItemPrice(CreateMultiItemPriceRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new StringContent(JsonConvert.SerializeObject(request),
                    Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(ConfigurationManager.AppSettings.Get("CreateMultiItemPrice").ToString(),
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

        public async Task<ImportItemPriceRespone> ImportItemPrice(ImportItemPriceRequest request)
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
                            ByteArrayContent bytes = new ByteArrayContent(data);
                            requestContent.Add(new StreamContent(new MemoryStream(data)), "ImportFile", Path.GetFileName(request.FilePath));
                        }

                        try
                        {
                            var response = client.PostAsync(ConfigurationManager.AppSettings.Get("ImportItemPrice").ToString(),
                                requestContent).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                var responseData = JsonConvert.DeserializeObject<ImportItemPriceRespone>(responseContent);
                                return responseData;
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ImportItemPriceRespone().SetResult(false, ex.InnerException.Message);
                        }

                        fs.Close();
                    }
                }
            }

            return null;
        }
    }
}
