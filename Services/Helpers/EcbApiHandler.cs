using ExchangeRateAPI.Models.ECB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRateAPI.Utility;
using ExchangeRateAPI.Services.Helpers.Interfaces;
using ExchangeRateAPI.Mappers;
using ExchangeRateAPI.Models;

namespace ExchangeRateAPI.Services.Helpers
{
    public class EcbApiHandler : IEcbApiHandler
    {
        private readonly IHttpClientFactory _clientFactory;

        public EcbApiHandler(IHttpClientFactory clientFactory)
        {
            this._clientFactory = clientFactory;
        }

        public async Task<List<ExchangeResponseModel>> GetExchangeData(Dictionary<string, string> currencies, DateTime fromDate, DateTime toDate)
        {
            string responseData = "";
            var curr = NormalizeInputCurrency(currencies);

            //adding +/- 3 days to request to get information about currency from days off
            string requestUri = $@"https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.{curr.Item1}.{curr.Item2}.SP00.A?startPeriod={fromDate.AddDays(-3).ToString("yyyy-MM-dd")}&endPeriod={toDate.AddDays(3).ToString("yyyy-MM-dd")}";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                responseData = await response.Content.ReadAsStringAsync();

            var data = responseData.XmlDeserializeFromString<GenericData>();
            var listedData = data.MappGenericDataToResponseModelList();

            FillGapDates(ref listedData);

            return listedData;
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

        private void FillGapDates(ref List<ExchangeResponseModel> list)
        {
            if (list.Count < 1) return;

            list = list.OrderBy(x => x.DateOfExchangeRate)
                       .ThenBy(x => x.CurrencyFrom)
                       .ThenBy(x => x.CurrencyTo)
                       .ToList();

            var lastItem = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                var i_date = list[i].DateOfExchangeRate;
                if((i_date - lastItem.DateOfExchangeRate).Days > 1)
                {
                    var model = new ExchangeResponseModel
                    {
                        CurrencyFrom = lastItem.CurrencyFrom,
                        CurrencyTo = lastItem.CurrencyTo,
                        ExchangeRate = lastItem.ExchangeRate,
                        DateOfExchangeRate = lastItem.DateOfExchangeRate.AddDays(1)
                    };
                    list.Insert(i, model);
                }
                lastItem = list[i];
            }
        }
    }
}
