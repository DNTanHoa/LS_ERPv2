using LS_ERP.XAF.Module.Service.Response;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LS_ERP.XAF.Module.Service
{
    public class OnlineExchangeService
    {
        public bool GetOnlineExchangeViaVietComBank(string sourceCurrency, string destionationCurrency,
            out decimal value, out string errorMessage)
        {
            value = 0;
            errorMessage = string.Empty;

            using (var client = new HttpClient())
            {
                var url = ConfigurationManager.AppSettings.Get("VietComBankExchangeUrl").ToString();
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    XmlSerializer serializer = new XmlSerializer(typeof(ExrateList));

                    using (TextReader reader = new StringReader(responseContent))
                    {
                        var exrates = serializer.Deserialize(reader) as ExrateList;

                        var destinationExrate = exrates.Exrate.FirstOrDefault(x => x.CurrencyCode == destionationCurrency ||
                                                                        destionationCurrency == "VND");

                        if (destionationCurrency == "VND")
                            destinationExrate.Transfer = "1";

                        var sourceExrate = exrates.Exrate.FirstOrDefault(x => x.CurrencyCode == sourceCurrency ||
                                                                        sourceCurrency == "VND");

                        if (sourceCurrency == "VND")
                            sourceExrate.Transfer = "1";


                        if (destinationExrate != null &&
                            sourceExrate != null)
                        {
                            value = decimal.Parse(sourceExrate.Transfer) /
                                    decimal.Parse(destinationExrate.Transfer);

                            return true;
                        }

                        errorMessage = "Can't find exchange rate for currency " + destionationCurrency;

                    }
                }
                else
                {
                    errorMessage = response.ReasonPhrase;
                }
            }

            return false;
        }
    }
}
