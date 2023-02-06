using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LS_ERP.XAF.Module.Service
{
    public class MaterialTransactionService
    {
        public async Task<MaterialTransactionReportResponse> GetSummaryReport(
            string storageCode, DateTime fromDate,  DateTime toDate)
        {
            var resultResponse = new MaterialTransactionReportResponse();
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetMaterialTransactionReport");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "/" + storageCode + "/" +
                              fromDate.ToString("yyyy-MM-dd") + "/" +
                              toDate.ToString("yyyy-MM-dd");

                    var response = client.GetAsync(url).Result;

                    if(response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<List<MaterialTransactionReportData>>(content);
                        resultResponse.Data = data;
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
