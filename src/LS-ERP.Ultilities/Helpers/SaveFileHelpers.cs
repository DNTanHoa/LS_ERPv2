using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Ultilities.Helpers
{
    public class SaveFileHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] Dowload(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadData(url);
                }
                catch (Exception ex)
                {
                    //LogHelper.Instance.Error("Download file with url {@url} error with message {@mes}",
                    //    url, ex.InnerException?.Message);
                    return null;
                }
            }
        }

        public static string Upload(byte[] fileData, string fileName, string url)
        {
            using (var client = new HttpClient())
            {

                var requestContent = new MultipartFormDataContent();

                requestContent.Add(new StreamContent(new MemoryStream(fileData)), "File", fileName);

                var response = client.PostAsync(url, requestContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var responseData = JsonConvert
                        .DeserializeObject<dynamic>(responseContent);
                    return responseData.data;
                }
            }

            return string.Empty;
        }

    }
}
