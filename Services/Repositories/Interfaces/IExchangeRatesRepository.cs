using ExchangeRateAPI.Database;
using ExchangeRateAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Repositories.Interfaces
{
    public interface IExchangeRatesRepository : IBaseRepository<ExchangeRate>
    {
        List<ExchangeRate> SelectExchangeRates(Dictionary<string, string> currencies, DateTime fromDate, DateTime toDate);
        bool FillExchangeRates(IEnumerable<ExchangeResponseModel> responseModels, DateTime fromDate, DateTime toDate);
    }
}
