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

namespace LS_ERP.XAF.Module.Service.SalesContract
{
    public class SalesContractService
    {
        public async Task<CommonRespone> ImportSalesContract(ImportSalesContractRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

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

                            try
                            {
                                var response = client.PostAsync(ConfigurationManager.AppSettings
                                    .Get("ImportSalesContract").ToString(),
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

                        fs.Close();
                    }
                }
            }

            return null;
        }
        public async Task<CommonRespone> UpdateSalesContract(UpdateSalesContractRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "Username");

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.SalesContractID != null)
                    requestContent.Add(new StringContent(request.SalesContractID), "SalesContractID");

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

                            try
                            {
                                var response = client.PutAsync(ConfigurationManager.AppSettings
                                    .Get("UpdateSalesContract").ToString(),
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

                        fs.Close();
                    }
                }
            }

            return null;
        }


        public async Task<CommonRespone> UpdateContractInfo(UpdateContractInfoRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "Username");

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

                            try
                            {
                                var response = client.PutAsync(ConfigurationManager.AppSettings
                                    .Get("UpdateContractInfo").ToString(),
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

                        fs.Close();
                    }
                }
            }

            return null;
        }

        public async Task<CommonRespone> UpdatePurchaseOrderNumberSalesContract(UpdateSalesContractRequest request)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                if (request.UserName != null)
                    requestContent.Add(new StringContent(request.UserName), "Username");

                if (request.CustomerID != null)
                    requestContent.Add(new StringContent(request.CustomerID), "CustomerID");

                if (request.SalesContractID != null)
                    requestContent.Add(new StringContent(request.SalesContractID), "SalesContractID");

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

                            try
                            {
                                var response = client.PutAsync(ConfigurationManager.AppSettings
                                    .Get("UpdatePurchaseOrderNumberSalesContract").ToString(),
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

                        fs.Close();
                    }
                }
            }

            return null;
        }

        public async Task<CommonRespone> DeleteSalesContract(string ContractID)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                try
                {
                    var response = client.DeleteAsync(ConfigurationManager.AppSettings
                        .Get("DeleteSalesContract").ToString() + ContractID).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<CommonRespone>(responseContent);
                        return responseData;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    return new CommonRespone().SetResult(false, ex.InnerException.Message);
                }
            }
        }
    }
}
