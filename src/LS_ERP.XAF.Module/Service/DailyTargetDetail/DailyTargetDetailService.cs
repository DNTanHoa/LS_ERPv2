using LS_ERP.Service.Common;
using LS_ERP.Service.Mail;
using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraCharts;

namespace LS_ERP.XAF.Module.Service
{
    public class DailyTargetDetailService
    {
        public async Task<GetItemDailyTargetDetailReportResponse> GetDailyTargetDetailReport(string customerID,
           string style, DateTime fromDate, DateTime toDate)
        {
            var resultResponse = new GetItemDailyTargetDetailReportResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetDailyTargetDetail");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";
                    // fromDate.ToString("yyyy-MM-dd") + "/" +
                    // toDate.ToString("yyyy-MM-dd") + "?";
                    if (!string.IsNullOrEmpty(customerID))
                    {
                        url += "customerID=" + customerID;
                    }
                    if (!string.IsNullOrEmpty(style))
                    {
                        url += "&style=" + style;
                    }
                    url += "&fromDate=" + fromDate.ToString("yyyy-MM-dd");
                    url += "&toDate=" + toDate.ToString("yyyy-MM-dd");

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetItemDailyTargetDetailReportResponse>(content);
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
                    return resultResponse;
                }
            }
            return resultResponse;
        }

        public async Task<GetDailyTargetDetailSummaryByDateResponse> GetDailyTargetDetailSummaryByDate(string customerId,
                    string operation, DateTime produceDate)
        {
            var resultResponse = new GetDailyTargetDetailSummaryByDateResponse();

            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");

                try
                {
                    var baseUrl = ConfigurationManager.AppSettings
                        .Get("GetDailyTargetDetailByOperationDate");
                    var builder = new UriBuilder(ConfigurationManager.AppSettings.Get("Server"));

                    var url = builder + baseUrl + "?";
                    if (!string.IsNullOrEmpty(customerId))
                    {
                        url += "customerID=" + customerId;
                    }
                    if (!string.IsNullOrEmpty(operation))
                    {
                        url += "&operation=" + operation;
                    }

                    url += "&produceDate=" + produceDate.ToString("yyyy-MM-dd");

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var data = JsonConvert
                            .DeserializeObject<GetDailyTargetDetailSummaryByDateResponse>(content);
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
                    return resultResponse;
                }
            }
            return resultResponse;
        }

        public bool SenViaGmailDailyTargetDetailServices(Message message, AlternateView alternateView, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var host = ConfigurationManager.AppSettings.Get("MailHost");
                var port = ConfigurationManager.AppSettings.Get("MailPort");
                var fromAddress = ConfigurationManager.AppSettings.Get("MailFromAddress");
                var password = ConfigurationManager.AppSettings.Get("MailPassword");
                var toAddress = ConfigurationManager.AppSettings.Get("MailToAddress");

                var configSMTP = new ConfigSMTP()
                {
                    Host = host,
                    Port = int.Parse(port),
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    Password = password
                };
                var mailService = new MailService();
                return mailService.SendViaGmail(message, configSMTP, alternateView);                
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }        
    }
}
