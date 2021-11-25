using ExchangeRateAPI.Database;
using ExchangeRateAPI.Models;
using ExchangeRateAPI.Models.ECB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Mappers
{
    public static class ExchangeResponseMapper
    {
        public static List<ExchangeResponseModel> MappGenericDataToResponseModelList(this GenericData data, Dictionary<string,string> currencies)
        {
            List<ExchangeResponseModel> result = new List<ExchangeResponseModel>();

            try
            {
                if (data != null)
                {
                    foreach (var ser in data.DataSet.Series)
                    {
                        string fromCurr = null, toCurr = null;
                        foreach (var key in ser.SeriesKey)
                        {
                            if (key.id == "CURRENCY")
                                fromCurr = key.value;
                            if (key.id == "CURRENCY_DENOM")
                                toCurr = key.value;

                            if (fromCurr != null && toCurr != null)
                                break;
                        }

                        //ECB API returns cartesian sets, this API is more precise, so need to exclude currency rates that wasn't requested
                        if (currencies[fromCurr] == toCurr)
                        {
                            foreach (var obs in ser.Obs)
                            {
                                ExchangeResponseModel item = new ExchangeResponseModel()
                                {
                                    CurrencyFrom = fromCurr,
                                    CurrencyTo = toCurr,
                                    DateOfExchangeRate = obs.ObsDimension.value,
                                    ExchangeRate = obs.ObsValue.value
                                };
                                result.Add(item);
                            }
                        }
                    }
                }
            }
            catch { }

            return result;
        }

        public static List<ExchangeResponseModel> MappExchangeRatesToResponseModelList(this IEnumerable<ExchangeRate> data)
        {
            List<ExchangeResponseModel> result = new List<ExchangeResponseModel>();
            foreach(var e in data)
            {
                result.Add(new ExchangeResponseModel
                {
                    CurrencyFrom = e.CurrencyFrom,
                    CurrencyTo = e.CurrencyTo,
                    DateOfExchangeRate = e.Date,
                    ExchangeRate = e.ExchangeRate1
                });
            }
            return result;
        }
    }
}
