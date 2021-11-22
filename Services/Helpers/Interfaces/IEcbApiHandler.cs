using ExchangeRateAPI.Models.ECB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Helpers.Interfaces
{
    public interface IEcbApiHandler
    {
        Task<GenericData> GetExchangeData(Dictionary<string, string> currencies, DateTime fromDate, DateTime toDate);
    }
}
