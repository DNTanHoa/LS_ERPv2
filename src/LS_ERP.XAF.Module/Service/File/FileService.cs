using LS_ERP.Ultilities.Helpers;
using LS_ERP.XAF.Module.Service.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class FileService
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
                catch(Exception ex)
                {
                    //LogHelper.Instance.Error("Download file with url {@url} error with message {@mes}",
                    //    url, ex.InnerException?.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="saveToPath"></param>
        /// <returns></returns>
        public string Download(string url, string saveToPath)
        {
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Upload(byte[] fileData, string fileName)
        {
            using (var client = new HttpClient())
            {
                ConfigurationManager.AppSettings.Get("Server");
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("Server").ToString());

                var requestContent = new MultipartFormDataContent();

                requestContent.Add(new StreamContent(new MemoryStream(fileData)), "File", fileName);

                var response = client.PostAsync(ConfigurationManager.AppSettings.Get("UploadFile")
                         .ToString(), requestContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var responseData = JsonConvert
                        .DeserializeObject<UploadFileResponse>(responseContent);
                    return responseData.Data;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string Upload(string filePath)
        {
            return string.Empty;
        }
    }
}
