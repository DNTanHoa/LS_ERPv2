using Common;
using LS_ERP.Mobile.Models;
using LS_ERP.Mobile.Services.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Mobile.Services.Implement
{
    public class DeliveryNoteService : IDeliveryNoteService
    {
       
        public async Task<CommonResponseModel<List<DeliveryNoteModel>>> GetDeliveryNote(string companyID, string type)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostErpApi();
                    uri += "DeliveryNotes/for_scan_qrcode?CompanyID=" + companyID +"&Type="+type;
                    var response = await httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<List<DeliveryNoteModel>>();
                        result = JsonConvert.DeserializeObject<CommonResponseModel<List<DeliveryNoteModel>>>(resultString);                       
                        return result;
                    }
                }
                catch (Exception exp)
                {
                    // TODO LOG.
                    return null;

                }
            }
        }
    }
}
