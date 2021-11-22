using ExchangeRateAPI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Repositories.Interfaces
{
    public interface IApiKeysRepository : IBaseRepository<ApiKey>
    {
        string GenerateNewApiKey(out bool result);
        bool CheckKey(string passedKey);
    }
}
