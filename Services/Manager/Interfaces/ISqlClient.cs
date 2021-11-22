using ExchangeRateAPI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Services.Manager.Interfaces
{
    public interface ISqlClient
    {
        ExchangeDBContext DbContext { get; }
    }
}
