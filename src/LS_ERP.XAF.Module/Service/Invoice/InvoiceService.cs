using LS_ERP.XAF.Module.Service.Invoice.Response;
using LS_ERP.XAF.Module.Service.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Invoice
{
    public class InvoiceService
    {
        public async Task<UploadDocumentResponse> UploadDocument(
            UploadDocumentRequest request)
        {

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());
                client.Timeout = TimeSpan.FromMinutes(15);
                var requestContent = new MultipartFormDataContent();

                if (request.InvoiceDocumentTypeID != 0)
                    requestContent.Add(new StringContent(request.InvoiceDocumentTypeID.ToString()), "InvoiceDocumentTypeID");

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
                        .Get("UploadDocumentInvoice").ToString(),
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<UploadDocumentResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new UploadDocumentResponse().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }

        public async Task<UpdateInvoiceResponse> UpdateInvoice(
            UpdateInvoiceRequest request)
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
                    string ht = ConfigurationManager.AppSettings
                        .Get("UpdateInvoice").ToString();
                    var response = client.PostAsync(ht,
                        requestContent).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<UpdateInvoiceResponse>(responseContent);
                        return responseData;
                    }
                }
                catch (Exception ex)
                {
                    return new UpdateInvoiceResponse().SetResult(false, ex.InnerException.Message);
                }
            }

            return null;
        }
    }
}
