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
    public class StorageBinEntryService : IStorageBinEntryService
    {
       
        public async Task<CommonResponseModel<List<StorageBinEntryModel>>> GetBinEntry(string storageCode)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    ConfigService config = new ConfigService();
                    var uri = config.GetHostErpApi();
                    uri += "StorageBinEntrys/with_storage_code?storageCode=" + storageCode;
                    var response = await httpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException();
                    }
                    else
                    {
                        var resultString = await response.Content.ReadAsStringAsync();
                        var result = new CommonResponseModel<List<StorageBinEntryModel>>();
                        result = JsonConvert.DeserializeObject<CommonResponseModel<List<StorageBinEntryModel>>>(resultString);                       
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
