using ExchangeRateAPI.Models;
using ExchangeRateAPI.Models.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace ExchangeRateAPI.IntegrationTests
{
    public class ExchangeControllerTests : IntegrationTest
    {
        private const string baseRoute = @"/Exchange";
        private const string queryParams = @"?CurrencyCodes={CurrencyCodes}&StartDate={StartDate}&EndDate={EndDate}&ApiKey={ApiKey}";

        #region [ HELPFUL METHODS ]

        #warning REPLACE API KEY WITH WALID VALUE
        private string GetRequestString(Dictionary<string, string> currencies, DateTime startDate, DateTime? endDate = null, string apiKey = @"MTE6Mzc6MTU6NDM3LTIyLjA1LjE5")
        {
            var CurrencyCodes = WebUtility.UrlEncode(JsonConvert.SerializeObject(currencies));
            var StartDate = startDate.Date.ToString("yyyy-MM-dd");
            var ApiKey = apiKey ?? "";

            string request = queryParams;

            if (endDate == null)
                request = request.Replace(@"&EndDate={EndDate}", "");
            else
                request = request.Replace(@"{EndDate}", endDate.Value.Date.ToString("yyyy-MM-dd"));

            string requestString = baseRoute + request.Replace("{CurrencyCodes}", CurrencyCodes)
                                                      .Replace("{StartDate}", StartDate)
                                                      .Replace("{ApiKey}", ApiKey);

            return requestString;
        }

        #endregion


        [Fact]
        public async Task Get_PassFutureDate_ReturnsNotFound()
        {
            #region [ ARRANGE ]

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            currencies.Add("EUR", "PLN");

            var StartDate = DateTime.Now.AddDays(7);
            var EndDate = DateTime.Now.AddDays(8);

            var requestString = GetRequestString(currencies, StartDate, EndDate);

            #endregion

            #region [ ACT ]

            var response = await _testClient.GetAsync(requestString);
            var data = await response.Content.ReadAsStringAsync();
            var dataObject = JsonConvert.DeserializeObject<ErrorResponse>(data);

            #endregion

            #region [ ASSERT ]

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            dataObject.Succeded.Should().BeFalse();
            dataObject.Data.Should().NotBeEmpty();

            #endregion
        }

        [Fact]
        public async Task Get_PassDayOffDate_ReturnsLastKnown()
        {
            #region [ ARRANGE ]

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            currencies.Add("PLN", "EUR");

            var StartDate = Convert.ToDateTime("2021-11-21"); //Sunday has no data, so API should return currency rate from friday 2021-11-19
            var StartDate2 = Convert.ToDateTime("2021-11-19");

            string requestString = GetRequestString(currencies, StartDate);
            string requestString2 = GetRequestString(currencies, StartDate2);

            #endregion

            #region [ ACT ]

            var response = await _testClient.GetAsync(requestString);
            var data = await response.Content.ReadAsStringAsync();
            var dataObject = JsonConvert.DeserializeObject<Response<List<ExchangeResponseModel>>>(data);

            var response2 = await _testClient.GetAsync(requestString2);
            var data2 = await response2.Content.ReadAsStringAsync();
            var dataObject2 = JsonConvert.DeserializeObject<Response<List<ExchangeResponseModel>>>(data2);

            #endregion

            #region [ ASSERT ]

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dataObject.Succeded.Should().BeTrue();
            dataObject.Data.Should().HaveCount(1);
            dataObject.Data.Should().OnlyContain(x => x.ExchangeRate == dataObject2.Data[0].ExchangeRate);

            #endregion
        }

        [Fact]
        public async Task Get_AskForWeekCurrency_Returns7Elements()
        {
            #region [ ARRANGE ]

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            currencies.Add("PLN", "EUR");

            var StartDate = DateTime.Now.AddDays(-6);
            var EndDate = DateTime.Now;

            var requestString = GetRequestString(currencies, StartDate, EndDate);

            #endregion

            #region [ ACT ]

            var response = await _testClient.GetAsync(requestString);
            var data = await response.Content.ReadAsStringAsync();
            var dataObject = JsonConvert.DeserializeObject<Response<List<ExchangeResponseModel>>>(data);

            #endregion

            #region [ ASSERT ]

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dataObject.Succeded.Should().BeTrue();
            dataObject.Data.Should().HaveCount(7);

            #endregion
        }

        [Fact]
        public async Task Get_AskForWeekCurrencies_Returns14Elements()
        {
            #region [ ARRANGE ]

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            currencies.Add("PLN", "EUR");
            currencies.Add("USD", "EUR");

            var StartDate = DateTime.Now.AddDays(-6);
            var EndDate = DateTime.Now;

            var requestString = GetRequestString(currencies, StartDate, EndDate);

            #endregion

            #region [ ACT ]

            var response = await _testClient.GetAsync(requestString);
            var data = await response.Content.ReadAsStringAsync();
            var dataObject = JsonConvert.DeserializeObject<Response<List<ExchangeResponseModel>>>(data);

            #endregion

            #region [ ASSERT ]

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dataObject.Succeded.Should().BeTrue();
            dataObject.Data.Should().HaveCount(14);

            #endregion
        }

        [Fact]
        public async Task Get_AskForWeekCurrenciesOnlyOneHasData_ReturnsWhatHave()
        {
            #region [ ARRANGE ]

            Dictionary<string, string> currencies = new Dictionary<string, string>();
            currencies.Add("PLN", "EUR");
            currencies.Add("USD", "PLN"); // ECB has no data on that currency

            var StartDate = DateTime.Now.AddDays(-6);
            var EndDate = DateTime.Now;

            var requestString = GetRequestString(currencies, StartDate, EndDate);

            #endregion

            #region [ ACT ]

            var response = await _testClient.GetAsync(requestString);
            var data = await response.Content.ReadAsStringAsync();
            var dataObject = JsonConvert.DeserializeObject<Response<List<ExchangeResponseModel>>>(data);

            #endregion

            #region [ ASSERT ]

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            dataObject.Succeded.Should().BeTrue();
            dataObject.Data.Should().HaveCount(7);
            dataObject.Data.Should().OnlyContain(x => x.CurrencyFrom == "PLN" && x.CurrencyTo == "EUR");

            #endregion
        }

    }
}
