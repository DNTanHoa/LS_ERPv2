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
    public class CuttingCardService : ICuttingCardService
    {
       
        public async Task<CommonResponseModel<List<CuttingCardModel>>> GetCuttingCard(string Id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostErpApi();
                    uri += "CuttingCards/with_id?Id=" + Id;
                    var response = await httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<List<CuttingCardModel>>();
                        result = JsonConvert.DeserializeObject<CommonResponseModel<List<CuttingCardModel>>>(resultString);                       
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
        public async Task<CommonResponseModel<List<CuttingCardModel>>> GetLocationCuttingCard(string Id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostErpApi();
                    uri += "CuttingCards/search_location_with_id?Id=" + Id;
                    var response = await httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<List<CuttingCardModel>>();
                        result = JsonConvert.DeserializeObject<CommonResponseModel<List<CuttingCardModel>>>(resultString);
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
        public async Task<CommonResponseModel<CuttingCardModel>> UpdateCuttingCard(CuttingCardModel cuttingCard)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostErpApi();
                    uri += "CuttingCards";
                    var content = JsonConvert.SerializeObject(cuttingCard);
                    HttpContent contentPut = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync(uri,contentPut);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<CuttingCardModel>();
                        result = JsonConvert.DeserializeObject<CommonResponseModel<CuttingCardModel>>(resultString);
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
        public async Task<CommonResponseModel<CuttingCardModel>> UpdateBulkCuttingCard(BulkCuttingCardModel bulkCuttingCard)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostErpApi();
                    uri += "CuttingCards/multi_cards_location";
                    var content = JsonConvert.SerializeObject(bulkCuttingCard);
                    HttpContent contentPut = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync(uri, contentPut);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<CuttingCardModel>();
                        result = JsonConvert.DeserializeObject<CommonResponseModel<CuttingCardModel>>(resultString);
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
