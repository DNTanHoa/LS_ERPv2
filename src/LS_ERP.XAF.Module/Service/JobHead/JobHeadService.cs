using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Request;
using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class JobHeadService
    {
        public async Task<JobHeadFilterResponse> GetJobHeadFilter(JobHeadFilterRequest request)
        {
            var resultResponse = new JobHeadFilterResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                
                try 
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetFilterJobHead");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?fromDate=" +
                              request.FromDate.ToString("yyyy-MM-dd") + "&toDate=" +
                              request.ToDate.ToString("yyyy-MM-dd") + "&";

                    if (!string.IsNullOrEmpty(request.CustomerID))
                    {
                        url += "customerID=" + request.CustomerID;
                    }

                    if (!string.IsNullOrEmpty(request.Style))
                    {
                        url += "Style=?" + request.Style;
                    }

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<JobHeadFilterResponse>(content);
                        resultResponse.Data = data.Data;
                        resultResponse.IsSuccess = true;
                    }
                    else
                    {
                        resultResponse.ErrorMessage = response.ReasonPhrase;
                    }
                }
                catch(Exception ex)
                {
                    resultResponse.ErrorMessage = ex.Message;
                    return resultResponse;
                }
            }

            return resultResponse;
        }
    }
}
