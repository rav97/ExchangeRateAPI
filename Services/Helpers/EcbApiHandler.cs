using ExchangeRateAPI.Models.ECB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRateAPI.Utility;
using ExchangeRateAPI.Services.Helpers.Interfaces;

namespace ExchangeRateAPI.Services.Helpers
{
    public class EcbApiHandler : IEcbApiHandler
    {
        private readonly IHttpClientFactory _clientFactory;

        public EcbApiHandler(IHttpClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
        }

        public async Task<GenericData> GetExchangeData(Dictionary<string, string> currencies, DateTime fromDate, DateTime toDate)
        {
            string responseData = "";
            var curr = NormalizeInputCurrency(currencies);

            string requestUri = $@"https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.{curr.Item1}.{curr.Item2}.SP00.A?startPeriod={fromDate.ToString("yyyy-MM-dd")}&endPeriod={toDate.ToString("yyyy-MM-dd")}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                responseData = await response.Content.ReadAsStringAsync();

            return responseData.XmlDeserializeFromString<GenericData>();
        }

        private Tuple<string, string> NormalizeInputCurrency(Dictionary<string, string> currencies)
        {
            string fromCurrency = "";
            string toCurrency = "";

            if (currencies.Count > 0)
            {
                int i = 0;
                foreach (var curPair in currencies)
                {
                    if (i == 0)
                    {
                        fromCurrency = curPair.Key;
                        toCurrency = curPair.Value;
                    }
                    else
                    {
                        fromCurrency += $"+{curPair.Key}";
                        toCurrency += $"+{curPair.Value}";
                    }
                    i++;
                }
            }

            return new Tuple<string, string>(fromCurrency, toCurrency);
        }
    }
}
