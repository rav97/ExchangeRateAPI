using ExchangeRateAPI.Database;
using ExchangeRateAPI.Models;
using ExchangeRateAPI.Services.Manager.Interfaces;
using ExchangeRateAPI.Services.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Repositories
{
    public class ExchangeRatesRepository : BaseRepository<ExchangeRate>, IExchangeRatesRepository
    {
        public ExchangeRatesRepository(ISqlClient sqlClient) : base(sqlClient)
        {

        }

        public bool FillExchangeRates(IEnumerable<ExchangeResponseModel> responseModels, DateTime fromDate, DateTime toDate)
        {
            string fromCurr = string.Empty;
            string toCurr = string.Empty;

            var responseCopy = responseModels.ToList();

            //convert dictionary to string in which database can look for currency code
            responseCopy.ForEach(x => { fromCurr += $"{x.CurrencyFrom} "; toCurr += $"{x.CurrencyTo} "; });

            var knownRates = db.ExchangeRates
                               .Where(r => r.Date >= fromDate)
                               .Where(r => r.Date <= toDate)
                               .Where(r => fromCurr.Contains(r.CurrencyFrom))
                               .Where(r => toCurr.Contains(r.CurrencyTo))
                               .ToList();

            var unknownRates = responseCopy.Where(r => !knownRates.Any(k => k.CurrencyFrom == r.CurrencyFrom && k.CurrencyTo == r.CurrencyTo && k.Date == r.DateOfExchangeRate)).ToList();

            foreach(var e in unknownRates)
            {
                db.ExchangeRates.Add(new ExchangeRate
                {
                    Date = e.DateOfExchangeRate,
                    CurrencyFrom = e.CurrencyFrom,
                    CurrencyTo = e.CurrencyTo,
                    ExchangeRate1 = e.ExchangeRate
                });
            }

            return db.SaveChanges() > 0; 
        }

        public List<ExchangeRate> SelectExchangeRates(Dictionary<string, string> currencies, DateTime fromDate, DateTime toDate)
        {
            string fromCurr = string.Empty;
            string toCurr = string.Empty;

            //convert dictionary to string in which database can look for currency code
            currencies.ToList().ForEach(x => { fromCurr += $"{x.Key} "; toCurr += $"{x.Value} "; });

            var result = db.ExchangeRates
                           .Where(r => r.Date >= fromDate)
                           .Where(r => r.Date <= toDate)
                           .Where(r => fromCurr.Contains(r.CurrencyFrom))
                           .Where(r => toCurr.Contains(r.CurrencyTo))
                           .ToList();

            //ECB API returns cartesian sets, this API is more precise, so need to exclude currency rates that wasn't requested
            result.RemoveAll(x => currencies[x.CurrencyFrom] != x.CurrencyTo);

            return result;
        }
    }
}
